using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace aclogview
{
    static class PCapReader
    {
        public static List<PacketRecord> LoadPcap(string fileName, ref bool abort)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    uint magicNumber = binaryReader.ReadUInt32();

                    binaryReader.BaseStream.Position = 0;

                    if (magicNumber == 0xA1B2C3D4 || magicNumber == 0xD4C3B2A1)
                        return loadPcapContent(binaryReader, ref abort);

                    return loadPcapngContent(binaryReader, ref abort);
                }
            }
        }

        private static List<PacketRecord> loadPcapContent(BinaryReader binaryReader, ref bool abort)
        {
            List<PacketRecord> results = new List<PacketRecord>();

            /*PcapHeader pcapHeader = */
            PcapHeader.read(binaryReader);

            int curPacket = 0;

            while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
            {
                if (abort)
                    break;

                curPacket++;

                if (binaryReader.BaseStream.Length - binaryReader.BaseStream.Position < 16)
                {
                    //MessageBox.Show("Stream cut short (packet " + curPacket + "), stopping read: " + (binaryReader.BaseStream.Length - binaryReader.BaseStream.Position));
                    break;
                }

                PcapRecordHeader recordHeader = PcapRecordHeader.read(binaryReader);

                if (recordHeader.inclLen > 50000)
                {
                    //MessageBox.Show("Enormous packet (packet " + curPacket + "), stopping read: " + recordHeader.inclLen);
                    break;
                }

                // Make sure there's enough room for an ethernet header
                if (recordHeader.inclLen < 14)
                {
                    binaryReader.BaseStream.Position += recordHeader.inclLen;
                    continue;
                }

                var packetRecord = readPacketRecordData(binaryReader, recordHeader.inclLen, recordHeader.tsSec, curPacket);

                if (packetRecord == null)
                    break;

                results.Add(packetRecord);
            }

            return results;
        }

        private static List<PacketRecord> loadPcapngContent(BinaryReader binaryReader, ref bool abort)
        {
            List<PacketRecord> results = new List<PacketRecord>();

            int curPacket = 0;

            while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
            {
                if (abort)
                    break;

                curPacket++;

                if (binaryReader.BaseStream.Length - binaryReader.BaseStream.Position < 8)
                {
                    //MessageBox.Show("Stream cut short (packet " + curPacket + "), stopping read: " + (binaryReader.BaseStream.Length - binaryReader.BaseStream.Position));
                    break;
                }

                long recordStartPos = binaryReader.BaseStream.Position;

                uint blockType = binaryReader.ReadUInt32();
                uint blockTotalLength = binaryReader.ReadUInt32();

                if (blockType == 6)
                {
                    /*uint interfaceID = */binaryReader.ReadUInt32();
                    /*uint tsHigh = */binaryReader.ReadUInt32();
                    uint tsLow = binaryReader.ReadUInt32();
                    uint capturedLen = binaryReader.ReadUInt32();
                    /*uint packetLen = */binaryReader.ReadUInt32();

                    var packetRecord = readPacketRecordData(binaryReader, capturedLen, tsLow, curPacket);

                    if (packetRecord == null)
                        break;

                    results.Add(packetRecord);
                }
                else if (blockType == 3)
                {
                    /*uint packetLen = */binaryReader.ReadUInt32();
                    uint capturedLen = blockTotalLength - 16;

                    var packetRecord = readPacketRecordData(binaryReader, capturedLen, 0, curPacket);

                    if (packetRecord == null)
                        break;

                    results.Add(packetRecord);
                }

                binaryReader.BaseStream.Position += blockTotalLength - (binaryReader.BaseStream.Position - recordStartPos);
            }

            return results;
        }

        private static PacketRecord readPacketRecordData(BinaryReader binaryReader, long len, uint tsSec, int curPacket)
        {
            // Begin reading headers
            long packetStartPos = binaryReader.BaseStream.Position;

            EthernetHeader ethernetHeader = EthernetHeader.read(binaryReader);

            // Skip non-IP packets
            if (ethernetHeader.proto != 8)
            {
                binaryReader.BaseStream.Position += len - (binaryReader.BaseStream.Position - packetStartPos);
                return null;
            }

            IpHeader ipHeader = IpHeader.read(binaryReader);

            // Skip non-UDP packets
            if (ipHeader.proto != 17)
            {
                binaryReader.BaseStream.Position += len - (binaryReader.BaseStream.Position - packetStartPos);
                return null;
            }

            UdpHeader udpHeader = UdpHeader.read(binaryReader);

            bool isSend = (udpHeader.dPort >= 9000 && udpHeader.dPort <= 9013);
            bool isRecv = (udpHeader.sPort >= 9000 && udpHeader.sPort <= 9013);

            // Skip non-AC-port packets
            if (!isSend && !isRecv)
            {
                binaryReader.BaseStream.Position += len - (binaryReader.BaseStream.Position - packetStartPos);
                return null;
            }

            long headersSize = binaryReader.BaseStream.Position - packetStartPos;

            // Begin reading non-header packet content
            StringBuilder packetHeadersStr = new StringBuilder();
            StringBuilder packetTypeStr = new StringBuilder();

            PacketRecord packet = new PacketRecord();
            packet.index = (curPacket - 1);
            packet.isSend = isSend;
            packet.tsSec = tsSec;
            packet.netPacket = new NetPacket();
            packet.data = binaryReader.ReadBytes((int)(len - headersSize));
            packet.extraInfo = "";
            BinaryReader packetReader = new BinaryReader(new MemoryStream(packet.data));
            try
            {
                ProtoHeader pHeader = ProtoHeader.read(packetReader);

                readOptionalHeaders(packet, pHeader.header_, packetHeadersStr, packetReader);

                if (packetReader.BaseStream.Position == packetReader.BaseStream.Length)
                    packetTypeStr.Append("<Header Only>");

                uint HAS_FRAGS_MASK = 0x4; // See SharedNet::SplitPacketData

                if ((pHeader.header_ & HAS_FRAGS_MASK) != 0)
                {
                    bool first = true;

                    while (packetReader.BaseStream.Position != packetReader.BaseStream.Length)
                    {
                        if (!first)
                            packetTypeStr.Append(" + ");

                        readPacket(packet, packetTypeStr, packetReader);
                        first = false;
                    }
                }

                if (packetReader.BaseStream.Position != packetReader.BaseStream.Length)
                    packet.extraInfo = "Didnt read entire packet! " + packet.extraInfo;
            }
            catch (OutOfMemoryException e)
            {
                //MessageBox.Show("Out of memory (packet " + curPacket + "), stopping read: " + e);
                return null;
            }
            catch (Exception e)
            {
                packet.extraInfo += "EXCEPTION: " + e.Message + " " + e.StackTrace;
            }

            packet.packetHeadersStr = packetHeadersStr.ToString();
            packet.packetTypeStr = packetTypeStr.ToString();

            return packet;
        }

        private static void readPacket(PacketRecord packet, StringBuilder packetTypeStr, BinaryReader packetReader)
        {
            BlobFrag newFrag = new BlobFrag();
            newFrag.memberHeader_ = BlobFragHeader_t.read(packetReader);
            newFrag.dat_ = packetReader.ReadBytes(newFrag.memberHeader_.blobFragSize - 16); // 16 == size of frag header

            packet.netPacket.fragList_.Add(newFrag);

            BinaryReader fragDataReader = new BinaryReader(new MemoryStream(newFrag.dat_));

            if (newFrag.memberHeader_.blobNum != 0)
            {
                packetTypeStr.Append("FragData[");
                packetTypeStr.Append(newFrag.memberHeader_.blobNum);
                packetTypeStr.Append("]");
            }
            else
            {
                PacketOpcode opcode = Util.readOpcode(fragDataReader);
                packet.opcodes.Add(opcode);
                packetTypeStr.Append(opcode);
            }
        }

        private static void readOptionalHeaders(PacketRecord packet, uint header_, StringBuilder packetHeadersStr, BinaryReader packetReader)
        {
            long readStartPos = packetReader.BaseStream.Position;

            if ((header_ & CServerSwitchStructHeader.mask) != 0)
            {
                /*CServerSwitchStruct serverSwitchStruct = */CServerSwitchStruct.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Server Switch");
            }

            if ((header_ & LogonServerAddrHeader.mask) != 0)
            {
                /*sockaddr_in serverAddr = */sockaddr_in.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Logon Server Addr");
            }

            if ((header_ & CEmptyHeader1.mask) != 0)
            {
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Empty Header 1");
            }

            if ((header_ & CReferralStructHeader.mask) != 0)
            {
                /*CReferralStruct referralStruct = */CReferralStruct.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Referral");
            }

            if ((header_ & NakHeader.mask) != 0)
            {
                /*CSeqIDListHeader nakSeqIDs = */NakHeader.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Nak");
            }

            if ((header_ & EmptyAckHeader.mask) != 0)
            {
                /*CSeqIDListHeader ackSeqIDs = */EmptyAckHeader.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Empty Ack");
            }

            if ((header_ & PakHeader.mask) != 0)
            {
                /*PakHeader pakHeader = */PakHeader.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Pak");
            }

            if ((header_ & CEmptyHeader2.mask) != 0)
            {
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Empty Header 2");
            }

            if ((header_ & CLogonHeader.mask) != 0)
            {
                CLogonHeader.HandshakeWireData handshakeData = CLogonHeader.HandshakeWireData.read(packetReader);
                /*byte[] authData = */packetReader.ReadBytes((int)handshakeData.cbAuthData);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Logon");
            }

            if ((header_ & ULongHeader.mask) != 0)
            {
                /*ULongHeader ulongHeader = */ULongHeader.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("ULong 1");
            }

            if ((header_ & CConnectHeader.mask) != 0)
            {
                /*CConnectHeader.HandshakeWireData handshakeData = */CConnectHeader.HandshakeWireData.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Connect");
            }

            if ((header_ & ULongHeader2.mask) != 0)
            {
                /*ULongHeader2 ulongHeader = */ULongHeader2.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("ULong 2");
            }

            if ((header_ & NetErrorHeader.mask) != 0)
            {
                /*NetError netError = */NetError.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Net Error");
            }

            if ((header_ & NetErrorHeader_cs_DisconnectReceived.mask) != 0)
            {
                /*NetError netError = */NetError.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Net Error Disconnect");
            }

            if ((header_ & CICMDCommandStructHeader.mask) != 0)
            {
                /*CICMDCommandStruct icmdStruct = */CICMDCommandStruct.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("ICmd");
            }

            if ((header_ & CTimeSyncHeader.mask) != 0)
            {
                /*CTimeSyncHeader timeSyncHeader = */CTimeSyncHeader.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Time Sync");
            }

            if ((header_ & CEchoRequestHeader.mask) != 0)
            {
                /*CEchoRequestHeader echoRequestHeader = */CEchoRequestHeader.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Echo Request");
            }

            if ((header_ & CEchoResponseHeader.mask) != 0)
            {
                /*CEchoResponseHeader.CEchoResponseHeaderWireData echoResponseData = */CEchoResponseHeader.CEchoResponseHeaderWireData.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Echo Response");
            }

            if ((header_ & CFlowStructHeader.mask) != 0)
            {
                /*CFlowStruct flowStruct = */CFlowStruct.read(packetReader);
                if (packetHeadersStr.Length != 0)
                    packetHeadersStr.Append(" | ");
                packetHeadersStr.Append("Flow");
            }

            packet.optionalHeadersLen = (int)(packetReader.BaseStream.Position - readStartPos);
        }
    }
}

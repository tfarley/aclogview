using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using aclogview.Properties;

namespace aclogview {
    public partial class Form1 : Form {
        private ListViewItemComparer comparer = new ListViewItemComparer();
        public List<MessageProcessor> messageProcessors = new List<MessageProcessor>();
        private long curPacket;

        private string[] args;

        /// <summary>
        /// Add multiple opcodes to highlight to this list.<para /> 
        /// Each opcode will have a different row highlight color.
        /// </summary>
        private readonly List<int> opCodesToHighlight = new List<int>();

        private StringBuilder strbuilder = new StringBuilder();

        public Form1(string[] args) {
            InitializeComponent();

            this.args = args;
        }

        private void Form1_Load(object sender, EventArgs e) {
            Util.initReaders();
            messageProcessors.Add(new CM_Admin());
            messageProcessors.Add(new CM_Advocate());
            messageProcessors.Add(new CM_Allegiance());
            messageProcessors.Add(new CM_Character());
            messageProcessors.Add(new CM_Combat());
            messageProcessors.Add(new CM_Communication());
            messageProcessors.Add(new CM_Examine());
            messageProcessors.Add(new CM_Fellowship());
            messageProcessors.Add(new CM_Game());
            messageProcessors.Add(new CM_House());
            messageProcessors.Add(new CM_Inventory());
            messageProcessors.Add(new CM_Item());
            messageProcessors.Add(new CM_Login());
            messageProcessors.Add(new CM_Magic());
            messageProcessors.Add(new CM_Misc());
            messageProcessors.Add(new CM_Movement());
            messageProcessors.Add(new CM_Physics());
            messageProcessors.Add(new CM_Qualities());
            messageProcessors.Add(new CM_Social());
            messageProcessors.Add(new CM_Trade());
            messageProcessors.Add(new CM_Train());
            messageProcessors.Add(new CM_Vendor());
            messageProcessors.Add(new CM_Writing());
            messageProcessors.Add(new Proto_UI());

            if (args != null && args.Length >= 2)
            {
                int opcode;
                if (int.TryParse(args[1], out opcode))
                    opCodesToHighlight.Add(opcode);
            }
            if (args != null && args.Length >= 1)
                loadPcap(args[0]);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Settings.Default.Save();
        }


        private void readPacket(PacketRecord packet, StringBuilder packetTypeStr, BinaryReader packetReader) {
            BlobFrag newFrag = new BlobFrag();
            newFrag.memberHeader_ = BlobFragHeader_t.read(packetReader);
            newFrag.dat_ = packetReader.ReadBytes(newFrag.memberHeader_.blobFragSize - 16); // 16 == size of frag header

            packet.netPacket.fragList_.Add(newFrag);

            BinaryReader fragDataReader = new BinaryReader(new MemoryStream(newFrag.dat_));

            if (newFrag.memberHeader_.blobNum != 0) {
                packetTypeStr.Append("FragData[");
                packetTypeStr.Append(newFrag.memberHeader_.blobNum);
                packetTypeStr.Append("]");
            } else {
                PacketOpcode opcode = Util.readOpcode(fragDataReader);
                packet.opcodes.Add(opcode);
                packetTypeStr.Append(opcode.ToString());
            }
        }

        private void readOptionalHeaders(PacketRecord packet, uint header_, StringBuilder packetHeadersStr, BinaryReader packetReader) {
            long readStartPos = packetReader.BaseStream.Position;

            if ((header_ & CServerSwitchStructHeader.mask) != 0) {
                CServerSwitchStruct serverSwitchStruct = CServerSwitchStruct.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Server Switch");
            }

            if ((header_ & LogonServerAddrHeader.mask) != 0) {
                sockaddr_in serverAddr = sockaddr_in.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Logon Server Addr");
            }

            if ((header_ & CEmptyHeader1.mask) != 0) {
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Empty Header 1");
            }

            if ((header_ & CReferralStructHeader.mask) != 0) {
                CReferralStruct referralStruct = CReferralStruct.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Referral");
            }

            if ((header_ & NakHeader.mask) != 0) {
                CSeqIDListHeader nakSeqIDs = NakHeader.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Nak");
            }

            if ((header_ & EmptyAckHeader.mask) != 0) {
                CSeqIDListHeader ackSeqIDs = EmptyAckHeader.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Empty Ack");
            }

            if ((header_ & PakHeader.mask) != 0) {
                PakHeader pakHeader = PakHeader.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Pak");
            }

            if ((header_ & CEmptyHeader2.mask) != 0) {
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Empty Header 2");
            }

            if ((header_ & CLogonHeader.mask) != 0) {
                CLogonHeader.HandshakeWireData handshakeData = CLogonHeader.HandshakeWireData.read(packetReader);
                byte[] authData = packetReader.ReadBytes((int)handshakeData.cbAuthData);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Logon");
            }

            if ((header_ & ULongHeader.mask) != 0) {
                ULongHeader ulongHeader = ULongHeader.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("ULong 1");
            }

            if ((header_ & CConnectHeader.mask) != 0) {
                CConnectHeader.HandshakeWireData handshakeData = CConnectHeader.HandshakeWireData.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Connect");
            }

            if ((header_ & ULongHeader2.mask) != 0) {
                ULongHeader2 ulongHeader = ULongHeader2.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("ULong 2");
            }

            if ((header_ & NetErrorHeader.mask) != 0) {
                NetError netError = NetError.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Net Error");
            }

            if ((header_ & NetErrorHeader_cs_DisconnectReceived.mask) != 0) {
                NetError netError = NetError.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Net Error Disconnect");
            }

            if ((header_ & CICMDCommandStructHeader.mask) != 0) {
                CICMDCommandStruct icmdStruct = CICMDCommandStruct.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("ICmd");
            }

            if ((header_ & CTimeSyncHeader.mask) != 0) {
                CTimeSyncHeader timeSyncHeader = CTimeSyncHeader.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Time Sync");
            }

            if ((header_ & CEchoRequestHeader.mask) != 0) {
                CEchoRequestHeader echoRequestHeader = CEchoRequestHeader.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Echo Request");
            }

            if ((header_ & CEchoResponseHeader.mask) != 0) {
                CEchoResponseHeader.CEchoResponseHeaderWireData echoResponseData = CEchoResponseHeader.CEchoResponseHeaderWireData.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Echo Response");
            }

            if ((header_ & CFlowStructHeader.mask) != 0) {
                CFlowStruct flowStruct = CFlowStruct.read(packetReader);
                if (packetHeadersStr.Length != 0) {
                    packetHeadersStr.Append(" | ");
                }
                packetHeadersStr.Append("Flow");
            }

            packet.optionalHeadersLen = (int)(packetReader.BaseStream.Position - readStartPos);
        }

        List<PacketRecord> records = new List<PacketRecord>();
        List<ListViewItem> listItems = new List<ListViewItem>();

        private int readPacketRecordData(BinaryReader binaryReader, long len, uint tsSec, long curPacket, bool dontList) {
            // Begin reading headers
            long packetStartPos = binaryReader.BaseStream.Position;

            EthernetHeader ethernetHeader = EthernetHeader.read(binaryReader);

            // Skip non-IP packets
            if (ethernetHeader.proto != 8) {
                binaryReader.BaseStream.Position += len - (binaryReader.BaseStream.Position - packetStartPos);
                return 1;
            }

            IpHeader ipHeader = IpHeader.read(binaryReader);

            // Skip non-UDP packets
            if (ipHeader.proto != 17) {
                binaryReader.BaseStream.Position += len - (binaryReader.BaseStream.Position - packetStartPos);
                return 1;
            }

            UdpHeader udpHeader = UdpHeader.read(binaryReader);

            bool isSend = (udpHeader.dPort >= 9000 && udpHeader.dPort <= 9013);
            bool isRecv = (udpHeader.sPort >= 9000 && udpHeader.sPort <= 9013);

            // Skip non-AC-port packets
            if (!isSend && !isRecv) {
                binaryReader.BaseStream.Position += len - (binaryReader.BaseStream.Position - packetStartPos);
                return 1;
            }

            long headersSize = binaryReader.BaseStream.Position - packetStartPos;

            // Begin reading non-header packet content
            StringBuilder packetHeadersStr = new StringBuilder();
            StringBuilder packetTypeStr = new StringBuilder();

            PacketRecord packet = new PacketRecord();
            packet.index = records.Count;
            packet.isSend = isSend;
            packet.tsSec = tsSec;
            packet.netPacket = new NetPacket();
            packet.data = binaryReader.ReadBytes((int)(len - headersSize));
            packet.extraInfo = "";
            BinaryReader packetReader = new BinaryReader(new MemoryStream(packet.data));
            try {
                ProtoHeader pHeader = ProtoHeader.read(packetReader);

                readOptionalHeaders(packet, pHeader.header_, packetHeadersStr, packetReader);

                if (packetReader.BaseStream.Position == packetReader.BaseStream.Length) {
                    packetTypeStr.Append("<Header Only>");
                }

                uint HAS_FRAGS_MASK = 0x4; // See SharedNet::SplitPacketData
                if ((pHeader.header_ & HAS_FRAGS_MASK) != 0) {
                    bool first = true;
                    while (packetReader.BaseStream.Position != packetReader.BaseStream.Length) {
                        if (!first) {
                            packetTypeStr.Append(" + ");
                        }
                        readPacket(packet, packetTypeStr, packetReader);
                        first = false;
                    }
                }

                if (packetReader.BaseStream.Position != packetReader.BaseStream.Length) {
                    packet.extraInfo = "Didnt read entire packet! " + packet.extraInfo;
                }
            } catch (OutOfMemoryException e) {
                //MessageBox.Show("Out of memory (packet " + curPacket + "), stopping read: " + e);
                return 2;
            } catch (Exception e) {
                packet.extraInfo += "EXCEPTION: " + e.Message + " " + e.StackTrace;
            }
            packet.packetHeadersStr = packetHeadersStr.ToString();
            packet.packetTypeStr = packetTypeStr.ToString();

            records.Add(packet);

            if (!dontList) {
                ListViewItem newItem = new ListViewItem(packet.index.ToString());
                newItem.SubItems.Add(packet.isSend ? "Send" : "Recv");
                newItem.SubItems.Add(packet.tsSec.ToString());
                newItem.SubItems.Add(packet.packetHeadersStr);
                newItem.SubItems.Add(packet.packetTypeStr);
                newItem.SubItems.Add(packet.data.Length.ToString());
                newItem.SubItems.Add(packet.extraInfo);
                listItems.Add(newItem);
            }

            return 0;
        }

        private void loadPcapContent(BinaryReader binaryReader, bool dontList) {
            PcapHeader pcapHeader = PcapHeader.read(binaryReader);

            while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length) {
                curPacket++;

                if (binaryReader.BaseStream.Length - binaryReader.BaseStream.Position < 16) {
                    //MessageBox.Show("Stream cut short (packet " + curPacket + "), stopping read: " + (binaryReader.BaseStream.Length - binaryReader.BaseStream.Position));
                    break;
                }

                PcapRecordHeader recordHeader = PcapRecordHeader.read(binaryReader);

                if (recordHeader.inclLen > 50000) {
                    //MessageBox.Show("Enormous packet (packet " + curPacket + "), stopping read: " + recordHeader.inclLen);
                    break;
                }

                // Make sure there's enough room for an ethernet header
                if (recordHeader.inclLen < 14) {
                    binaryReader.BaseStream.Position += recordHeader.inclLen;
                    continue;
                }

                if (readPacketRecordData(binaryReader, recordHeader.inclLen, recordHeader.tsSec, curPacket, dontList) == 2) {
                    break;
                }
            }
        }

        private void loadPcapngContent(BinaryReader binaryReader, bool dontList) {
            while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length) {
                curPacket++;

                if (binaryReader.BaseStream.Length - binaryReader.BaseStream.Position < 8) {
                    //MessageBox.Show("Stream cut short (packet " + curPacket + "), stopping read: " + (binaryReader.BaseStream.Length - binaryReader.BaseStream.Position));
                    break;
                }

                long recordStartPos = binaryReader.BaseStream.Position;

                uint blockType = binaryReader.ReadUInt32();
                uint blockTotalLength = binaryReader.ReadUInt32();

                if (blockType == 6) {
                    uint interfaceID = binaryReader.ReadUInt32();
                    uint tsHigh = binaryReader.ReadUInt32();
                    uint tsLow = binaryReader.ReadUInt32();
                    uint capturedLen = binaryReader.ReadUInt32();
                    uint packetLen = binaryReader.ReadUInt32();

                    if (readPacketRecordData(binaryReader, capturedLen, tsLow, curPacket, dontList) == 2) {
                        break;
                    }
                } else if (blockType == 3) {
                    uint packetLen = binaryReader.ReadUInt32();
                    uint capturedLen = blockTotalLength - 16;

                    if (readPacketRecordData(binaryReader, capturedLen, 0, curPacket, dontList) == 2) {
                        break;
                    }
                }

                binaryReader.BaseStream.Position += blockTotalLength - (binaryReader.BaseStream.Position - recordStartPos);
            }
        }

        private void loadPcap(string fileName, bool dontList = false) {
            this.Text = "AC Log View - " + Path.GetFileName(fileName);

            if (opCodesToHighlight.Count > 0)
            {
                this.Text += "              Highlighted OpCodes: ";
                foreach (var opcode in opCodesToHighlight)
                    Text += opcode + " (" + opcode.ToString("X4") + "),";
            }

            records.Clear();
            listItems.Clear();

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                using (BinaryReader binaryReader = new BinaryReader(fileStream)) {
                    uint magicNumber = binaryReader.ReadUInt32();
                    binaryReader.BaseStream.Position = 0;

                    if (magicNumber == 0xA1B2C3D4 || magicNumber == 0xD4C3B2A1) {
                        loadPcapContent(binaryReader, dontList);
                    } else {
                        loadPcapngContent(binaryReader, dontList);
                    }
                }
            }

            if (!dontList && records.Count > 0) {
                listView_Packets.VirtualListSize = records.Count;

                listView_Packets.RedrawItems(0, records.Count - 1, false);
                updateData();
            } else {
                listView_Packets.VirtualListSize = 0;
            }
        }

        private void listView_Packets_ColumnClick(object sender, ColumnClickEventArgs e) {
            if (comparer.col == e.Column) {
                comparer.reverse = !comparer.reverse;
            }
            comparer.col = e.Column;
            listItems.Sort(comparer);

            listView_Packets.RedrawItems(0, records.Count - 1, false);
            updateData();
        }

        class ListViewItemComparer : IComparer<ListViewItem> {
            public int col;
            public bool reverse;

            public int Compare(ListViewItem x, ListViewItem y) {
                int result = 0;
                if (col == 0 || col == 2 || col == 5) {
                    result = CompareUInt(x.SubItems[col].Text, y.SubItems[col].Text);
                } else {
                    result = CompareString(x.SubItems[col].Text, y.SubItems[col].Text);
                }

                if (reverse) {
                    result = -result;
                }

                return result;
            }

            public int CompareUInt(string x, string y) {
                return UInt32.Parse(x).CompareTo(UInt32.Parse(y));
            }

            public int CompareString(string x, string y) {
                return String.Compare(x, y);
            }
        }

        private void updateData() {
            updateText();
            updateTree();
        }

        private void updateText() {
            textBox_PacketData.Clear();

            if (listView_Packets.SelectedIndices.Count > 0) {
                PacketRecord record = records[Int32.Parse(listItems[listView_Packets.SelectedIndices[0]].SubItems[0].Text)];
                byte[] data = record.data;

                if (checkBox_useHighlighting.Checked) {
                    int fragStartPos = 20 + record.optionalHeadersLen;
                    int curFrag = 0;
                    int curLine = 0;
                    int dataConsumed = 0;
                    while (dataConsumed < data.Length) {
                        textBox_PacketData.SelectionColor = Color.Black;
                        textBox_PacketData.AppendText(string.Format("{0:X4}  ", curLine));

                        int lineFragStartPos = fragStartPos;
                        int linecurFrag = curFrag;

                        int hexIndex = 0;
                        for (; hexIndex < Math.Min(16, data.Length - dataConsumed); ++hexIndex) {
                            if (hexIndex == 8) {
                                textBox_PacketData.AppendText(" ");
                            }

                            int dataIndex = dataConsumed + hexIndex;

                            int selectedIndex = -1;
                            TreeNode selectedNode = treeView_ParsedData.SelectedNode;
                            if (selectedNode != null) {
                                while (selectedNode.Parent != null) {
                                    selectedNode = selectedNode.Parent;
                                }
                                selectedIndex = selectedNode.Index;
                            }

                            // Default color
                            textBox_PacketData.SelectionColor = Color.Red;
                            textBox_PacketData.SelectionBackColor = Color.White;

                            if (dataIndex < 20) {
                                // Protocol header
                                textBox_PacketData.SelectionColor = Color.Blue;
                            } else if (dataIndex < 20 + record.optionalHeadersLen) {
                                // Optional headers
                                textBox_PacketData.SelectionColor = Color.Green;
                            } else if (record.netPacket.fragList_.Count > 0) {
                                if (curFrag < record.netPacket.fragList_.Count) {
                                    int fragCurPos = dataIndex - fragStartPos;
                                    if (fragCurPos < 16) {
                                        // Fragment header
                                        textBox_PacketData.SelectionColor = Color.Magenta;
                                    } else if (fragCurPos == (16 + record.netPacket.fragList_[curFrag].dat_.Length)) {
                                        // Next fragment
                                        fragStartPos = dataIndex;
                                        curFrag++;
                                        textBox_PacketData.SelectionColor = Color.Magenta;
                                    } else {
                                        // Fragment data
                                        textBox_PacketData.SelectionColor = Color.Black;
                                    }

                                    if (selectedIndex == curFrag) {
                                        textBox_PacketData.SelectionBackColor = Color.LightGray;
                                    }
                                }
                            }

                            textBox_PacketData.AppendText(string.Format("{0:X2} ", data[dataIndex]));
                        }

                        textBox_PacketData.SelectionBackColor = Color.White;
                        StringBuilder spaceAligner = new StringBuilder();
                        spaceAligner.Append(' ', 1 + (16 - hexIndex) * 3 + (hexIndex <= 8 ? 1 : 0));
                        textBox_PacketData.AppendText(spaceAligner.ToString());

                        fragStartPos = lineFragStartPos;
                        curFrag = linecurFrag;

                        for (int i = 0; i < Math.Min(16, data.Length - dataConsumed); ++i) {
                            if (i == 8) {
                                textBox_PacketData.AppendText(" ");
                            }

                            int dataIndex = dataConsumed + i;

                            int selectedIndex = -1;
                            TreeNode selectedNode = treeView_ParsedData.SelectedNode;
                            if (selectedNode != null) {
                                while (selectedNode.Parent != null) {
                                    selectedNode = selectedNode.Parent;
                                }
                                selectedIndex = selectedNode.Index;
                            }

                            // Default color
                            textBox_PacketData.SelectionColor = Color.Red;
                            textBox_PacketData.SelectionBackColor = Color.White;

                            if (dataIndex < 20) {
                                // Protocol header
                                textBox_PacketData.SelectionColor = Color.Blue;
                            } else if (dataIndex < 20 + record.optionalHeadersLen) {
                                // Optional headers
                                textBox_PacketData.SelectionColor = Color.Green;
                            } else if (record.netPacket.fragList_.Count > 0) {
                                if (curFrag < record.netPacket.fragList_.Count) {
                                    int fragCurPos = dataIndex - fragStartPos;
                                    if (fragCurPos < 16) {
                                        // Fragment header
                                        textBox_PacketData.SelectionColor = Color.Magenta;
                                    } else if (fragCurPos == (16 + record.netPacket.fragList_[curFrag].dat_.Length)) {
                                        // Next fragment
                                        fragStartPos = dataIndex;
                                        curFrag++;
                                        textBox_PacketData.SelectionColor = Color.Magenta;
                                    } else {
                                        // Fragment data
                                        textBox_PacketData.SelectionColor = Color.Black;
                                    }

                                    if (selectedIndex == curFrag) {
                                        textBox_PacketData.SelectionBackColor = Color.LightGray;
                                    }
                                }
                            }

                            char asChar = Convert.ToChar(data[dataIndex]);
                            if (asChar >= ' ' && asChar <= '~') {
                                textBox_PacketData.AppendText(Char.ToString(asChar));
                            } else {
                                textBox_PacketData.AppendText(".");
                            }
                        }

                        textBox_PacketData.AppendText("\n");

                        dataConsumed += 16;
                        curLine++;
                    }
                } else {
                    StringBuilder strBuilder = new StringBuilder();
                    StringBuilder hexBuilder = new StringBuilder();
                    StringBuilder asciiBuilder = new StringBuilder();
                    int wrapCounter = 0;
                    for (int i = 0; i < data.Length; ++i) {
                        if (wrapCounter == 0) {
                            strBuilder.Append(string.Format("{0:X4}  ", i));
                        }

                        hexBuilder.Append(string.Format("{0:X2} ", data[i]));

                        char asChar = Convert.ToChar(data[i]);
                        if (asChar >= ' ' && asChar <= '~') {
                            asciiBuilder.Append(asChar);
                        } else {
                            asciiBuilder.Append('.');
                        }

                        wrapCounter++;
                        if (wrapCounter == 8) {
                            hexBuilder.Append(' ');
                            asciiBuilder.Append(' ');
                        } else if (wrapCounter == 16) {
                            strBuilder.Append(hexBuilder.ToString());
                            hexBuilder.Clear();

                            strBuilder.Append(' ');

                            strBuilder.Append(asciiBuilder.ToString());
                            asciiBuilder.Clear();

                            strBuilder.Append(Environment.NewLine);

                            wrapCounter = 0;
                        }
                    }

                    if (wrapCounter != 0) {
                        int spacesToAppend = (16 - wrapCounter) * 3;
                        if (wrapCounter < 8) {
                            spacesToAppend++;
                        }

                        hexBuilder.Append(' ', spacesToAppend);

                        strBuilder.Append(hexBuilder.ToString());

                        strBuilder.Append(' ');

                        strBuilder.Append(asciiBuilder.ToString());
                    }

                    textBox_PacketData.Text = strBuilder.ToString();
                }
            }
        }

        private void updateTree() {
            treeView_ParsedData.Nodes.Clear();

            if (listView_Packets.SelectedIndices.Count > 0) {
                PacketRecord record = records[Int32.Parse(listItems[listView_Packets.SelectedIndices[0]].SubItems[0].Text)];

                foreach (BlobFrag frag in record.netPacket.fragList_) {
                    BinaryReader fragDataReader = new BinaryReader(new MemoryStream(frag.dat_));
                    try {
                        bool handled = false;
                        foreach (MessageProcessor messageProcessor in messageProcessors) {
                            long readerStartPos = fragDataReader.BaseStream.Position;

                            bool accepted = messageProcessor.acceptMessageData(fragDataReader, treeView_ParsedData);

                            if (accepted && handled) {
                                throw new Exception("Multiple message processors are handling the same data!");
                            }

                            if (accepted) {
                                handled = true;
                                if (fragDataReader.BaseStream.Position != fragDataReader.BaseStream.Length) {
                                    treeView_ParsedData.Nodes.Add(new TreeNode("WARNING: Prev fragment not fully read!"));
                                }
                            }

                            fragDataReader.BaseStream.Position = readerStartPos;
                        }

                        if (!handled) {
                            PacketOpcode opcode = Util.readOpcode(fragDataReader);
                            treeView_ParsedData.Nodes.Add(new TreeNode("Unhandled: " + opcode));
                        }
                    } catch (Exception e) {
                        treeView_ParsedData.Nodes.Add(new TreeNode("EXCEPTION: " + e.Message));
                    }
                }
            }
        }

        private void listView_Packets_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateData();
        }

        private void listView_Packets_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex < listItems.Count) {
                e.Item = listItems[e.ItemIndex];

                // Apply highlights here
                if (opCodesToHighlight.Count > 0)
                {
                    var record = records[Int32.Parse(e.Item.SubItems[0].Text)];

                    for (int i = 0 ; i < opCodesToHighlight.Count ; i++)
                    {
                        if (record.opcodes.Contains((PacketOpcode)opCodesToHighlight[i]))
                        {
                            if (i == 0) e.Item.BackColor = Color.LightBlue;
                            else if (i == 1) e.Item.BackColor = Color.LightPink;
                            else if (i == 2) e.Item.BackColor = Color.LightGreen;
                            else if (i == 3) e.Item.BackColor = Color.GreenYellow;
                            else e.Item.BackColor = Color.MediumVioletRed;
                            break;
                        }
                    }
                }
            }
        }


        private void treeView_ParsedData_AfterSelect(object sender, TreeViewEventArgs e)
        {
            updateText();
        }


        private void menuItem_Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.AddExtension = true;
            openFile.Filter = "Packet Captures (*.pcap;*.pcapng)|*.pcap;*.pcapng|All Files (*.*)|*.*";

            if (openFile.ShowDialog() != DialogResult.OK)
                return;

            loadPcap(openFile.FileName);
        }

        private void mnuItem_EditPreviousHighlightedRow_Click(object sender, EventArgs e)
        {
            if (listView_Packets.TopItem == null)
                return;

            for (int i = listView_Packets.TopItem.Index - 1; i >= 0; i--)
            {
                if (listView_Packets.Items[i].BackColor != SystemColors.Window)
                {
                    listView_Packets.TopItem = listView_Packets.Items[i];
                    listView_Packets.TopItem.Selected = true;
                    listView_Packets.TopItem.Focused = true;
                    break;
                }
            }
        }

        private void mnuItem_EditNextHighlightedRow_Click(object sender, EventArgs e)
        {
            if (listView_Packets.TopItem == null)
                return;

            for (int i = listView_Packets.TopItem.Index + 1; i < listView_Packets.Items.Count; i++)
            {
                if (listView_Packets.Items[i].BackColor != SystemColors.Window)
                {
                    listView_Packets.TopItem = listView_Packets.Items[i];
                    listView_Packets.TopItem.Selected = true;
                    listView_Packets.TopItem.Focused = true;
                    break;
                }
            }
        }


        private void menuItem_ToolCount_Click(object sender, EventArgs e) {
            FolderBrowserDialog openFolder = new FolderBrowserDialog();

            if (openFolder.ShowDialog() != DialogResult.OK)
                return;

            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(openFolder.SelectedPath, "*.pcap", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(openFolder.SelectedPath, "*.pcapng", SearchOption.AllDirectories));

            OrderedDictionary opcodeOccurrences = new OrderedDictionary();

            foreach (PacketOpcode opcode in Enum.GetValues(typeof(PacketOpcode)))
                opcodeOccurrences[opcode] = 0;

            foreach (string file in files)
            {
                loadPcap(file, true);

                foreach (PacketRecord record in records)
                {
                    foreach (PacketOpcode opcode in record.opcodes)
                    {
                        if (opcodeOccurrences.Contains(opcode))
                            opcodeOccurrences[opcode] = (Int32)opcodeOccurrences[opcode] + 1;
                        else
                            opcodeOccurrences[opcode] = 1;
                    }
                }
            }

            long totalCount = 0;
            StringBuilder occurencesString = new StringBuilder();
            foreach (DictionaryEntry entry in opcodeOccurrences)
            {
                occurencesString.Append(entry.Key);
                occurencesString.Append(" = ");
                occurencesString.Append(entry.Value);
                occurencesString.Append("\r\n");

                totalCount += (Int32)entry.Value;
            }

            occurencesString.Append("\r\n\r\nTotal Count = ");
            occurencesString.Append(totalCount);
            occurencesString.Append("\r\n");

            TextPopup popup = new TextPopup();
            popup.setText(occurencesString.ToString());
            popup.setText(occurencesString.ToString() + "\r\n\r\n" + String.Join("\r\n", files));
            popup.ShowDialog();
        }

        private void menuItem_ToolBad_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFolder = new FolderBrowserDialog();

            if (openFolder.ShowDialog() != DialogResult.OK)
                return;

            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(openFolder.SelectedPath, "*.pcap", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(openFolder.SelectedPath, "*.pcapng", SearchOption.AllDirectories));

            OrderedDictionary opcodeOccurrences = new OrderedDictionary();

            foreach (PacketOpcode opcode in Enum.GetValues(typeof(PacketOpcode)))
                opcodeOccurrences[opcode] = 0;

            foreach (string file in files)
            {
                loadPcap(file);

                int curPacket = 0;
                int curFragment = 0;
                try
                {
                    for (curPacket = 0; curPacket < records.Count; ++curPacket)
                    {
                        PacketRecord record = records[curPacket];
                        for (curFragment = 0; curFragment < record.netPacket.fragList_.Count; ++curFragment)
                        {
                            BlobFrag frag = record.netPacket.fragList_[curFragment];
                            if (frag.memberHeader_.numFrags > 0)
                                continue;

                            BinaryReader fragDataReader = new BinaryReader(new MemoryStream(frag.dat_));

                            bool handled = false;
                            foreach (MessageProcessor messageProcessor in messageProcessors)
                            {
                                long readerStartPos = fragDataReader.BaseStream.Position;

                                bool accepted = messageProcessor.acceptMessageData(fragDataReader, treeView_ParsedData);

                                if (accepted && handled)
                                    throw new Exception("Multiple message processors are handling the same data!");

                                if (accepted)
                                    handled = true;

                                fragDataReader.BaseStream.Position = readerStartPos;
                            }

                            /*if (!handled) {
                                PacketOpcode opcode = Util.readOpcode(fragDataReader);
                                treeView_ParsedData.Nodes.Add(new TreeNode("Unhandled: " + opcode));
                            }*/

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Packet " + curPacket + " Fragment " + curFragment + " EXCEPTION: " + ex.Message);
                    break;
                }
            }
        }

        private void menuItem_ToolHeatmap_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFolder = new FolderBrowserDialog();

            if (openFolder.ShowDialog() != DialogResult.OK)
                return;

            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(openFolder.SelectedPath, "*.pcap", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(openFolder.SelectedPath, "*.pcapng", SearchOption.AllDirectories));

            uint packetCount = 0;
            uint messageCount = 0;
            uint[,] heatmap = new uint[256, 256];
            foreach (string file in files)
            {
                loadPcap(file, true);

                foreach (PacketRecord record in records)
                {
                    packetCount++;
                    foreach (BlobFrag frag in record.netPacket.fragList_)
                    {
                        if (frag.memberHeader_.blobNum == 0)
                            messageCount++;

                        if (frag.dat_.Length > 20)
                        {
                            BinaryReader fragDataReader = new BinaryReader(new MemoryStream(frag.dat_));
                            fragDataReader.ReadUInt32();
                            fragDataReader.ReadUInt32();
                            if ((PacketOpcode)fragDataReader.ReadUInt32() == PacketOpcode.Evt_Movement__AutonomousPosition_ID)
                            {
                                uint objcell_id = fragDataReader.ReadUInt32();
                                uint x = (objcell_id >> 24) & 0xFF;
                                uint y = 255 - ((objcell_id >> 16) & 0xFF);
                                heatmap[x, y] = 1;
                            }
                        }
                    }
                }
            }

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream imageStream = assembly.GetManifestResourceStream("aclogview.map.png");
            Bitmap heatmapImg = new Bitmap(imageStream);
            for (int y = 0; y < 256; ++y)
            {
                for (int x = 0; x < 256; ++x)
                {
                    if (heatmap[x, y] > 0)
                    {
                        Color curColor = heatmapImg.GetPixel(x, y);
                        heatmapImg.SetPixel(x, y, Color.FromArgb(255, Math.Min(255, 200 + curColor.R), curColor.G, curColor.B));
                    }
                }
            }

            ImagePopup popup = new ImagePopup();
            popup.Text = "Coverage Map - " + packetCount + " packets, " + messageCount + " messages";
            popup.ClientSize = new Size(512, 512);
            popup.setImage(heatmapImg);
            popup.ShowDialog();
        }

        private void mnuItem_ToolFindOpcodeInFiles_Click(object sender, EventArgs e)
        {
            var form = new FindOpcodeInFilesForm();
            form.Show(this);
        }

        private void mnuItem_ToolFragDatListTool_Click(object sender, EventArgs e)
        {
            var form = new FragDatListToolForm();
            form.Show(this);
        }

        private void menuItem_About_Click(object sender, EventArgs e)
        {
            MessageBox.Show("aclogview\n\nA program to view and parse Asheron's Call 1 packet capture files (.pcap) generated by aclog.\n\nFor more info and source code, see https://github.com/tfarley/aclogview", "About");
        }


        private void checkBox_HideHeaderOnly_CheckedChanged(object sender, EventArgs e)
        {
            listView_Packets.RedrawItems(0, records.Count - 1, false);
            updateData();
        }

        private void checkBox_useHighlighting_CheckedChanged(object sender, EventArgs e)
        {
            updateText();
        }

        private void parsedContextMenu_Opening(object sender, CancelEventArgs e)
        {

        }


        private void parsedContextMenu_Click(object sender, EventArgs e)
        {

            if (treeView_ParsedData.Nodes.Count > 0)
            {
                strbuilder.Clear();
                foreach (var node in GetTreeNodes(treeView_ParsedData.Nodes))
                {
                    strbuilder.AppendLine(node.Text);
                }
                Clipboard.SetText(strbuilder.ToString());
            }
        }

        IEnumerable<TreeNode> GetTreeNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                yield return node;
                foreach (var child in GetTreeNodes(node.Nodes))
                    yield return child;
            }
        }

    }
}

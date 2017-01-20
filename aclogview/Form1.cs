using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aclogview {
    public partial class Form1 : Form {
        private ListViewItemComparer comparer = new ListViewItemComparer();
        public List<MessageProcessor> messageProcessors = new List<MessageProcessor>();

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            Util.initReaders();
            messageProcessors.Add(new CM_Admin());
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
        }

        private void menuItem5_Click(object sender, EventArgs e) {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.AddExtension = true;
            openFile.Filter = "Packet Captures (*.pcap)|*.pcap|All Files (*.*)|*.*";

            if (openFile.ShowDialog() != DialogResult.OK) {
                return;
            }

            loadPcap(openFile.FileName);
        }

        class PacketRecord {
            public int index;
            public bool isSend;
            public uint tsSec;
            public string packetHeadersStr;
            public string packetTypeStr;
            public byte[] data;
            public NetPacket netPacket;
            public PacketOpcode opcode;
            public string extraInfo;
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
                packetTypeStr.Append(Util.readOpcode(fragDataReader).ToString());
            }
        }

        private void readOptionalHeaders(uint header_, StringBuilder packetHeadersStr, BinaryReader packetReader) {
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
        }

        List<PacketRecord> records = new List<PacketRecord>();
        List<ListViewItem> listItems = new List<ListViewItem>();

        private void loadPcap(string fileName) {
            this.Text = "AC Log View - " + Path.GetFileName(fileName);

            records.Clear();
            listItems.Clear();

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                using (BinaryReader binaryReader = new BinaryReader(fileStream)) {
                    PcapHeader pcapHeader = PcapHeader.read(binaryReader);
                    while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length) {
                        PcapRecordHeader recordHeader = PcapRecordHeader.read(binaryReader);

                        long packetStartPos = binaryReader.BaseStream.Position;
                        EthernetHeader ethernetHeader = EthernetHeader.read(binaryReader);
                        IpHeader ipHeader = IpHeader.read(binaryReader);
                        UdpHeader udpHeader = UdpHeader.read(binaryReader);
                        long headersSize = binaryReader.BaseStream.Position - packetStartPos;

                        StringBuilder packetHeadersStr = new StringBuilder();
                        StringBuilder packetTypeStr = new StringBuilder();

                        PacketRecord packet = new PacketRecord();
                        packet.index = records.Count;
                        packet.isSend = (udpHeader.sPort == 12345);
                        packet.tsSec = recordHeader.tsSec;
                        packet.netPacket = new NetPacket();
                        packet.data = binaryReader.ReadBytes((int)(recordHeader.inclLen - headersSize));
                        packet.extraInfo = "";
                        BinaryReader packetReader = new BinaryReader(new MemoryStream(packet.data));
                        try {
                            ProtoHeader pHeader = ProtoHeader.read(packetReader);

                            readOptionalHeaders(pHeader.header_, packetHeadersStr, packetReader);

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
                        } catch (Exception e) {
                            packet.extraInfo += "EXCEPTION: " + e.Message + " " + e.StackTrace;
                        }
                        packet.packetHeadersStr = packetHeadersStr.ToString();
                        packet.packetTypeStr = packetTypeStr.ToString();

                        records.Add(packet);

                        ListViewItem newItem = new ListViewItem(packet.index.ToString());
                        newItem.SubItems.Add(packet.isSend ? "Send" : "Recv");
                        newItem.SubItems.Add(packet.tsSec.ToString());
                        newItem.SubItems.Add(packet.packetHeadersStr);
                        newItem.SubItems.Add(packet.packetTypeStr);
                        newItem.SubItems.Add(packet.data.Length.ToString());
                        newItem.SubItems.Add(packet.extraInfo);
                        listItems.Add(newItem);
                    }
                }
            }

            listView_Packets.VirtualListSize = records.Count;

            listView_Packets.RedrawItems(0, records.Count - 1, false);
            updateData();
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
            if (listView_Packets.SelectedIndices.Count > 0) {
                PacketRecord record = records[Int32.Parse(listItems[listView_Packets.SelectedIndices[0]].SubItems[0].Text)];
                byte[] data = record.data;

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
            } else {
                textBox_PacketData.Text = "";
                treeView_ParsedData.Nodes.Clear();
            }
        }

        private void listView_Packets_SelectedIndexChanged(object sender, EventArgs e) {
            updateData();
        }

        private void checkBox_HideHeaderOnly_CheckedChanged(object sender, EventArgs e) {
            listView_Packets.RedrawItems(0, records.Count - 1, false);
            updateData();
        }

        private void listView_Packets_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
            e.Item = listItems[e.ItemIndex];
        }

        private void menuItem_About_Click(object sender, EventArgs e) {
            MessageBox.Show("aclogview\n\nA program to view and parse Asheron's Call 1 packet capture files (.pcap) generated by aclog.\n\nFor more info and source code, see https://github.com/tfarley/aclogview", "About");
        }
    }
}

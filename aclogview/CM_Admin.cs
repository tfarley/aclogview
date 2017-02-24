using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class CM_Admin : MessageProcessor {

    public override bool acceptMessageData(BinaryReader messageDataReader, TreeView outputTreeView) {
        bool handled = true;

        PacketOpcode opcode = Util.readOpcode(messageDataReader);
        switch (opcode) {
            case PacketOpcode.Evt_Admin__ChatServerData_ID: // 0xF7DE
                {
                    var message = ChatServerData.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            default: {
                    handled = false;
                    break;
                }
        }

        return handled;
    }

    public class ChatServerData : Message
    {
        public uint Size;
        public uint TurbineChatType;
        public uint Unknown_1;
        public uint Unknown_2;
        public uint Unknown_3;
        public uint Unknown_4;
        public uint Unknown_5;
        public uint Unknown_6;
        public uint PayloadSize;

        // 0x01
        public uint Channel;
        public string SenderName;
        public string Message;
        public uint Unknown01_1;
        public uint Sender;
        public uint Unknown01_2;
        public uint Unknown01_3;

        // 0x03
        public uint Unknown03_1;
        public uint Unknown03_2;
        public uint Unknown03_3;
        public uint OutChannel;
        public string OutText;
        public uint Unknown03_4;
        public uint OutSender;
        public uint Unknown03_5;
        public uint Unknown03_6;

        // 0x05
        public uint Unknown05_1;
        public uint Unknown05_2;
        public uint Unknown05_3;
        public uint Unknown05_4;

        public static ChatServerData read(BinaryReader binaryReader)
        {
            var newObj = new ChatServerData();
            newObj.Size = binaryReader.ReadUInt32();
            newObj.TurbineChatType = binaryReader.ReadUInt32();
            newObj.Unknown_1 = binaryReader.ReadUInt32();
            newObj.Unknown_2 = binaryReader.ReadUInt32();
            newObj.Unknown_3 = binaryReader.ReadUInt32();
            newObj.Unknown_4 = binaryReader.ReadUInt32();
            newObj.Unknown_5 = binaryReader.ReadUInt32();
            newObj.Unknown_6 = binaryReader.ReadUInt32();
            newObj.PayloadSize = binaryReader.ReadUInt32();

            if (newObj.TurbineChatType == 0x01)
            {
                newObj.Channel = binaryReader.ReadUInt32();

                var messageLen = binaryReader.ReadByte();
                var messageBytes = binaryReader.ReadBytes(messageLen * 2);
                newObj.SenderName = Encoding.Unicode.GetString(messageBytes);

                messageLen = binaryReader.ReadByte();
                messageBytes = binaryReader.ReadBytes(messageLen * 2);
                newObj.Message = Encoding.Unicode.GetString(messageBytes);

                newObj.Unknown01_1 = binaryReader.ReadUInt32();
                newObj.Sender = binaryReader.ReadUInt32();
                newObj.Unknown01_2 = binaryReader.ReadUInt32();
                newObj.Unknown01_3 = binaryReader.ReadUInt32();
            }
            else if (newObj.TurbineChatType == 0x03)
            {
                newObj.Unknown03_1 = binaryReader.ReadUInt32();
                newObj.Unknown03_2 = binaryReader.ReadUInt32();
                newObj.Unknown03_3 = binaryReader.ReadUInt32();
                newObj.OutChannel = binaryReader.ReadUInt32();

                var messageLen = binaryReader.ReadByte();
                var messageBytes = binaryReader.ReadBytes(messageLen * 2);
                newObj.OutText = Encoding.Unicode.GetString(messageBytes);

                newObj.Unknown03_4 = binaryReader.ReadUInt32();
                newObj.OutSender = binaryReader.ReadUInt32();
                newObj.Unknown03_5 = binaryReader.ReadUInt32();
                newObj.Unknown03_6 = binaryReader.ReadUInt32();

            }
            else if (newObj.TurbineChatType == 0x05)
            {
                newObj.Unknown05_1 = binaryReader.ReadUInt32();
                newObj.Unknown05_2 = binaryReader.ReadUInt32();
                newObj.Unknown05_3 = binaryReader.ReadUInt32();
                newObj.Unknown05_4 = binaryReader.ReadUInt32();
            }

            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("Size = " + Size);
            rootNode.Nodes.Add("TurbineChatType = " + TurbineChatType);
            rootNode.Nodes.Add("Unknown_1 = " + Unknown_1);
            rootNode.Nodes.Add("Unknown_2 = " + Unknown_2);
            rootNode.Nodes.Add("Unknown_3 = " + Unknown_3);
            rootNode.Nodes.Add("Unknown_4 = " + Unknown_4);
            rootNode.Nodes.Add("Unknown_5 = " + Unknown_5);
            rootNode.Nodes.Add("Unknown_6 = " + Unknown_6);
            rootNode.Nodes.Add("PayloadSize = " + PayloadSize);

            if (TurbineChatType == 0x01)
            {
                rootNode.Nodes.Add("Channel = " + Channel);
                rootNode.Nodes.Add("SenderName = " + SenderName);
                rootNode.Nodes.Add("Message = " + Message);
                rootNode.Nodes.Add("Unknown01_1 = " + Unknown01_1);
                rootNode.Nodes.Add("Sender = " + Sender);
                rootNode.Nodes.Add("Unknown01_2 = " + Unknown01_2);
                rootNode.Nodes.Add("Unknown01_3 = " + Unknown01_3);
            }
            else if (TurbineChatType == 0x03)
            {
                rootNode.Nodes.Add("Unknown03_1 = " + Unknown03_1);
                rootNode.Nodes.Add("Unknown03_2 = " + Unknown03_2);
                rootNode.Nodes.Add("Unknown03_3 = " + Unknown03_3);
                rootNode.Nodes.Add("OutChannel = " + OutChannel);
                rootNode.Nodes.Add("OutText = " + OutText);
                rootNode.Nodes.Add("Unknown03_4 = " + Unknown03_4);
                rootNode.Nodes.Add("OutSender = " + OutSender);
                rootNode.Nodes.Add("Unknown03_5 = " + Unknown03_5);
                rootNode.Nodes.Add("Unknown03_6 = " + Unknown03_6);
            }
            else if (TurbineChatType == 0x05)
            {
                rootNode.Nodes.Add("Unknown05_1 = " + Unknown05_1);
                rootNode.Nodes.Add("Unknown05_2 = " + Unknown05_2);
                rootNode.Nodes.Add("Unknown05_3 = " + Unknown05_3);
                rootNode.Nodes.Add("Unknown05_4 = " + Unknown05_4);
            }

            treeView.Nodes.Add(rootNode);
        }
    }
}

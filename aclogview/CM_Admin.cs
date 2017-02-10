using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class CM_Admin : MessageProcessor
{ 

    public override bool acceptMessageData(BinaryReader messageDataReader, TreeView outputTreeView)
    {
        bool handled = true;

        PacketOpcode opcode = Util.readOpcode(messageDataReader);
        switch (opcode)
        {
            case PacketOpcode.Evt_Admin__ChatServerData_ID:
                {
                    TurbineChat message = TurbineChat.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            default:
                {
                    handled = false;
                    break;
                }
        }

        return handled;
    }

    public class TurbineChat : Message
    {
        public uint size;
        public uint TurbineChatType;
        public uint Unk1;
        public uint Unk2;
        public uint Unk3;
        public uint Unk4;
        public uint Unk5;
        public uint Unk6;
        public uint payload_size;

        //0x1
        public uint channel;
        public byte sendernameLen;
        public string senderName;
        public byte msgLen;
        public string msgText;
        public UInt32 unk1_1;
        public UInt32 unk1_2;
        public UInt32 unk1_3;
        public UInt32 unk1_4;
        //

        //0x3 -- outbound
        public UInt32 unk3_1;
        public UInt32 unk3_2;
        public UInt32 unk3_3;
        public UInt32 out_channel;
        public byte ob_msgLen;
        public string ob_msgText;
        public UInt32 unk3_4;
        public UInt32 sender_id;
        public UInt32 unk3_5;
        public UInt32 unk3_6;
        //0x5
        public UInt32 unk5_1;
        public UInt32 unk5_2;
        public UInt32 unk5_3;
        public UInt32 unk5_4;

        public static TurbineChat read(BinaryReader binaryReader)
        {
            TurbineChat newObj = new TurbineChat();
            newObj.size = binaryReader.ReadUInt32();
            newObj.TurbineChatType = binaryReader.ReadUInt32();
            newObj.Unk1 = binaryReader.ReadUInt32();
            newObj.Unk2 = binaryReader.ReadUInt32();
            newObj.Unk3 = binaryReader.ReadUInt32();
            newObj.Unk4 = binaryReader.ReadUInt32();
            newObj.Unk5 = binaryReader.ReadUInt32();
            newObj.Unk6 = binaryReader.ReadUInt32();
            newObj.payload_size = binaryReader.ReadUInt32();

            if (newObj.TurbineChatType == 0x01) // inbound
            {
                newObj.channel = binaryReader.ReadUInt32();
                newObj.sendernameLen = binaryReader.ReadByte();
                newObj.senderName = System.Text.Encoding.Unicode.GetString(binaryReader.ReadBytes((newObj.sendernameLen * 2)));

                newObj.msgLen = binaryReader.ReadByte();
                newObj.msgText = System.Text.Encoding.Unicode.GetString(binaryReader.ReadBytes((newObj.msgLen * 2)));

                newObj.unk1_1 = binaryReader.ReadUInt32();
                newObj.unk1_2 = binaryReader.ReadUInt32();
                newObj.unk1_3 = binaryReader.ReadUInt32();
                newObj.unk1_4 = binaryReader.ReadUInt32();
                //newObj.senderName = binaryReader.ReadString();
            }
            else if (newObj.TurbineChatType == 0x3) // outbound
            {
                newObj.unk3_1 = binaryReader.ReadUInt32();
                newObj.unk3_2 = binaryReader.ReadUInt32();
                newObj.unk3_3 = binaryReader.ReadUInt32();
                newObj.out_channel = binaryReader.ReadUInt32();

                newObj.ob_msgLen = binaryReader.ReadByte();
                newObj.ob_msgText = System.Text.Encoding.Unicode.GetString(binaryReader.ReadBytes((newObj.ob_msgLen * 2)));

                newObj.unk3_4 = binaryReader.ReadUInt32();
                newObj.sender_id = binaryReader.ReadUInt32();
                newObj.unk3_5 = binaryReader.ReadUInt32();
                newObj.unk3_6 = binaryReader.ReadUInt32();
            }

            else if (newObj.TurbineChatType == 0x5) // ack
            {
                newObj.unk5_1 = binaryReader.ReadUInt32();
                newObj.unk5_2 = binaryReader.ReadUInt32();
                newObj.unk5_3 = binaryReader.ReadUInt32();
                newObj.unk5_4 = binaryReader.ReadUInt32();
            }

                return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            foreach (var prop in this.GetType().GetProperties())
            {
                rootNode.Nodes.Add("prop.Name = " + prop.GetValue(this).ToString());
            }
            rootNode.Nodes.Add("size = " + size.ToString());
            rootNode.Nodes.Add("TurbineChatType = " + TurbineChatType.ToString("X"));
            rootNode.Nodes.Add("Unk1 = " + Unk1.ToString("X"));
            rootNode.Nodes.Add("Unk2 = " + Unk2.ToString("X"));
            rootNode.Nodes.Add("Unk3 = " + Unk3.ToString("X"));
            rootNode.Nodes.Add("Unk4 = " + Unk4.ToString("X"));
            rootNode.Nodes.Add("Unk5 = " + Unk5.ToString("X"));
            rootNode.Nodes.Add("Unk6 = " + Unk6.ToString("X"));

            if (TurbineChatType == 0x1)
            {
                TreeNode MsgType = rootNode.Nodes.Add("Inbound Msg (0x1)");
                MsgType.Nodes.Add("channel = " + channel.ToString("X"));
                MsgType.Nodes.Add("senderNameLen = " + sendernameLen.ToString());
                MsgType.Nodes.Add("SenderName = " + senderName.ToString());

                MsgType.Nodes.Add("msgLen = " + msgLen.ToString());
                MsgType.Nodes.Add("msgText = " + msgText.ToString());

                MsgType.Nodes.Add("unk1 = " + unk1_1.ToString());
                MsgType.Nodes.Add("unk2 = " + unk1_2.ToString());
                MsgType.Nodes.Add("unk3 = " + unk1_3.ToString());
                MsgType.Nodes.Add("unk4 = " + unk1_4.ToString());

            }
            else if(TurbineChatType == 0x3)
            {
                TreeNode MsgType = rootNode.Nodes.Add("Outbound Msg (0x3)");
                MsgType.Nodes.Add("unk1 = " + unk3_1.ToString());
                MsgType.Nodes.Add("unk2 = " + unk3_2.ToString());
                MsgType.Nodes.Add("unk3 = " + unk3_3.ToString());
                MsgType.Nodes.Add("out_channel = " + out_channel.ToString());

                MsgType.Nodes.Add("OutboundMsgLen = " + ob_msgLen.ToString());
                MsgType.Nodes.Add("OutBoundMsgTxt = " + ob_msgText.ToString());

                MsgType.Nodes.Add("unk4 = " + unk3_4.ToString());
                MsgType.Nodes.Add("sender_id = " + sender_id.ToString());
                MsgType.Nodes.Add("unk5 = " + unk3_5.ToString());
                MsgType.Nodes.Add("unk6 = " + unk3_6.ToString());
            }
            if(TurbineChatType == 0x5)
            {
                TreeNode MsgType = rootNode.Nodes.Add("Inbound Ack of Outbound Msg (0x5)");
                MsgType.Nodes.Add("unk1 = " + unk1_1.ToString());
                MsgType.Nodes.Add("unk2 = " + unk1_2.ToString());
                MsgType.Nodes.Add("unk3 = " + unk1_3.ToString());
                MsgType.Nodes.Add("unk4 = " + unk1_4.ToString());
            }
          
            treeView.Nodes.Add(rootNode);
            treeView.ExpandAll();
        }
    }
}

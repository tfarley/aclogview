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
            rootNode.Nodes.Add("size = " + size.ToString("X"));
            rootNode.Nodes.Add("TurbineChatType = " + TurbineChatType.ToString("X"));
            rootNode.Nodes.Add("Unk1 = " + Unk1.ToString("X"));
            rootNode.Nodes.Add("Unk2 = " + Unk2.ToString("X"));
            rootNode.Nodes.Add("Unk3 = " + Unk3.ToString("X"));
            rootNode.Nodes.Add("Unk4 = " + Unk4.ToString("X"));
            rootNode.Nodes.Add("Unk5 = " + Unk5.ToString("X"));
            rootNode.Nodes.Add("Unk6 = " + Unk6.ToString("X"));
            treeView.Nodes.Add(rootNode);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class CM_Communication : MessageProcessor {

    public override bool acceptMessageData(BinaryReader messageDataReader, TreeView outputTreeView) {
        bool handled = true;

        PacketOpcode opcode = Util.readOpcode(messageDataReader);
        switch (opcode) {
            case PacketOpcode.Evt_Communication__Recv_ChatRoomTracker_ID:
                {
                    ChatRoomTracker message = ChatRoomTracker.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
           
            case PacketOpcode.Evt_Communication__WeenieError_ID: {
                    WeenieError message = WeenieError.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__WeenieErrorWithString_ID: {
                    WeenieErrorWithString message = WeenieErrorWithString.read(messageDataReader);
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

    public class WeenieError : Message {
        public WERROR etype;

        public static WeenieError read(BinaryReader binaryReader) {
            WeenieError newObj = new WeenieError();
            newObj.etype = (WERROR)binaryReader.ReadUInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("etype = " + etype);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class WeenieErrorWithString : Message {
        public WERROR etype;
        public PStringChar user_data;

        public static WeenieErrorWithString read(BinaryReader binaryReader) {
            WeenieErrorWithString newObj = new WeenieErrorWithString();
            newObj.etype = (WERROR)binaryReader.ReadUInt32();
            newObj.user_data = PStringChar.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("etype = " + etype);
            rootNode.Nodes.Add("user_data = " + user_data);
            treeView.Nodes.Add(rootNode);
        }
    }
}

public class ChatRoomTracker : Message
{
    public uint Allegiance;
    public uint General;
    public uint Trade;
    public uint LFG;
    public uint Roleplay;
    public uint Olthoi;
    public uint Unk1;
    public uint Unk2;
    public uint Unk3;

    public static ChatRoomTracker read(BinaryReader binaryReader)
    {
        ChatRoomTracker newObj = new ChatRoomTracker();
        newObj.Allegiance = binaryReader.ReadUInt32();
        newObj.General = binaryReader.ReadUInt32();
        newObj.Trade = binaryReader.ReadUInt32();
        newObj.LFG = binaryReader.ReadUInt32();
        newObj.Roleplay = binaryReader.ReadUInt32();
        newObj.Olthoi = binaryReader.ReadUInt32();
        newObj.Unk1 = binaryReader.ReadUInt32();
        newObj.Unk2 = binaryReader.ReadUInt32();
        newObj.Unk3 = binaryReader.ReadUInt32();

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
            rootNode.Nodes.Add("AllegianceChannelID = " + Allegiance.ToString("X"));
        rootNode.Nodes.Add("General = " + General.ToString("X"));
        rootNode.Nodes.Add("Trade = " + Trade.ToString("X"));
        rootNode.Nodes.Add("LFG = " + LFG.ToString("X"));
        rootNode.Nodes.Add("Roleplay = " + Roleplay.ToString("X"));
        rootNode.Nodes.Add("Olthoi = " + Olthoi.ToString("X"));
        rootNode.Nodes.Add("Unk1 = " + Unk1.ToString("X"));
        rootNode.Nodes.Add("Unk2 = " + Unk2.ToString("X"));
        rootNode.Nodes.Add("Unk3 = " + Unk3.ToString("X"));

        treeView.Nodes.Add(rootNode);
    }
}

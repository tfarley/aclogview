using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using aclogview;

public class CM_Communication : MessageProcessor {

    public override bool acceptMessageData(BinaryReader messageDataReader, TreeView outputTreeView) {
        bool handled = true;

        PacketOpcode opcode = Util.readOpcode(messageDataReader);
        switch (opcode) {
            case PacketOpcode.Evt_Communication__Talk_ID: // 0x0015
                {
                    var message = Talk.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__TalkDirect_ID: // 0x0032
                {
                    var message = TalkDirect.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__TalkDirectByName_ID: // 0x005D
                {
                    var message = TalkDirectByName.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__ChannelBroadcast_ID: // 0x0147
                {
                    var message = ChannelBroadcast.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            /*case PacketOpcode.Evt_Communication__SetSquelchDB_ID: // 0x01F4
                {
                    var message = SetSquelchDB.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }*/
            case PacketOpcode.Evt_Communication__Emote_ID: // 0x01DF
                {
                    var message = Emote.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__HearEmote_ID: // 0x01E0
                {
                    var message = HearEmote.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__SoulEmote_ID: // 0x01E1
                {
                    var message = SoulEmote.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__HearSoulEmote_ID: // 0x01E2
                {
                    var message = HearSoulEmote.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__Recv_ChatRoomTracker_ID: // 0x0295
                {
                    var message = Recv_ChatRoomTracker.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__WeenieError_ID: // 0x028A
                {
                    WeenieError message = WeenieError.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__WeenieErrorWithString_ID: // 0x028B
                {
                    WeenieErrorWithString message = WeenieErrorWithString.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__HearSpeech_ID: // 0x02BB
                {
                    var message = HearSpeech.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__HearRangedSpeech_ID: // 0x02BC
                {
                    var message = HearRangedSpeech.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__HearDirectSpeech_ID: // 0x2BD
                {
                    var message = HearDirectSpeech.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Communication__TextboxString_ID: // 0xF7E0
                {
                    var message = TextBoxString.read(messageDataReader);
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

    public class Talk : Message
    {
        public PStringChar MessageText;

        public static Talk read(BinaryReader binaryReader)
        {
            var newObj = new Talk();
            newObj.MessageText = PStringChar.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("MessageText = " + MessageText.m_buffer);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class TalkDirect : Message
    {
        public PStringChar MessageText;
        public uint TargetID;

        public static TalkDirect read(BinaryReader binaryReader)
        {
            var newObj = new TalkDirect();
            newObj.MessageText = PStringChar.read(binaryReader);
            newObj.TargetID = binaryReader.ReadUInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("MessageText = " + MessageText.m_buffer);
            rootNode.Nodes.Add("TargetID = " + Utility.FormatGuid(TargetID));
            treeView.Nodes.Add(rootNode);
        }
    }

    public class TalkDirectByName : Message
    {
        public PStringChar MessageText;
        public PStringChar TargetName;

        public static TalkDirectByName read(BinaryReader binaryReader)
        {
            var newObj = new TalkDirectByName();
            newObj.MessageText = PStringChar.read(binaryReader);
            newObj.TargetName = PStringChar.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("MessageText = " + MessageText.m_buffer);
            rootNode.Nodes.Add("TargetName = " + TargetName.m_buffer);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class ChannelBroadcast : Message
    {
        public uint GroupChatType;
        public uint Unknown;
        public PStringChar MessageText;

        public static ChannelBroadcast read(BinaryReader binaryReader)
        {
            var newObj = new ChannelBroadcast();
            newObj.GroupChatType = binaryReader.ReadUInt32();
            newObj.Unknown = binaryReader.ReadUInt32();
            newObj.MessageText = PStringChar.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("GroupChatType = " + GroupChatType);
            rootNode.Nodes.Add("Unknown = " + Unknown);
            rootNode.Nodes.Add("MessageText = " + MessageText.m_buffer);
            treeView.Nodes.Add(rootNode);
        }
    }

    /*public class SetSquelchDB : Message
    {
        public static SetSquelchDB read(BinaryReader binaryReader)
        {
            var newObj = new SetSquelchDB();

            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            throw new NotImplementedException();
        }
    }*/

    public class Emote : Message
    {
        public PStringChar emoteMessage;

        public static Emote read(BinaryReader binaryReader)
        {
            var newObj = new Emote();
            newObj.emoteMessage = PStringChar.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("emoteMessage = " + emoteMessage);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class HearEmote : Message
    {
        public PStringChar EmoteMessage;
        public PStringChar SenderName;
        public uint SenderID;

        public static HearEmote read(BinaryReader binaryReader)
        {
            var newObj = new HearEmote();
            newObj.SenderID = binaryReader.ReadUInt32();
            newObj.SenderName = PStringChar.read(binaryReader);
            newObj.EmoteMessage = PStringChar.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("EmoteMessage = " + EmoteMessage.m_buffer);
            rootNode.Nodes.Add("SenderName = " + SenderName.m_buffer);
            rootNode.Nodes.Add("SenderID = " + Utility.FormatGuid(this.SenderID));
            treeView.Nodes.Add(rootNode);
        }
    }
    public class SoulEmote : Message
    {
        public PStringChar EmoteMessage;

        public static SoulEmote read(BinaryReader binaryReader)
        {
            SoulEmote newObj = new SoulEmote();
            newObj.EmoteMessage = PStringChar.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("EmoteMessage = " + EmoteMessage.m_buffer);
            treeView.Nodes.Add(rootNode);
        }
    }
    public class HearSoulEmote : Message
    {
        public PStringChar EmoteMessage;
        public PStringChar SenderName;
        public uint SenderID;

        public static HearSoulEmote read(BinaryReader binaryReader)
        {
            var newObj = new HearSoulEmote();
            newObj.SenderID = binaryReader.ReadUInt32();
            newObj.SenderName = PStringChar.read(binaryReader);
            newObj.EmoteMessage = PStringChar.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("EmoteMessage = " + EmoteMessage.m_buffer);
            rootNode.Nodes.Add("SenderName = " + SenderName.m_buffer);
            rootNode.Nodes.Add("SenderID = " + Utility.FormatGuid(this.SenderID));
            treeView.Nodes.Add(rootNode);
        }
    }

    public class Recv_ChatRoomTracker : Message
    {
        public uint AllegianceChannel;
        public uint GeneralChannel;
        public uint TradeChannel;
        public uint LFGChannel;
        public uint RoleplayChannel;
        public uint Olthoi;
        public uint Society;
        public uint SocietyCelHan;
        public uint SocietyEldWeb;
        public uint SocietyRadBlo;

		public static Recv_ChatRoomTracker read(BinaryReader binaryReader)
        {
            var newObj = new Recv_ChatRoomTracker();
            newObj.AllegianceChannel = binaryReader.ReadUInt32();
            newObj.GeneralChannel = binaryReader.ReadUInt32();
            newObj.TradeChannel = binaryReader.ReadUInt32();
            newObj.LFGChannel = binaryReader.ReadUInt32();
            newObj.RoleplayChannel = binaryReader.ReadUInt32();
            newObj.Olthoi = binaryReader.ReadUInt32();
            newObj.Society = binaryReader.ReadUInt32();
            newObj.SocietyCelHan = binaryReader.ReadUInt32();
            newObj.SocietyEldWeb = binaryReader.ReadUInt32();
            newObj.SocietyRadBlo = binaryReader.ReadUInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("AllegianceChannel = " + AllegianceChannel);
            rootNode.Nodes.Add("GeneralChannel = " + GeneralChannel);
            rootNode.Nodes.Add("TradeChannel = " + TradeChannel);
            rootNode.Nodes.Add("LFGChannel = " + LFGChannel);
            rootNode.Nodes.Add("RoleplayChannel = " + RoleplayChannel);
            rootNode.Nodes.Add("Olthoi = " + Olthoi);
            rootNode.Nodes.Add("Society = " + Society);
            rootNode.Nodes.Add("SocietyCelHan = " + SocietyCelHan);
            rootNode.Nodes.Add("SocietyEldWeb = " + SocietyEldWeb);
            rootNode.Nodes.Add("SocietyRadBlo = " + SocietyRadBlo);
            treeView.Nodes.Add(rootNode);
        }
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

    public class HearSpeech : Message
    {
        public PStringChar MessageText;
        public PStringChar SenderName;
        public uint SenderID;
        public uint ChatMessageType;

        public static HearSpeech read(BinaryReader binaryReader)
        {
            var newObj = new HearSpeech();
            newObj.MessageText = PStringChar.read(binaryReader);
            newObj.SenderName = PStringChar.read(binaryReader);
            newObj.SenderID = binaryReader.ReadUInt32();
            newObj.ChatMessageType = binaryReader.ReadUInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("MessageText = " + MessageText.m_buffer);
            rootNode.Nodes.Add("SenderName = " + SenderName.m_buffer);
            rootNode.Nodes.Add("SenderID = " + Utility.FormatGuid(this.SenderID));                        
            rootNode.Nodes.Add("ChatMessageType = " + ChatMessageType);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class HearRangedSpeech : Message
    {
        public PStringChar MessageText;
        public PStringChar SenderName;
        public uint SenderID;
        public float Range;
        public uint ChatMessageType;

        public static HearRangedSpeech read(BinaryReader binaryReader)
        {
            var newObj = new HearRangedSpeech();
            newObj.MessageText = PStringChar.read(binaryReader);
            newObj.SenderName = PStringChar.read(binaryReader);
            newObj.SenderID = binaryReader.ReadUInt32();
            newObj.Range = binaryReader.ReadSingle();
            newObj.ChatMessageType = binaryReader.ReadUInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("MessageText = " + MessageText.m_buffer);
            rootNode.Nodes.Add("SenderName = " + SenderName.m_buffer);
            rootNode.Nodes.Add("SenderID = " +Utility.FormatGuid(this.SenderID));
            rootNode.Nodes.Add("Range = " + Range);
            rootNode.Nodes.Add("ChatMessageType = " + ChatMessageType);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class HearDirectSpeech : Message
    {
        public PStringChar MessageText;
        public PStringChar SenderName;
        public uint SenderID;
        public uint TargetID;
        public uint ChatMessageType;
        public uint Unknown;

        public static HearDirectSpeech read(BinaryReader binaryReader)
        {
            var newObj = new HearDirectSpeech();
            newObj.MessageText = PStringChar.read(binaryReader);
            newObj.SenderName = PStringChar.read(binaryReader);
            newObj.SenderID = binaryReader.ReadUInt32();
            newObj.TargetID = binaryReader.ReadUInt32();
            newObj.ChatMessageType = binaryReader.ReadUInt32();
            newObj.Unknown = binaryReader.ReadUInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("MessageText = " + MessageText.m_buffer);
            rootNode.Nodes.Add("SenderName = " + SenderName.m_buffer);
            rootNode.Nodes.Add("SenderID = " + Utility.FormatGuid(this.SenderID));
            rootNode.Nodes.Add("TargetID = " + Utility.FormatGuid(this.TargetID));                    
            rootNode.Nodes.Add("ChatMessageType = " + ChatMessageType);
            rootNode.Nodes.Add("Unknown = " + Unknown);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class TextBoxString : Message
    {
        public PStringChar MessageText;
        public uint ChatMessageType;

        public static TextBoxString read(BinaryReader binaryReader)
        {
            var newObj = new TextBoxString();
            newObj.MessageText = PStringChar.read(binaryReader);
            newObj.ChatMessageType = binaryReader.ReadUInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("MessageText = " + MessageText.m_buffer);
            rootNode.Nodes.Add("ChatMessageType = " + ChatMessageType);
            treeView.Nodes.Add(rootNode);
        }
    }
}

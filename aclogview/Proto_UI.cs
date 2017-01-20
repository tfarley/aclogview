using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class Proto_UI : MessageProcessor {

    public override bool acceptMessageData(BinaryReader messageDataReader, TreeView outputTreeView) {
        bool handled = true;

        PacketOpcode opcode = Util.readOpcode(messageDataReader);
        switch (opcode) {
            case PacketOpcode.CHARACTER_EXIT_GAME_EVENT: {
                    LogOff message = LogOff.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.CHARACTER_ENTER_GAME_EVENT: {
                    EnterWorld message = EnterWorld.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.CONTROL_FORCE_OBJDESC_SEND_EVENT: {
                    ForceObjdesc message = ForceObjdesc.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.CLIENT_REQUEST_ENTER_GAME_EVENT: {
                    EnterWorldRequest message = EnterWorldRequest.read(messageDataReader);
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

    public class EnterWorldRequest : Message {
        public static EnterWorldRequest read(BinaryReader binaryReader) {
            EnterWorldRequest newObj = new EnterWorldRequest();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            treeView.Nodes.Add(rootNode);
        }
    }

    public class EnterWorld : Message {
        public uint gid;
        public PStringChar account;

        public static EnterWorld read(BinaryReader binaryReader) {
            EnterWorld newObj = new EnterWorld();
            newObj.gid = binaryReader.ReadUInt32();
            newObj.account = PStringChar.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("gid = " + gid);
            rootNode.Nodes.Add("account = " + account.m_buffer);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class ForceObjdesc : Message {
        public uint object_id;

        public static ForceObjdesc read(BinaryReader binaryReader) {
            ForceObjdesc newObj = new ForceObjdesc();
            newObj.object_id = binaryReader.ReadUInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("object_id = " + object_id);
            treeView.Nodes.Add(rootNode);
        }
    }

    // TODO: This is bidirectional: client-to-sever has a gid; server-to-client does not
    public class LogOff : Message {
        public uint gid;

        public static LogOff read(BinaryReader binaryReader) {
            LogOff newObj = new LogOff();
            newObj.gid = binaryReader.ReadUInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("gid = " + gid);
            treeView.Nodes.Add(rootNode);
        }
    }
}

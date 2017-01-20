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

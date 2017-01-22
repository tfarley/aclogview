using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class CM_Vendor : MessageProcessor {

    public override bool acceptMessageData(BinaryReader messageDataReader, TreeView outputTreeView) {
        bool handled = true;

        PacketOpcode opcode = Util.readOpcode(messageDataReader);
        switch (opcode) {
            case PacketOpcode.Evt_Vendor__Buy_ID: {
                    Buy message = Buy.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Vendor__Sell_ID: {
                    Sell message = Sell.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            // TODO: PacketOpcode.Evt_Vendor__RequestVendorInfo_ID
            // TODO: PacketOpcode.VENDOR_INFO_EVENT:
            default: {
                    handled = false;
                    break;
                }
        }

        return handled;
    }

    public class ItemProfile {
        public uint amount;
        public uint iid;
        public CM_Physics.PublicWeenieDesc pwd;

        public static ItemProfile read(BinaryReader binaryReader) {
            ItemProfile newObj = new ItemProfile();
            newObj.amount = binaryReader.ReadUInt32();
            newObj.iid = binaryReader.ReadUInt32();
            // TODO: If (amt & (1 << 24)) != 0, then this should actually read an OldPublicWeenieDesc!
            newObj.pwd = CM_Physics.PublicWeenieDesc.read(binaryReader);
            newObj.amount = newObj.amount & 0xFFFFFF;
            return newObj;
        }

        public void contributeToTreeNode(TreeNode node) {
            node.Nodes.Add("amount = " + amount);
            node.Nodes.Add("iid = " + iid);
            TreeNode pwdNode = node.Nodes.Add("pwd = ");
            pwd.contributeToTreeNode(pwdNode);
        }
    }

    public class Buy : Message {
        public uint i_vendorID;
        public PList<ItemProfile> i_stuff;
        public uint i_alternateCurrencyID;

        public static Buy read(BinaryReader binaryReader) {
            Buy newObj = new Buy();
            newObj.i_vendorID = binaryReader.ReadUInt32();
            Util.readToAlign(binaryReader);
            newObj.i_stuff = PList<ItemProfile>.read(binaryReader);
            newObj.i_alternateCurrencyID = binaryReader.ReadUInt32();
            Util.readToAlign(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("i_vendorID = " + i_vendorID);
            TreeNode stuffNode = rootNode.Nodes.Add("i_stuff = ");
            i_stuff.contributeToTreeNode(stuffNode);
            rootNode.Nodes.Add("i_alternateCurrencyID = " + i_alternateCurrencyID);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class Sell : Message {
        public uint i_vendorID;
        public PList<ItemProfile> i_stuff;

        public static Sell read(BinaryReader binaryReader) {
            Sell newObj = new Sell();
            newObj.i_vendorID = binaryReader.ReadUInt32();
            Util.readToAlign(binaryReader);
            newObj.i_stuff = PList<ItemProfile>.read(binaryReader);
            Util.readToAlign(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("i_vendorID = " + i_vendorID);
            TreeNode stuffNode = rootNode.Nodes.Add("i_stuff = ");
            i_stuff.contributeToTreeNode(stuffNode);
            treeView.Nodes.Add(rootNode);
        }
    }
}

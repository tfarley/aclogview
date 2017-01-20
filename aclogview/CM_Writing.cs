using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class CM_Writing : MessageProcessor {

    public override bool acceptMessageData(BinaryReader messageDataReader, TreeView outputTreeView) {
        bool handled = true;

        PacketOpcode opcode = Util.readOpcode(messageDataReader);
        switch (opcode) {
            // TODO: Add all BOOK_*_RESPONSE_EVENT
            case PacketOpcode.Evt_Writing__BookData_ID: {
                    BookData message = BookData.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Writing__BookModifyPage_ID: {
                    BookModifyPage message = BookModifyPage.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Writing__BookAddPage_ID: {
                    BookAddPage message = BookAddPage.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Writing__BookDeletePage_ID: {
                    BookDeletePage message = BookDeletePage.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Writing__BookPageData_ID: {
                    BookPageData message = BookPageData.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            // TODO: Evt_Writing__GetInscription_ID
            case PacketOpcode.Evt_Writing__SetInscription_ID: {
                    SetInscription message = SetInscription.read(messageDataReader);
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

    public class BookData : Message {
        public uint i_objectID;

        public static BookData read(BinaryReader binaryReader) {
            BookData newObj = new BookData();
            newObj.i_objectID = binaryReader.ReadUInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("i_objectID = " + i_objectID);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class BookModifyPage : Message {
        public uint i_objectID;
        public int i_pageNum;
        public PStringChar i_pageText;

        public static BookModifyPage read(BinaryReader binaryReader) {
            BookModifyPage newObj = new BookModifyPage();
            newObj.i_objectID = binaryReader.ReadUInt32();
            newObj.i_pageNum = binaryReader.ReadInt32();
            newObj.i_pageText = PStringChar.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("i_objectID = " + i_objectID);
            rootNode.Nodes.Add("i_pageNum = " + i_pageNum);
            rootNode.Nodes.Add("i_pageText = " + i_pageText);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class BookAddPage : Message {
        public uint i_objectID;

        public static BookAddPage read(BinaryReader binaryReader) {
            BookAddPage newObj = new BookAddPage();
            newObj.i_objectID = binaryReader.ReadUInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("i_objectID = " + i_objectID);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class BookDeletePage : Message {
        public uint i_objectID;
        public int i_pageNum;

        public static BookDeletePage read(BinaryReader binaryReader) {
            BookDeletePage newObj = new BookDeletePage();
            newObj.i_objectID = binaryReader.ReadUInt32();
            newObj.i_pageNum = binaryReader.ReadInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("i_objectID = " + i_objectID);
            rootNode.Nodes.Add("i_pageNum = " + i_pageNum);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class BookPageData : Message {
        public uint i_objectID;
        public int i_pageNum;

        public static BookPageData read(BinaryReader binaryReader) {
            BookPageData newObj = new BookPageData();
            newObj.i_objectID = binaryReader.ReadUInt32();
            newObj.i_pageNum = binaryReader.ReadInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("i_objectID = " + i_objectID);
            rootNode.Nodes.Add("i_pageNum = " + i_pageNum);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class SetInscription : Message {
        public uint i_objectID;
        public PStringChar i_inscription;

        public static SetInscription read(BinaryReader binaryReader) {
            SetInscription newObj = new SetInscription();
            newObj.i_objectID = binaryReader.ReadUInt32();
            newObj.i_inscription = PStringChar.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("i_objectID = " + i_objectID);
            rootNode.Nodes.Add("i_inscription = " + i_inscription);
            treeView.Nodes.Add(rootNode);
        }
    }
}

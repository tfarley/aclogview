using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class CM_Login : MessageProcessor {

    public override bool acceptMessageData(BinaryReader messageDataReader, TreeView outputTreeView) {
        bool handled = true;

        PacketOpcode opcode = Util.readOpcode(messageDataReader);
        switch (opcode) {
            case PacketOpcode.Evt_Login__CharacterSet_ID: {
                    Login__CharacterSet message = Login__CharacterSet.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Login__WorldInfo_ID: {
                    WorldInfo message = WorldInfo.read(messageDataReader);
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

    public class CharacterIdentity {
        public uint gid_;
        public PStringChar name_;
        public uint secondsGreyedOut_;

        public static CharacterIdentity read(BinaryReader binaryReader) {
            CharacterIdentity newObj = new CharacterIdentity();
            newObj.gid_ = binaryReader.ReadUInt32();
            newObj.name_ = PStringChar.read(binaryReader);
            newObj.secondsGreyedOut_ = binaryReader.ReadUInt32();

            return newObj;
        }

        public void contributeToTreeNode(TreeNode node) {
            node.Nodes.Add("gid_ = " + gid_);
            node.Nodes.Add("name_ = " + name_.m_buffer);
            node.Nodes.Add("secondsGreyedOut_ = " + secondsGreyedOut_);
        }
    }

    public class Login__CharacterSet : Message {
        public uint status_;
        public List<CharacterIdentity> set_ = new List<CharacterIdentity>();
        public List<CharacterIdentity> delSet_ = new List<CharacterIdentity>();
        public uint numAllowedCharacters_;
        public PStringChar account_;
        public uint m_fUseTurbineChat;
        public uint m_fHasThroneofDestiny;

        public static Login__CharacterSet read(BinaryReader binaryReader) {
            Login__CharacterSet newObj = new Login__CharacterSet();
            newObj.status_ = binaryReader.ReadUInt32();
            uint setNum = binaryReader.ReadUInt32();
            for (uint i = 0; i < setNum; ++i) {
                newObj.set_.Add(CharacterIdentity.read(binaryReader));
            }
            uint delSetNum = binaryReader.ReadUInt32();
            for (uint i = 0; i < delSetNum; ++i) {
                newObj.delSet_.Add(CharacterIdentity.read(binaryReader));
            }
            newObj.numAllowedCharacters_ = binaryReader.ReadUInt32();
            newObj.account_ = PStringChar.read(binaryReader);
            newObj.m_fUseTurbineChat = binaryReader.ReadUInt32();
            newObj.m_fHasThroneofDestiny = binaryReader.ReadUInt32();

            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("status_ = " + status_);
            TreeNode setNode = rootNode.Nodes.Add("set_ = ");
            foreach (CharacterIdentity identity in set_) {
                identity.contributeToTreeNode(setNode);
            }
            TreeNode delSetNode = rootNode.Nodes.Add("delSet_ = ");
            foreach (CharacterIdentity identity in delSet_) {
                identity.contributeToTreeNode(delSetNode);
            }
            rootNode.Nodes.Add("numAllowedCharacters_ = " + numAllowedCharacters_);
            rootNode.Nodes.Add("account_ = " + account_.m_buffer);
            rootNode.Nodes.Add("m_fUseTurbineChat = " + m_fUseTurbineChat);
            rootNode.Nodes.Add("m_fHasThroneofDestiny = " + m_fHasThroneofDestiny);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class WorldInfo : Message {
        public int cConnections;
        public int cMaxConnections;
        public PStringChar strWorldName;

        public static WorldInfo read(BinaryReader binaryReader) {
            WorldInfo newObj = new WorldInfo();
            newObj.cConnections = binaryReader.ReadInt32();
            newObj.cMaxConnections = binaryReader.ReadInt32();
            newObj.strWorldName = PStringChar.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("cConnections = " + cConnections);
            rootNode.Nodes.Add("cMaxConnections = " + cMaxConnections);
            rootNode.Nodes.Add("strWorldName = " + strWorldName.m_buffer);
            treeView.Nodes.Add(rootNode);
        }
    }
}

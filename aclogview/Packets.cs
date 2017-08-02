﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class Util {
    public static IDictionary<Type, Func<BinaryReader, dynamic>> readers = null;

    public static void initReaders() {
        if (readers == null) {
            readers = new Dictionary<Type, Func<BinaryReader, dynamic>>();
            readers.Add(typeof(byte), r => r.ReadByte());
            readers.Add(typeof(ushort), r => r.ReadUInt16());
            readers.Add(typeof(short), r => r.ReadInt16());
            readers.Add(typeof(uint), r => r.ReadUInt32());
            readers.Add(typeof(int), r => r.ReadInt32());
            readers.Add(typeof(ulong), r => r.ReadUInt64());
            readers.Add(typeof(long), r => r.ReadInt64());
            readers.Add(typeof(float), r => r.ReadSingle());
            readers.Add(typeof(double), r => r.ReadDouble());

            readers.Add(typeof(PStringChar), r => PStringChar.read(r));

            readers.Add(typeof(STypeInt), r => (STypeInt)r.ReadUInt32());
            readers.Add(typeof(STypeInt64), r => (STypeInt64)r.ReadUInt32());
            readers.Add(typeof(STypeBool), r => (STypeBool)r.ReadUInt32());
            readers.Add(typeof(STypeFloat), r => (STypeFloat)r.ReadUInt32());
            readers.Add(typeof(STypeString), r => (STypeString)r.ReadUInt32());
            readers.Add(typeof(STypeDID), r => (STypeDID)r.ReadUInt32());
            readers.Add(typeof(STypePosition), r => (STypePosition)r.ReadUInt32());
            readers.Add(typeof(STypeIID), r => (STypeIID)r.ReadUInt32());
            readers.Add(typeof(STypeSkill), r => (STypeSkill)r.ReadUInt32());

            readers.Add(typeof(CharacterTitle), r => (CharacterTitle)r.ReadUInt32());
            readers.Add(typeof(SKILL_ADVANCEMENT_CLASS), r => (SKILL_ADVANCEMENT_CLASS)r.ReadUInt32());

            readers.Add(typeof(CM_Magic.Enchantment), r => CM_Magic.Enchantment.read(r));
            readers.Add(typeof(CM_Social.FriendData), r => CM_Social.FriendData.read(r));
            readers.Add(typeof(Attribute), r => Attribute.read(r));
            readers.Add(typeof(SecondaryAttribute), r => SecondaryAttribute.read(r));
            readers.Add(typeof(Position), r => Position.read(r));
            readers.Add(typeof(Skill), r => Skill.read(r));

            readers.Add(typeof(SpellID), r => (SpellID)r.ReadUInt32());
            readers.Add(typeof(CM_Vendor.ItemProfile), r => CM_Vendor.ItemProfile.read(r));
            readers.Add(typeof(CM_Login.ContentProfile), r => CM_Login.ContentProfile.read(r));
            readers.Add(typeof(CM_Login.InventoryPlacement), r => CM_Login.InventoryPlacement.read(r));
            readers.Add(typeof(CM_Character.BaseProperty), r => CM_Character.BaseProperty.read(r));
        }
    }

    public static ushort byteSwapped(ushort value) {
        return (ushort)(((value & 0x00FFU) << 8) | ((value & 0xFF00U) >> 8));
    }

    public static void readToAlign(BinaryReader binaryReader) {
        long alignDelta = binaryReader.BaseStream.Position % 4;
        if (alignDelta != 0) {
            binaryReader.ReadBytes((int)(4 - alignDelta));
        }
    }

    public static uint readDataIDOfKnownType(uint i_didFirstID, BinaryReader binaryReader) {
        ushort offset = binaryReader.ReadUInt16();

        if ((offset & 0x8000) == 0) {
            return i_didFirstID + offset;
        } else {
            ushort offsetHigh = binaryReader.ReadUInt16();
            return i_didFirstID + (uint)(offsetHigh | ((offset & 0x3FFF) << 16));
        }
    }

    public static uint readWClassIDCompressed(BinaryReader binaryReader) {
        ushort id = binaryReader.ReadUInt16();

        if ((id & 0x8000) == 0) {
            return id;
        } else {
            ushort idHigh = binaryReader.ReadUInt16();
            return (uint)(idHigh | ((id & 0x7FFF) << 16));
        }
    }

    public static PacketOpcode readOpcode(BinaryReader fragDataReader) {
        PacketOpcode opcode = 0;
        opcode = (PacketOpcode)fragDataReader.ReadUInt32();
        if (opcode == PacketOpcode.WEENIE_ORDERED_EVENT) {
            WOrderHdr orderHeader = WOrderHdr.read(fragDataReader);
            opcode = (PacketOpcode)fragDataReader.ReadUInt32();
        }
        if (opcode == PacketOpcode.ORDERED_EVENT) {
            OrderHdr orderHeader = OrderHdr.read(fragDataReader);
            opcode = (PacketOpcode)fragDataReader.ReadUInt32();
        }

        return opcode;
    }
}

public class NetBlobIDUtils {
    public static bool IsEphemeralFlagSet(ulong _id) {
        return (_id & 0x8000000000000000) != 0;
    }

    public static ulong GetOrderingType(ulong _id) {
        return (_id & 0x1F00000000000000);
    }

    public static ulong GetSequenceID(ulong _id) {
        return (_id & 0x00FF0000FFFFFFFF);
    }
}

public class NetBlob {
    public enum State {
        NETBLOB_FROZEN,
	    NETBLOB_SENDING,
	    NETBLOB_RECEIVING,
	    NETBLOB_RECEIVED,
	    NETBLOB_FRAGMENTED
    }

    public State state_;
    public byte[] buf_;
    public uint cMaxFragments_;
    public uint numFragments_;
    public ushort sender_;
    public ushort queueID_;
    public uint priority_;
    public NetBlob waitNext_;
    ulong savedNetBlobID_;
}

public class BlobFrag {
    //public BlobFragHeader_t hdrWrite_;
    //public BlobFragHeader_t hdrRead_;
    public BlobFragHeader_t memberHeader_;
    public byte[] dat_;
    //public NetBlob myBlob_;
}

public class NetPacket {
    public enum Flags__guessedname {
        npfChecksumEncrypted = (1 << 0),
        npfHasTimeSensitiveHeaders = (1 << 1),
        npfHasSequencedData = (1 << 2),
        npfHasHighPriorityHeaders = (1 << 3)
    }

    public List<COptionalHeader> specialFragList_ = new List<COptionalHeader>();
    public List<BlobFrag> fragList_ = new List<BlobFrag>();
    public ushort recipient_;
    public uint realPriority_;
    public uint size_;
    public uint seqNum_;
    public uint cryptoKey_;
    public uint checksum_;
    public uint flags_;
}

public class PStringChar {
    public string m_buffer;

    public static PStringChar read(BinaryReader binaryReader) {
        PStringChar newObj = new PStringChar();

        uint size = binaryReader.ReadUInt16();
        if (size == ushort.MaxValue) {
            binaryReader.BaseStream.Seek(-2, SeekOrigin.Current);
            size = binaryReader.ReadUInt32();
        }

        if (size == 0) {
            newObj.m_buffer = null;
        } else {
            newObj.m_buffer = new string(binaryReader.ReadChars((int)size));
        }

        Util.readToAlign(binaryReader);

        return newObj;
    }

    public override string ToString() {
        return m_buffer;
    }
}

public class PList<T> {
    public List<T> list = new List<T>();

    public static PList<T> read(BinaryReader binaryReader) {
        PList<T> newObj = new PList<T>();
        uint numElements = binaryReader.ReadUInt32();
        for (int i = 0; i < numElements; ++i) {
            newObj.list.Add(Util.readers[typeof(T)](binaryReader));
        }
        return newObj;
    }

    public void contributeToTreeNode(TreeNode node) {
        foreach (T element in list) {
            node.Nodes.Add(element.ToString());
        }
    }
}

public class PackableHashTable<TKey, TValue> {
    public Dictionary<TKey, TValue> hashTable = new Dictionary<TKey, TValue>();

    public static PackableHashTable<TKey, TValue> read(BinaryReader binaryReader) {
        PackableHashTable<TKey, TValue> newObj = new PackableHashTable<TKey, TValue>();
        uint sizeInfo = binaryReader.ReadUInt32();
        uint _table_size = sizeInfo >> 16; // NOTE: We don't actually need the bucket sizes since C# will just do its own thing internally
        uint _currNum = sizeInfo & 0xFFFF;
        for (int i = 0; i < _currNum; ++i) {
            newObj.hashTable.Add(Util.readers[typeof(TKey)](binaryReader), Util.readers[typeof(TValue)](binaryReader));
        }
        return newObj;
    }

    public void contributeToTreeNode(TreeNode node) {
        foreach (KeyValuePair<TKey, TValue> element in hashTable) {
            node.Nodes.Add(element.Key + " -> " + element.Value);
        }
    }
}

public abstract class Message {
    public abstract void contributeToTreeView(TreeView treeView);
}

public class EmptyMessage : Message {
    public PacketOpcode opcode;

    public EmptyMessage(PacketOpcode opcode) {
        this.opcode = opcode;
    }

    public override void contributeToTreeView(TreeView treeView) {
        TreeNode rootNode = new TreeNode(opcode.ToString());
        rootNode.Expand();
        treeView.Nodes.Add(rootNode);
    }
}

public class MessageProcessor {
    public virtual bool acceptMessageData(BinaryReader messageDataReader, TreeView outputTreeView) {
        return false;
    }
}

public class Vector3 {
    public float x;
    public float y;
    public float z;

    public static Vector3 read(BinaryReader binaryReader) {
        Vector3 newObj = new Vector3();
        newObj.x = binaryReader.ReadSingle();
        newObj.y = binaryReader.ReadSingle();
        newObj.z = binaryReader.ReadSingle();
        return newObj;
    }

    public override string ToString() {
        StringBuilder builder = new StringBuilder();

        builder.Append(x);
        builder.Append(" ");
        builder.Append(y);
        builder.Append(" ");
        builder.Append(z);

        return builder.ToString();
    }
}

public class Mat3x3 {
    public float[] entries = new float[9];

    public override string ToString() {
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < entries.Length; ++i) {
            builder.Append(entries[i]);
            builder.Append(" ");
        }

        return builder.ToString();
    }
}

public class Frame {
    public Vector3 m_fOrigin;
    public float qw = 1.0f;
    public float qx;
    public float qy;
    public float qz;
    public Mat3x3 m_fl2gv = new Mat3x3(); // Local-to-global matrix?

    public void cache() {
        // TODO: Clean this up
        float b2 = qx + qx;
        float c2 = qy + qy;
        float v3 = qz + qz;
        float v4 = qw * b2;
        float v5 = qw * c2;
        float v6 = qw * v3;
        float v7 = qx * b2;
        float v8 = qx * c2;
        float bd2 = qx * v3;
        float cc2 = qy * c2;
        float v11 = qy * v3;
        float v12 = qz * v3;
        m_fl2gv.entries[0] = 1.0f - cc2 - v12;
        m_fl2gv.entries[1] = v8 + v6;
        m_fl2gv.entries[2] = bd2 - v5;
        m_fl2gv.entries[3] = v8 - v6;
        float v13 = 1.0f - v7;
        m_fl2gv.entries[4] = v13 - v12;
        m_fl2gv.entries[5] = v11 + v4;
        m_fl2gv.entries[6] = bd2 + v5;
        m_fl2gv.entries[7] = v11 - v4;
        m_fl2gv.entries[8] = v13 - cc2;
    }

    public static Frame read(BinaryReader binaryReader) {
        Frame newObj = new Frame();
        newObj.m_fOrigin = Vector3.read(binaryReader);
        newObj.qw = binaryReader.ReadSingle();
        newObj.qx = binaryReader.ReadSingle();
        newObj.qy = binaryReader.ReadSingle();
        newObj.qz = binaryReader.ReadSingle();
        newObj.cache();
        return newObj;
    }

    public void contributeToTreeNode(TreeNode node) {
        node.Nodes.Add("m_fOrigin = " + m_fOrigin);
        node.Nodes.Add("qw = " + qw);
        node.Nodes.Add("qx = " + qx);
        node.Nodes.Add("qy = " + qy);
        node.Nodes.Add("qz = " + qz);
        node.Nodes.Add("m_fl2gv = " + m_fl2gv);
    }
}

public class Position {
    public uint objcell_id;
    public Frame frame = new Frame();

    public static Position read(BinaryReader binaryReader) {
        Position newObj = new Position();
        newObj.objcell_id = binaryReader.ReadUInt32();
        newObj.frame = Frame.read(binaryReader);
        return newObj;
    }

    public static Position readOrigin(BinaryReader binaryReader) {
        Position newObj = new Position();
        newObj.objcell_id = binaryReader.ReadUInt32();
        newObj.frame.m_fOrigin = Vector3.read(binaryReader);
        return newObj;
    }

    public void contributeToTreeNode(TreeNode node) {
        node.Nodes.Add("objcell_id = 0x" + objcell_id.ToString("X8"));
        TreeNode frameNode = node.Nodes.Add("frame = ");
        frame.contributeToTreeNode(frameNode);
    }
}

public class Skill {
    public uint _level_from_pp;
    public SKILL_ADVANCEMENT_CLASS _sac;
    public uint _pp;
    public uint _init_level;
    public uint _resistance_of_last_check;
    public double _last_used_time;

    public static Skill read(BinaryReader binaryReader) {
        Skill newObj = new Skill();
        newObj._level_from_pp = binaryReader.ReadUInt32();
        newObj._sac = (SKILL_ADVANCEMENT_CLASS)binaryReader.ReadUInt32();
        newObj._pp = binaryReader.ReadUInt32();
        newObj._init_level = binaryReader.ReadUInt32();
       // newObj._resistance_of_last_check = binaryReader.ReadDouble();
        newObj._resistance_of_last_check = binaryReader.ReadUInt32();
        newObj._last_used_time = binaryReader.ReadDouble();
        return newObj;
    }

    public void contributeToTreeNode(TreeNode node) {
        node.Nodes.Add("_level_from_pp = " + _level_from_pp);
        node.Nodes.Add("_sac = " + _sac);
        node.Nodes.Add("_pp = " + _pp);
        node.Nodes.Add("_init_level = " + _init_level);
        node.Nodes.Add("_resistance_of_last_check = " + _resistance_of_last_check);
        node.Nodes.Add("_last_used_time = " + _last_used_time);
    }
}

public class Attribute {
    public uint _level_from_cp;
    public uint _init_level;
    public uint _cp_spent;

    public static Attribute read(BinaryReader binaryReader) {
        Attribute newObj = new Attribute();
        newObj._level_from_cp = binaryReader.ReadUInt32();
        newObj._init_level = binaryReader.ReadUInt32();
        newObj._cp_spent = binaryReader.ReadUInt32();
        return newObj;
    }

    public void contributeToTreeNode(TreeNode node) {
        node.Nodes.Add("_level_from_cp = " + _level_from_cp);
        node.Nodes.Add("_init_level = " + _init_level);
        node.Nodes.Add("_cp_spent = " + _cp_spent);
    }
}

public class SecondaryAttribute {
    public uint _level_from_cp;
    public uint _init_level;
    public uint _cp_spent;
    public uint _current_level;

    public static SecondaryAttribute read(BinaryReader binaryReader) {
        SecondaryAttribute newObj = new SecondaryAttribute();
        newObj._level_from_cp = binaryReader.ReadUInt32();
        newObj._init_level = binaryReader.ReadUInt32();
        newObj._cp_spent = binaryReader.ReadUInt32();
        newObj._current_level = binaryReader.ReadUInt32();
        return newObj;
    }

    public void contributeToTreeNode(TreeNode node) {
        node.Nodes.Add("_level_from_cp = " + _level_from_cp);
        node.Nodes.Add("_init_level = " + _init_level);
        node.Nodes.Add("_cp_spent = " + _cp_spent);
        node.Nodes.Add("_current_level = " + _current_level);
    }
}

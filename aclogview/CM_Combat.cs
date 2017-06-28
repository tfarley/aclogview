﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using aclogview;

public class CM_Combat : MessageProcessor {

    public override bool acceptMessageData(BinaryReader messageDataReader, TreeView outputTreeView) {
        bool handled = true;

        PacketOpcode opcode = Util.readOpcode(messageDataReader);
        switch (opcode) {
            case PacketOpcode.ATTACK_DONE_EVENT: // 0x01A7
                {
                    AttackDone message = AttackDone.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Combat__CancelAttack_ID:
            case PacketOpcode.Evt_Combat__CommenceAttack_ID: {
                    EmptyMessage message = new EmptyMessage(opcode);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            // TODO: Evt_Combat__UntargetedMeleeAttack_ID
            case PacketOpcode.Evt_Combat__TargetedMeleeAttack_ID: {
                    TargetedMeleeAttack message = TargetedMeleeAttack.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Combat__TargetedMissileAttack_ID: {
                    TargetedMissileAttack message = TargetedMissileAttack.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            // TODO: Evt_Combat__UntargetedMissileAttack_ID
            case PacketOpcode.Evt_Combat__ChangeCombatMode_ID: {
                    ChangeCombatMode message = ChangeCombatMode.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Combat__QueryHealth_ID: {
                    QueryHealth message = QueryHealth.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Combat__QueryHealthResponse_ID: {
                    QueryHealthResponse message = QueryHealthResponse.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.ATTACKER_NOTIFICATION_EVENT:
                {
                    AttackerNotificationEvent message = AttackerNotificationEvent.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.DEFENDER_NOTIFICATION_EVENT:
                {
                    DefenderNotificationEvent message = DefenderNotificationEvent.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.EVASION_ATTACKER_NOTIFICATION_EVENT:
                {
                    EvasionAttackerNotificationEvent message = EvasionAttackerNotificationEvent.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.EVASION_DEFENDER_NOTIFICATION_EVENT:
                {
                    EvasionDefenderNotificationEvent message = EvasionDefenderNotificationEvent.read(messageDataReader);
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

    ///<remarks>
    /// 0x01A7 is found in many packet captures and it commonly returns a 0.
    /// TODO:
    ///     1. Verify/search for the true meaning of this packet and it's contents. The current documentation is too limited to trust that
    ///     the integer returned from the packet indicates the number of attacks. 
    ///     Documentation: http://acemulator.org/ProtocolViewer/Protocol.php?stylesheet=Classic.css&frame=frameMsg&type=F7B0&case=0x01A7
    ///         Supporting Evidence:
    ///             Here are the results from doing a quick count of the values that come back from ATTACK_DONE, utilizing 2,376 PCAP files:
    ///                 int value : occurrences total
    ///                 0 : 255,702
    ///                 2 : 2,140
    ///                 14 : 3
    ///                 29 : 550
    ///                 31 : 1
    ///                 35 : 115
    ///                 54 : 6,698
    ///                 55 : 4,717
    ///                 56 : 2,427
    ///                 57 : 5,795
    ///                 58 : 50
    ///                 60 : 1
    ///                 61 : 203
    ///                 75 : 47
    ///                 1016 : 42
    ///                 1067 : 24
    ///                 1073 : 7
    /// </remarks>
    public class AttackDone : Message
    {
        public uint NumberOfAttacks;

        public static AttackDone read(BinaryReader binaryReader)
        {
            AttackDone newObj = new AttackDone();
            newObj.NumberOfAttacks = binaryReader.ReadUInt32();
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("NumberOfAttacks = " + this.NumberOfAttacks);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class TargetedMeleeAttack : Message {
        public uint i_targetID;
        public ATTACK_HEIGHT i_ah;
        public float i_power_level;

        public static TargetedMeleeAttack read(BinaryReader binaryReader) {
            TargetedMeleeAttack newObj = new TargetedMeleeAttack();
            newObj.i_targetID = binaryReader.ReadUInt32();
            newObj.i_ah = (ATTACK_HEIGHT)binaryReader.ReadUInt32();
            newObj.i_power_level = binaryReader.ReadSingle();
            Util.readToAlign(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("i_targetID = " + Utility.FormatGuid(this.i_targetID));            
            
            rootNode.Nodes.Add("i_ah = " + i_ah);
            rootNode.Nodes.Add("i_power_level = " + i_power_level);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class TargetedMissileAttack : Message {
        public uint i_targetID;
        public ATTACK_HEIGHT i_ah;
        public float i_accuracy_level;

        public static TargetedMissileAttack read(BinaryReader binaryReader) {
            TargetedMissileAttack newObj = new TargetedMissileAttack();
            newObj.i_targetID = binaryReader.ReadUInt32();
            newObj.i_ah = (ATTACK_HEIGHT)binaryReader.ReadUInt32();
            newObj.i_accuracy_level = binaryReader.ReadSingle();
            Util.readToAlign(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("i_target_id = " + Utility.FormatGuid(this.i_targetID));
            rootNode.Nodes.Add("i_ah = " + i_ah);
            rootNode.Nodes.Add("i_accuracy_level = " + i_accuracy_level);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class ChangeCombatMode : Message {
        public COMBAT_MODE i_mode;

        public static ChangeCombatMode read(BinaryReader binaryReader) {
            ChangeCombatMode newObj = new ChangeCombatMode();
            newObj.i_mode = (COMBAT_MODE)binaryReader.ReadUInt32();
            Util.readToAlign(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("i_mode = " + i_mode);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class QueryHealth : Message {
        public uint i_target;

        public static QueryHealth read(BinaryReader binaryReader) {
            QueryHealth newObj = new QueryHealth();
            newObj.i_target = binaryReader.ReadUInt32();
            Util.readToAlign(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("i_target_ = " + Utility.FormatGuid(this.i_target));
            treeView.Nodes.Add(rootNode);
        }
    }

    public class QueryHealthResponse : Message {
        public uint target;
        public float health;

        public static QueryHealthResponse read(BinaryReader binaryReader) {
            QueryHealthResponse newObj = new QueryHealthResponse();
            newObj.target = binaryReader.ReadUInt32();
            newObj.health = binaryReader.ReadSingle();
            Util.readToAlign(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("target = " +Utility.FormatGuid(this.target));
            rootNode.Nodes.Add("health = " + health);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class AttackerNotificationEvent : Message
    {
        public PStringChar defenders_name;
        public uint damage_type;
        public double severity; // 0.0 to 1.0
        public uint damage;
        public uint critical;
        public ulong attack_conditions;

        public static AttackerNotificationEvent read(BinaryReader binaryReader)
        {
            AttackerNotificationEvent newObj = new AttackerNotificationEvent();
            newObj.defenders_name = PStringChar.read(binaryReader);
            newObj.damage_type = binaryReader.ReadUInt32();
            newObj.severity = binaryReader.ReadDouble();
            newObj.damage = binaryReader.ReadUInt32();
            newObj.critical = binaryReader.ReadUInt32();
            newObj.attack_conditions = binaryReader.ReadUInt64();
            Util.readToAlign(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("defenders_name = " + defenders_name);
            rootNode.Nodes.Add("damage_type = " + (DAMAGE_TYPE)damage_type);
            rootNode.Nodes.Add("severity = " + severity);
            rootNode.Nodes.Add("damage = " + damage);
            rootNode.Nodes.Add("critical = " + critical);
            rootNode.Nodes.Add("attackConditions = " + attack_conditions);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class DefenderNotificationEvent : Message
    {
        public PStringChar attackers_name;
        public uint damage_type;
        public double severity; // 0.0 to 1.0
        public uint damage;
        public uint part;
        public uint critical;
        public ulong attack_conditions;

        public static DefenderNotificationEvent read(BinaryReader binaryReader)
        {
            DefenderNotificationEvent newObj = new DefenderNotificationEvent();
            newObj.attackers_name = PStringChar.read(binaryReader);
            newObj.damage_type = binaryReader.ReadUInt32();
            newObj.severity = binaryReader.ReadDouble();
            newObj.damage = binaryReader.ReadUInt32();
            newObj.part = binaryReader.ReadUInt32();
            newObj.critical = binaryReader.ReadUInt32();
            newObj.attack_conditions = binaryReader.ReadUInt64();
            Util.readToAlign(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("attackers_name = " + attackers_name);
            rootNode.Nodes.Add("damage_type = " + (DAMAGE_TYPE)damage_type);
            rootNode.Nodes.Add("severity = " + severity);
            rootNode.Nodes.Add("damage = " + damage);
            rootNode.Nodes.Add("part = " + (BodyPart)part);
            rootNode.Nodes.Add("critical = " + critical);
            rootNode.Nodes.Add("attackConditions = " + attack_conditions);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class EvasionAttackerNotificationEvent : Message
    {
        public PStringChar defenders_name;

        public static EvasionAttackerNotificationEvent read(BinaryReader binaryReader)
        {
            EvasionAttackerNotificationEvent newObj = new EvasionAttackerNotificationEvent();
            newObj.defenders_name = PStringChar.read(binaryReader);
            Util.readToAlign(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("defenders_name = " + defenders_name);
            treeView.Nodes.Add(rootNode);
        }
    }

    public class EvasionDefenderNotificationEvent : Message
    {
        public PStringChar attackers_name;

        public static EvasionDefenderNotificationEvent read(BinaryReader binaryReader)
        {
            EvasionDefenderNotificationEvent newObj = new EvasionDefenderNotificationEvent();
            newObj.attackers_name = PStringChar.read(binaryReader);
            Util.readToAlign(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("attackers_name = " + attackers_name);
            treeView.Nodes.Add(rootNode);
        }
    }
}

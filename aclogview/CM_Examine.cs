using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class CM_Examine : MessageProcessor {

    public override bool acceptMessageData(BinaryReader messageDataReader, TreeView outputTreeView) {
        bool handled = true;

        PacketOpcode opcode = Util.readOpcode(messageDataReader);
        switch (opcode) {
            case PacketOpcode.APPRAISAL_INFO_EVENT: {
                    SetAppraiseInfo message = SetAppraiseInfo.read(messageDataReader);
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

    public class CreatureAppraisalProfile {
        public enum CreatureAppraisalProfilePackHeader {
            Packed_None = 0,
            Packed_Enchantments = (1 << 0),
            // NOTE: Skip 2
            Packed_Attributes = (1 << 3)
        }

        public enum Enchantment_BFIndex {
            BF_STRENGTH = (1 << 0),
            BF_ENDURANCE = (1 << 1),
            BF_QUICKNESS = (1 << 2),
            BF_COORDINATION = (1 << 3),
            BF_FOCUS = (1 << 4),
            BF_SELF = (1 << 5),
            BF_MAX_HEALTH = (1 << 6),
            BF_MAX_STAMINA = (1 << 7),
            BF_MAX_MANA = (1 << 8),
            // NOTE: Skip 7
            BF_STRENGTH_HI = (1 << 16),
            BF_ENDURANCE_HI = (1 << 17),
            BF_QUICKNESS_HI = (1 << 18),
            BF_COORDINATION_HI = (1 << 19),
            BF_FOCUS_HI = (1 << 20),
            BF_SELF_HI = (1 << 21),
            BF_MAX_HEALTH_HI = (1 << 22),
            BF_MAX_STAMINA_HI = (1 << 23),
            BF_MAX_MANA_HI = (1 << 24)
        }


    }

    public class AppraisalProfile {
        public enum AppraisalProfilePackHeader {
            Packed_None = 0,
            Packed_IntStats = (1 << 0),
            Packed_BoolStats = (1 << 1),
            Packed_FloatStats = (1 << 2),
            Packed_StringStats = (1 << 3),
            Packed_SpellList = (1 << 4),
            Packed_WeaponProfile = (1 << 5),
            Packed_HookProfile = (1 << 6),
            Packed_ArmorProfile = (1 << 7),
            Packed_CreatureProfile = (1 << 8),
            Packed_ArmorEnchant = (1 << 9),
            Packed_ResistEnchant = (1 << 10),
            Packed_WeaponEnchant = (1 << 11),
            Packed_DataIDStats = (1 << 12),
            Packed_Int64Stats = (1 << 13),
            Packed_ArmorLevels = (1 << 14)
        }

        public enum ResistanceEnchantment_BFIndex {
            BF_RESIST_SLASH = (1 << 0),
            BF_RESIST_PIERCE = (1 << 1),
            BF_RESIST_BLUDGEON = (1 << 2),
            BF_RESIST_FIRE = (1 << 3),
            BF_RESIST_COLD = (1 << 4),
            BF_RESIST_ACID = (1 << 5),
            BF_RESIST_ELECTRIC = (1 << 6),
            BF_RESIST_HEALTH_BOOST = (1 << 7),
            BF_RESIST_STAMINA_DRAIN = (1 << 8),
            BF_RESIST_STAMINA_BOOST = (1 << 9),
            BF_RESIST_MANA_DRAIN = (1 << 10),
            BF_RESIST_MANA_BOOST = (1 << 11),
            BF_MANA_CON_MOD = (1 << 12),
            BF_ELE_DAMAGE_MOD = (1 << 13),
            BF_RESIST_NETHER = (1 << 14),
            // NOTE: Skip 1
            BF_RESIST_SLASH_HI = (1 << 16),
            BF_RESIST_PIERCE_HI = (1 << 17),
            BF_RESIST_BLUDGEON_HI = (1 << 18),
            BF_RESIST_FIRE_HI = (1 << 19),
            BF_RESIST_COLD_HI = (1 << 20),
            BF_RESIST_ACID_HI = (1 << 21),
            BF_RESIST_ELECTRIC_HI = (1 << 22),
            BF_RESIST_HEALTH_BOOST_HI = (1 << 23),
            BF_RESIST_STAMINA_DRAIN_HI = (1 << 24),
            BF_RESIST_STAMINA_BOOST_HI = (1 << 25),
            BF_RESIST_MANA_DRAIN_HI = (1 << 26),
            BF_RESIST_MANA_BOOST_HI = (1 << 27),
            BF_MANA_CON_MOD_HI = (1 << 28),
            BF_ELE_DAMAGE_MOD_HI = (1 << 29),
            BF_RESIST_NETHER_HI = (1 << 30)
        }

        public enum WeaponEnchantment_BFIndex {
            BF_WEAPON_OFFENSE = (1 << 0),
            BF_WEAPON_DEFENSE = (1 << 1),
            BF_WEAPON_TIME = (1 << 2),
            BF_DAMAGE = (1 << 3),
            BF_DAMAGE_VARIANCE = (1 << 4),
            BF_DAMAGE_MOD = (1 << 5),
            // NOTE: Skip 10
            BF_WEAPON_OFFENSE_HI = (1 << 16),
            BF_WEAPON_DEFENSE_HI = (1 << 17),
            BF_WEAPON_TIME_HI = (1 << 18),
            BF_DAMAGE_HI = (1 << 19),
            BF_DAMAGE_VARIANCE_HI = (1 << 20),
            BF_DAMAGE_MOD_HI = (1 << 21)
        }

        public enum ArmorEnchantment_BFIndex {
            BF_ARMOR_LEVEL = (1 << 0),
            BF_ARMOR_MOD_VS_SLASH = (1 << 1),
            BF_ARMOR_MOD_VS_PIERCE = (1 << 2),
            BF_ARMOR_MOD_VS_BLUDGEON = (1 << 3),
            BF_ARMOR_MOD_VS_COLD = (1 << 4),
            BF_ARMOR_MOD_VS_FIRE = (1 << 5),
            BF_ARMOR_MOD_VS_ACID = (1 << 6),
            BF_ARMOR_MOD_VS_ELECTRIC = (1 << 7),
            BF_ARMOR_MOD_VS_NETHER = (1 << 8),
            // NOTE: Skip 7
            BF_ARMOR_LEVEL_HI = (1 << 16),
            BF_ARMOR_MOD_VS_SLASH_HI = (1 << 17),
            BF_ARMOR_MOD_VS_PIERCE_HI = (1 << 18),
            BF_ARMOR_MOD_VS_BLUDGEON_HI = (1 << 19),
            BF_ARMOR_MOD_VS_COLD_HI = (1 << 20),
            BF_ARMOR_MOD_VS_FIRE_HI = (1 << 21),
            BF_ARMOR_MOD_VS_ACID_HI = (1 << 22),
            BF_ARMOR_MOD_VS_ELECTRIC_HI = (1 << 23),
            BF_ARMOR_MOD_VS_NETHER_HI = (1 << 24)
        }

        public uint header;
        public uint success_flag;
        public PackableHashTable<STypeInt, int> _intStatsTable = new PackableHashTable<STypeInt, int>();
        public PackableHashTable<STypeInt64, long> _int64StatsTable = new PackableHashTable<STypeInt64, long>();
        public PackableHashTable<STypeBool, int> _boolStatsTable = new PackableHashTable<STypeBool, int>();
        public PackableHashTable<STypeFloat, double> _floatStatsTable = new PackableHashTable<STypeFloat, double>();
        public PackableHashTable<STypeString, PStringChar> _strStatsTable = new PackableHashTable<STypeString, PStringChar>();
        public PackableHashTable<STypeDID, uint> _didStatsTable = new PackableHashTable<STypeDID, uint>();

        public static AppraisalProfile read(BinaryReader binaryReader) {
            AppraisalProfile newObj = new AppraisalProfile();
            newObj.header = binaryReader.ReadUInt32();
            newObj.success_flag = binaryReader.ReadUInt32();
            if ((newObj.header & (uint)AppraisalProfilePackHeader.Packed_IntStats) != 0) {
                newObj._intStatsTable = PackableHashTable<STypeInt, int>.read(binaryReader);
            }
            if ((newObj.header & (uint)AppraisalProfilePackHeader.Packed_Int64Stats) != 0) {
                newObj._int64StatsTable = PackableHashTable<STypeInt64, long>.read(binaryReader);
            }
            if ((newObj.header & (uint)AppraisalProfilePackHeader.Packed_BoolStats) != 0) {
                newObj._boolStatsTable = PackableHashTable<STypeBool, int>.read(binaryReader);
            }
            if ((newObj.header & (uint)AppraisalProfilePackHeader.Packed_FloatStats) != 0) {
                newObj._floatStatsTable = PackableHashTable<STypeFloat, double>.read(binaryReader);
            }
            if ((newObj.header & (uint)AppraisalProfilePackHeader.Packed_StringStats) != 0) {
                newObj._strStatsTable = PackableHashTable<STypeString, PStringChar>.read(binaryReader);
            }
            if ((newObj.header & (uint)AppraisalProfilePackHeader.Packed_DataIDStats) != 0) {
                newObj._didStatsTable = PackableHashTable<STypeDID, uint>.read(binaryReader);
            }
            // TODO: Lots more to read here
            return newObj;
        }

        public void contributeToTreeNode(TreeNode node) {
            node.Nodes.Add("header = " + header);
            node.Nodes.Add("success_flag = " + success_flag);
            TreeNode intStatsNode = node.Nodes.Add("_intStatsTable = ");
            _intStatsTable.contributeToTreeNode(intStatsNode);
            TreeNode int64StatsNode = node.Nodes.Add("_int64StatsTable = ");
            _int64StatsTable.contributeToTreeNode(int64StatsNode);
            TreeNode boolStatsNode = node.Nodes.Add("_boolStatsTable = ");
            _boolStatsTable.contributeToTreeNode(boolStatsNode);
            TreeNode floatStatsNode = node.Nodes.Add("_floatStatsTable = ");
            _floatStatsTable.contributeToTreeNode(floatStatsNode);
            TreeNode strStatsNode = node.Nodes.Add("_strStatsTable = ");
            _strStatsTable.contributeToTreeNode(strStatsNode);
            TreeNode didStatsNode = node.Nodes.Add("_didStatsTable = ");
            _didStatsTable.contributeToTreeNode(didStatsNode);
            // TODO: Lots more to read here
        }
    }

    public class SetAppraiseInfo : Message {
        public uint i_objid;
        public AppraisalProfile i_prof;

        public static SetAppraiseInfo read(BinaryReader binaryReader) {
            SetAppraiseInfo newObj = new SetAppraiseInfo();
            newObj.i_objid = binaryReader.ReadUInt32();
            newObj.i_prof = AppraisalProfile.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("i_objid = " + i_objid);
            TreeNode profileNode = rootNode.Nodes.Add("i_prof = ");
            i_prof.contributeToTreeNode(profileNode);
            treeView.Nodes.Add(rootNode);
        }
    }
}

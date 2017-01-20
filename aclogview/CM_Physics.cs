using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class CM_Physics : MessageProcessor {

    public override bool acceptMessageData(BinaryReader messageDataReader, TreeView outputTreeView) {
        bool handled = true;

        PacketOpcode opcode = Util.readOpcode(messageDataReader);
        switch (opcode) {
            case PacketOpcode.Evt_Physics__CreateObject_ID: {
                    CreateObject message = CreateObject.read(messageDataReader);
                    message.contributeToTreeView(outputTreeView);
                    break;
                }
            case PacketOpcode.Evt_Physics__CreatePlayer_ID: {
                    CreatePlayer message = CreatePlayer.read(messageDataReader);
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

    public class CreatePlayer : Message {
        public uint object_id;

        public static CreatePlayer read(BinaryReader binaryReader) {
            CreatePlayer newObj = new CreatePlayer();
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

    public class Subpalette {
        public uint subID;
        public uint offset;
        public uint numcolors;

        public static Subpalette read(BinaryReader binaryReader) {
            Subpalette newObj = new Subpalette();
            newObj.subID = Util.readDataIDOfKnownType(0x4000000, binaryReader);
            newObj.offset = 8u * binaryReader.ReadByte();
            newObj.numcolors = binaryReader.ReadByte();
            if (newObj.numcolors == 0) {
                newObj.numcolors = 256;
            }
            newObj.numcolors *= 8;
            return newObj;
        }

        public void contributeToTreeNode(TreeNode node) {
            node.Nodes.Add("subID = " + subID);
            node.Nodes.Add("offset = " + offset);
            node.Nodes.Add("numcolors = " + numcolors);
        }
    }

    public class TextureMapChange {
        public byte part_index;
        public uint old_tex_id;
        public uint new_tex_id;

        public static TextureMapChange read(BinaryReader binaryReader) {
            TextureMapChange newObj = new TextureMapChange();
            newObj.part_index = binaryReader.ReadByte();
            newObj.old_tex_id = Util.readDataIDOfKnownType(0x5000000, binaryReader);
            newObj.new_tex_id = Util.readDataIDOfKnownType(0x5000000, binaryReader);
            return newObj;
        }

        public void contributeToTreeNode(TreeNode node) {
            node.Nodes.Add("part_index = " + part_index);
            node.Nodes.Add("old_tex_id = " + old_tex_id);
            node.Nodes.Add("new_tex_id = " + new_tex_id);
        }
    }

    public class AnimPartChange {
        public byte part_index;
        public uint part_id;

        public static AnimPartChange read(BinaryReader binaryReader) {
            AnimPartChange newObj = new AnimPartChange();
            newObj.part_index = binaryReader.ReadByte();
            newObj.part_id = Util.readDataIDOfKnownType(0x1000000, binaryReader);
            return newObj;
        }

        public void contributeToTreeNode(TreeNode node) {
            node.Nodes.Add("part_index = " + part_index);
            node.Nodes.Add("part_id = " + part_id);
        }
    }

    public class ObjDesc {
        public uint paletteID;
        public List<Subpalette> subpalettes = new List<Subpalette>(); // NOTE: This should be sorted on insertion or something, see ObjDesc::AddSubpalette
        public List<TextureMapChange> tmChanges = new List<TextureMapChange>();
        public List<AnimPartChange> apChanges = new List<AnimPartChange>();

        public static ObjDesc read(BinaryReader binaryReader) {
            ObjDesc newObj = new ObjDesc();
            binaryReader.ReadByte(); // Unk, should always be == 17

            byte numPalettes = binaryReader.ReadByte();
            byte numTMCs = binaryReader.ReadByte();
            byte numAPCs = binaryReader.ReadByte();

            if (numPalettes > 0) {
                newObj.paletteID = Util.readDataIDOfKnownType(0x4000000, binaryReader);
                for (int i = 0; i < numPalettes; ++i) {
                    newObj.subpalettes.Add(Subpalette.read(binaryReader));
                }
            }

            if (numTMCs > 0) {
                for (int i = 0; i < numTMCs; ++i) {
                    newObj.tmChanges.Add(TextureMapChange.read(binaryReader));
                }
            }

            if (numAPCs > 0) {
                for (int i = 0; i < numAPCs; ++i) {
                    newObj.apChanges.Add(AnimPartChange.read(binaryReader));
                }
            }

            Util.readToAlign(binaryReader);

            return newObj;
        }

        public void contributeToTreeNode(TreeNode node) {
            TreeNode paletteNode = node.Nodes.Add("subpalettes = ");
            foreach (Subpalette subpalette in subpalettes) {
                subpalette.contributeToTreeNode(paletteNode);
            }
            TreeNode textureNode = node.Nodes.Add("tmChanges = ");
            foreach (TextureMapChange tmChange in tmChanges) {
                tmChange.contributeToTreeNode(textureNode);
            }
            TreeNode animpartNode = node.Nodes.Add("apChanges = ");
            foreach (AnimPartChange apChange in apChanges) {
                apChange.contributeToTreeNode(animpartNode);
            }
        }
    }

    public class ChildInfo { // Not actually a class in client, but is here for convenience
        public uint id;
        public uint location_id;

        public static ChildInfo read(BinaryReader binaryReader) {
            ChildInfo newObj = new ChildInfo();
            newObj.id = binaryReader.ReadUInt32();
            newObj.location_id = binaryReader.ReadUInt32();
            return newObj;
        }

        public void contributeToTreeNode(TreeNode node) {
            node.Nodes.Add("id = " + id);
            node.Nodes.Add("location_id = " + location_id);
        }
    }

    public class PhysicsDesc {
        enum PhysicsDescInfo {
            CSetup = (1 << 0),
            MTABLE = (1 << 1),
            VELOCITY = (1 << 2),
            ACCELERATION = (1 << 3),
            OMEGA = (1 << 4),
            PARENT = (1 << 5),
            CHILDREN = (1 << 6),
            OBJSCALE = (1 << 7),
            FRICTION = (1 << 8),
            ELASTICITY = (1 << 9),
            TIMESTAMPS = (1 << 10),
            STABLE = (1 << 11),
            PETABLE = (1 << 12),
            DEFAULT_SCRIPT = (1 << 13),
            DEFAULT_SCRIPT_INTENSITY = (1 << 14),
            POSITION = (1 << 15),
            MOVEMENT = (1 << 16),
            ANIMFRAME_ID = (1 << 17),
            TRANSLUCENCY = (1 << 18)
        }

        public uint bitfield;
        public uint state;
        public byte[] movement_buffer;
        public uint autonomous_movement;
        public uint animframe_id;
        public Position pos = new Position();
        public uint mtable_id; // These are tag ids like animpartchange
        public uint stable_id;
        public uint phstable_id;
        public uint setup_id;
        public uint parent_id;
        public uint location_id;
        public List<ChildInfo> children = new List<ChildInfo>();
        public float object_scale;
        public float friction;
        public float elasticity;
        public float translucency;
        public Vector3 velocity = new Vector3();
        public Vector3 acceleration = new Vector3();
        public Vector3 omega = new Vector3();
        public PScriptType default_script;
        public float default_script_intensity;
        public ushort[] timestamps = new ushort[9];

        public static PhysicsDesc read(BinaryReader binaryReader) {
            PhysicsDesc newObj = new PhysicsDesc();
            newObj.bitfield = binaryReader.ReadUInt32();
            newObj.state = binaryReader.ReadUInt32();

            if ((newObj.bitfield & (uint)PhysicsDescInfo.MOVEMENT) != 0) {
                uint buff_length = binaryReader.ReadUInt32();
                newObj.movement_buffer = binaryReader.ReadBytes((int)buff_length);
                newObj.autonomous_movement = binaryReader.ReadUInt32();
            } else if ((newObj.bitfield & (uint)PhysicsDescInfo.ANIMFRAME_ID) != 0) {
                newObj.animframe_id = binaryReader.ReadUInt32();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.POSITION) != 0) {
                newObj.pos = Position.read(binaryReader);
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.MTABLE) != 0) {
                newObj.mtable_id = binaryReader.ReadUInt32();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.STABLE) != 0) {
                newObj.stable_id = binaryReader.ReadUInt32();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.PETABLE) != 0) {
                newObj.phstable_id = binaryReader.ReadUInt32();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.CSetup) != 0) {
                newObj.setup_id = binaryReader.ReadUInt32();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.PARENT) != 0) {
                newObj.parent_id = binaryReader.ReadUInt32();
                newObj.location_id = binaryReader.ReadUInt32();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.CHILDREN) != 0) {
                uint num_children = binaryReader.ReadUInt32();
                for (int i = 0; i < num_children; ++i) {
                    newObj.children.Add(ChildInfo.read(binaryReader));
                }
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.OBJSCALE) != 0) {
                newObj.object_scale = binaryReader.ReadSingle();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.FRICTION) != 0) {
                newObj.friction = binaryReader.ReadSingle();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.ELASTICITY) != 0) {
                newObj.elasticity = binaryReader.ReadSingle();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.TRANSLUCENCY) != 0) {
                newObj.translucency = binaryReader.ReadSingle();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.VELOCITY) != 0) {
                newObj.velocity.x = binaryReader.ReadSingle();
                newObj.velocity.y = binaryReader.ReadSingle();
                newObj.velocity.z = binaryReader.ReadSingle();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.ACCELERATION) != 0) {
                newObj.acceleration.x = binaryReader.ReadSingle();
                newObj.acceleration.y = binaryReader.ReadSingle();
                newObj.acceleration.z = binaryReader.ReadSingle();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.OMEGA) != 0) {
                newObj.omega.x = binaryReader.ReadSingle();
                newObj.omega.y = binaryReader.ReadSingle();
                newObj.omega.z = binaryReader.ReadSingle();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.DEFAULT_SCRIPT) != 0) {
                newObj.default_script = (PScriptType)binaryReader.ReadUInt32();
            }

            if ((newObj.bitfield & (uint)PhysicsDescInfo.DEFAULT_SCRIPT_INTENSITY) != 0) {
                newObj.default_script_intensity = binaryReader.ReadSingle();
            }

            for (int i = 0; i < newObj.timestamps.Length; ++i) {
                newObj.timestamps[i] = binaryReader.ReadUInt16();
            }

            Util.readToAlign(binaryReader);

            return newObj;
        }

        public void contributeToTreeNode(TreeNode node) {
            node.Nodes.Add("bitfield = " + bitfield);
            node.Nodes.Add("state = " + state);
            node.Nodes.Add("movement_buffer = " + movement_buffer);
            node.Nodes.Add("autonomous_movement = " + autonomous_movement);
            node.Nodes.Add("animframe_id = " + animframe_id);
            TreeNode posNode = node.Nodes.Add("pos = ");
            pos.contributeToTreeNode(posNode);
            node.Nodes.Add("mtable_id = " + mtable_id);
            node.Nodes.Add("stable_id = " + stable_id);
            node.Nodes.Add("phstable_id = " + phstable_id);
            node.Nodes.Add("setup_id = " + setup_id);
            node.Nodes.Add("parent_id = " + parent_id);
            node.Nodes.Add("location_id = " + location_id);
            TreeNode childrenNode = node.Nodes.Add("children = ");
            foreach (ChildInfo childInfo in children) {
                childInfo.contributeToTreeNode(childrenNode);
            }
            node.Nodes.Add("object_scale = " + object_scale);
            node.Nodes.Add("friction = " + friction);
            node.Nodes.Add("elasticity = " + elasticity);
            node.Nodes.Add("translucency = " + translucency);
            node.Nodes.Add("velocity = " + velocity);
            node.Nodes.Add("acceleration = " + acceleration);
            node.Nodes.Add("omega = " + omega);
            node.Nodes.Add("default_script = " + default_script);
            node.Nodes.Add("default_script_intensity = " + default_script_intensity);
            TreeNode timestampsNode = node.Nodes.Add("timestamps = ");
            for (int i = 0; i < timestamps.Length; ++i) {
                timestampsNode.Nodes.Add(timestamps[i].ToString());
            }
        }
    }

    public class RestrictionDB {
        // TODO: wew this looks complicated...
    }

    public class PublicWeenieDesc {
        public enum PublicWeenieDescPackHeader {
            PWD_Packed_None = 0,
            PWD_Packed_PluralName = (1 << 0),
            PWD_Packed_ItemsCapacity = (1 << 1),
            PWD_Packed_ContainersCapacity = (1 << 2),
            PWD_Packed_Value = (1 << 3),
            PWD_Packed_Useability = (1 << 4),
            PWD_Packed_UseRadius = (1 << 5),
            PWD_Packed_Monarch = (1 << 6),
            PWD_Packed_UIEffects = (1 << 7),
            PWD_Packed_AmmoType = (1 << 8),
            PWD_Packed_CombatUse = (1 << 9),
            PWD_Packed_Structure = (1 << 10),
            PWD_Packed_MaxStructure = (1 << 11),
            PWD_Packed_StackSize = (1 << 12),
            PWD_Packed_MaxStackSize = (1 << 13),
            PWD_Packed_ContainerID = (1 << 14),
            PWD_Packed_WielderID = (1 << 15),
            PWD_Packed_ValidLocations = (1 << 16),
            PWD_Packed_Location = (1 << 17),
            PWD_Packed_Priority = (1 << 18),
            PWD_Packed_TargetType = (1 << 19),
            PWD_Packed_BlipColor = (1 << 20),
            PWD_Packed_Burden = (1 << 21), // NOTE: May be PWD_Packed_VendorClassID
            PWD_Packed_SpellID = (1 << 22),
            PWD_Packed_RadarEnum = (1 << 23), // NOTE: May be PWD_Packed_RadarDistance
            PWD_Packed_Workmanship = (1 << 24),
            PWD_Packed_HouseOwner = (1 << 25),
            PWD_Packed_HouseRestrictions = (1 << 26),
            PWD_Packed_PScript = (1 << 27),
            PWD_Packed_HookType = (1 << 28),
            PWD_Packed_HookItemTypes = (1 << 29),
            PWD_Packed_IconOverlay = (1 << 30),
            PWD_Packed_MaterialType = (1 << 31)
        }

        public enum PublicWeenieDescPackHeader2 {
            PWD2_Packed_None = 0,
            PWD2_Packed_IconUnderlay = (1 << 0),
            PWD2_Packed_CooldownID = (1 << 1),
            PWD2_Packed_CooldownDuration = (1 << 2),
            PWD2_Packed_PetOwner = (1 << 3),
        }

        public enum BitfieldIndex {
            BF_OPENABLE = (1 << 0),
            BF_INSCRIBABLE = (1 << 1),
            BF_STUCK = (1 << 2),
            BF_PLAYER = (1 << 3),
            BF_ATTACKABLE = (1 << 4),
            BF_PLAYER_KILLER = (1 << 5),
            BF_HIDDEN_ADMIN = (1 << 6),
            BF_UI_HIDDEN = (1 << 7),
            BF_BOOK = (1 << 8),
            BF_VENDOR = (1 << 9),
            BF_PKSWITCH = (1 << 10),
            BF_NPKSWITCH = (1 << 11),
            BF_DOOR = (1 << 12),
            BF_CORPSE = (1 << 13),
            BF_LIFESTONE = (1 << 14),
            BF_FOOD = (1 << 15),
            BF_HEALER = (1 << 16),
            BF_LOCKPICK = (1 << 17),
            BF_PORTAL = (1 << 18),
            // NOTE: Skip 1
            BF_ADMIN = (1 << 20),
            BF_FREE_PKSTATUS = (1 << 21),
            BF_IMMUNE_CELL_RESTRICTIONS = (1 << 22),
            BF_REQUIRES_PACKSLOT = (1 << 23),
            BF_RETAINED = (1 << 24),
            BF_PKLITE_PKSTATUS = (1 << 25),
            BF_INCLUDES_SECOND_HEADER = (1 << 26),
            BF_BINDSTONE = (1 << 27),
            BF_VOLATILE_RARE = (1 << 28),
            BF_WIELD_ON_USE = (1 << 29),
            BF_WIELD_LEFT = (1 << 30),
        }

        public uint header;
        public uint header2;
        public PStringChar _name;
        public uint _wcid;
        public uint _iconID;
        public ITEM_TYPE _type;
        public uint _bitfield;
        public PStringChar _plural_name;
        public byte _itemsCapacity;
        public byte _containersCapacity;
        public AMMO_TYPE _ammoType;
        public uint _value;
        public ITEM_USEABLE _useability;
        public float _useRadius;
        public ITEM_TYPE _targetType;
        public uint _effects;
        public byte _combatUse;
        public ushort _structure;
        public ushort _maxStructure;
        public ushort _stackSize;
        public ushort _maxStackSize;
        public uint _containerID;
        public uint _wielderID;
        public uint _valid_locations;
        public uint _location;
        public uint _priority;
        public byte _blipColor;
        public RadarEnum _radar_enum;
        public ushort _pscript;
        public float _workmanship;
        public ushort _burden;
        public ushort _spellID;
        public uint _house_owner_iid;
        public RestrictionDB _db;
        public uint _hook_item_types;
        public uint _monarch;
        public ITEM_TYPE _hook_type;
        public uint _iconOverlayID;
        public uint _iconUnderlayID;
        public uint _material_type;
        public uint _cooldown_id;
        public double _cooldown_duration;
        public uint _pet_owner;

        public static PublicWeenieDesc read(BinaryReader binaryReader) {
            PublicWeenieDesc newObj = new PublicWeenieDesc();
            newObj.header = binaryReader.ReadUInt32();
            newObj._name = PStringChar.read(binaryReader);
            newObj._wcid = Util.readWClassIDCompressed(binaryReader);
            newObj._iconID = Util.readDataIDOfKnownType(0x6000000, binaryReader);
            newObj._type = (ITEM_TYPE)binaryReader.ReadUInt32();
            newObj._bitfield = binaryReader.ReadUInt32();

            Util.readToAlign(binaryReader);

            if ((newObj._bitfield & (uint)BitfieldIndex.BF_INCLUDES_SECOND_HEADER) != 0) {
                newObj.header2 = binaryReader.ReadUInt32();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_PluralName) != 0) {
                newObj._plural_name = PStringChar.read(binaryReader);
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_ItemsCapacity) != 0) {
                newObj._itemsCapacity = binaryReader.ReadByte();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_ContainersCapacity) != 0) {
                newObj._containersCapacity = binaryReader.ReadByte();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_AmmoType) != 0) {
                newObj._ammoType = (AMMO_TYPE)binaryReader.ReadUInt16();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_Value) != 0) {
                newObj._value = binaryReader.ReadUInt32();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_Useability) != 0) {
                newObj._useability = (ITEM_USEABLE)binaryReader.ReadUInt32();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_UseRadius) != 0) {
                newObj._useRadius = binaryReader.ReadSingle();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_TargetType) != 0) {
                newObj._targetType = (ITEM_TYPE)binaryReader.ReadUInt32();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_UIEffects) != 0) {
                newObj._effects = binaryReader.ReadUInt32();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_CombatUse) != 0) {
                newObj._combatUse = binaryReader.ReadByte();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_Structure) != 0) {
                newObj._structure = binaryReader.ReadUInt16();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_MaxStructure) != 0) {
                newObj._maxStructure = binaryReader.ReadUInt16();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_StackSize) != 0) {
                newObj._stackSize = binaryReader.ReadUInt16();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_MaxStackSize) != 0) {
                newObj._maxStackSize = binaryReader.ReadUInt16();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_ContainerID) != 0) {
                newObj._containerID = binaryReader.ReadUInt32();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_WielderID) != 0) {
                newObj._wielderID = binaryReader.ReadUInt32();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_ValidLocations) != 0) {
                newObj._valid_locations = binaryReader.ReadUInt32();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_Location) != 0) {
                newObj._location = binaryReader.ReadUInt32();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_Priority) != 0) {
                newObj._priority = binaryReader.ReadUInt32();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_BlipColor) != 0) {
                newObj._blipColor = binaryReader.ReadByte();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_RadarEnum) != 0) {
                newObj._radar_enum = (RadarEnum)binaryReader.ReadByte();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_PScript) != 0) {
                newObj._pscript = binaryReader.ReadUInt16();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_Workmanship) != 0) {
                newObj._workmanship = binaryReader.ReadSingle();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_Burden) != 0) {
                newObj._burden = binaryReader.ReadUInt16();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_SpellID) != 0) {
                newObj._spellID = binaryReader.ReadUInt16();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_HouseOwner) != 0) {
                newObj._house_owner_iid = binaryReader.ReadUInt32();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_HouseRestrictions) != 0) {
                // TODO: Read here once you get RestrictedDB read finished
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_HookItemTypes) != 0) {
                newObj._hook_item_types = binaryReader.ReadUInt32();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_Monarch) != 0) {
                newObj._monarch = binaryReader.ReadUInt32();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_HookType) != 0) {
                newObj._hook_type = (ITEM_TYPE)binaryReader.ReadUInt16();
            }

            if ((newObj.header & (uint)PublicWeenieDescPackHeader.PWD_Packed_IconOverlay) != 0) {
                newObj._iconOverlayID = Util.readDataIDOfKnownType(0x6000000, binaryReader);
            }

            if ((newObj.header & unchecked((uint)PublicWeenieDescPackHeader.PWD_Packed_MaterialType)) != 0) {
                newObj._material_type = binaryReader.ReadUInt32();
            }

            if ((newObj.header2 & (uint)PublicWeenieDescPackHeader2.PWD2_Packed_IconUnderlay) != 0) {
                newObj._iconUnderlayID = Util.readDataIDOfKnownType(0x6000000, binaryReader);
            }

            if ((newObj.header2 & (uint)PublicWeenieDescPackHeader2.PWD2_Packed_CooldownID) != 0) {
                newObj._cooldown_id = binaryReader.ReadUInt32();
            }

            if ((newObj.header2 & (uint)PublicWeenieDescPackHeader2.PWD2_Packed_CooldownDuration) != 0) {
                newObj._cooldown_duration = binaryReader.ReadDouble();
            }

            if ((newObj.header2 & (uint)PublicWeenieDescPackHeader2.PWD2_Packed_PetOwner) != 0) {
                newObj._pet_owner = binaryReader.ReadUInt32();
            }

            Util.readToAlign(binaryReader);

            return newObj;
        }

        public void contributeToTreeNode(TreeNode node) {
            node.Nodes.Add("header = " + header);
            node.Nodes.Add("header2 = " + header2);
            node.Nodes.Add("_name = " + _name.m_buffer);
            node.Nodes.Add("_wcid = " + _wcid);
            node.Nodes.Add("_iconID = " + _iconID);
            node.Nodes.Add("_type = " + _type);
            node.Nodes.Add("_bitfield = " + _bitfield);
            node.Nodes.Add("_plural_name = " + _plural_name);
            node.Nodes.Add("_itemsCapacity = " + _itemsCapacity);
            node.Nodes.Add("_containersCapacity = " + _containersCapacity);
            node.Nodes.Add("_ammoType = " + _ammoType);
            node.Nodes.Add("_value = " + _value);
            node.Nodes.Add("_useability = " + _useability);
            node.Nodes.Add("_useRadius = " + _useRadius);
            node.Nodes.Add("_targetType = " + _targetType);
            node.Nodes.Add("_effects = " + _effects);
            node.Nodes.Add("_combatUse = " + _combatUse);
            node.Nodes.Add("_structure = " + _structure);
            node.Nodes.Add("_maxStructure = " + _maxStructure);
            node.Nodes.Add("_stackSize = " + _stackSize);
            node.Nodes.Add("_maxStackSize = " + _maxStackSize);
            node.Nodes.Add("_containerID = " + _containerID);
            node.Nodes.Add("_wielderID = " + _wielderID);
            node.Nodes.Add("_valid_locations = " + _valid_locations);
            node.Nodes.Add("_location = " + _location);
            node.Nodes.Add("_priority = " + _priority);
            node.Nodes.Add("_blipColor = " + _blipColor);
            node.Nodes.Add("_radar_enum = " + _radar_enum);
            node.Nodes.Add("_pscript = " + _pscript);
            node.Nodes.Add("_workmanship = " + _workmanship);
            node.Nodes.Add("_burden = " + _burden);
            node.Nodes.Add("_spellID = " + _spellID);
            node.Nodes.Add("_house_owner_iid = " + _house_owner_iid);
            //node.Nodes.Add("_db = " + _db); // TODO: Add once implemented
            node.Nodes.Add("_hook_item_types = " + _hook_item_types);
            node.Nodes.Add("_monarch = " + _monarch);
            node.Nodes.Add("_hook_type = " + _hook_type);
            node.Nodes.Add("_iconOverlayID = " + _iconOverlayID);
            node.Nodes.Add("_iconUnderlayID = " + _iconUnderlayID);
            node.Nodes.Add("_material_type = " + _material_type);
            node.Nodes.Add("_cooldown_id = " + _cooldown_id);
            node.Nodes.Add("_cooldown_duration = " + _cooldown_duration);
            node.Nodes.Add("_pet_owner = " + _pet_owner);
        }
    }

    public class CreateObject : Message {
        public uint object_id;
        public ObjDesc objdesc;
        public PhysicsDesc physicsdesc;
        public PublicWeenieDesc wdesc;

        public static CreateObject read(BinaryReader binaryReader) {
            CreateObject newObj = new CreateObject();
            newObj.object_id = binaryReader.ReadUInt32();
            newObj.objdesc = ObjDesc.read(binaryReader);
            newObj.physicsdesc = PhysicsDesc.read(binaryReader);
            newObj.wdesc = PublicWeenieDesc.read(binaryReader);
            return newObj;
        }

        public override void contributeToTreeView(TreeView treeView) {
            TreeNode rootNode = new TreeNode(this.GetType().Name);
            rootNode.Expand();
            rootNode.Nodes.Add("object_id = " + object_id);
            TreeNode objdescNode = rootNode.Nodes.Add("objdesc = ");
            objdesc.contributeToTreeNode(objdescNode);
            TreeNode physicsdescNode = rootNode.Nodes.Add("physicsdesc = ");
            physicsdesc.contributeToTreeNode(physicsdescNode);
            TreeNode wdescNode = rootNode.Nodes.Add("wdesc = ");
            wdesc.contributeToTreeNode(wdescNode);
            treeView.Nodes.Add(rootNode);
        }
    }
}

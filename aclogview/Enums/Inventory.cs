using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum InventoryRequest {
    IR_NONE,
    IR_MERGE,
    IR_SPLIT,
    IR_MOVE,
    IR_PICK_UP,
    IR_PUT_IN_CONTAINER,
    IR_DROP,
    IR_WIELD,
    IR_VIEW_AS_GROUND_CONTAINER,
    IR_GIVE,
    IR_SHOP_EVENT
}

public enum DropItemFlags {
    DROPITEM_FLAGS_NONE = 0,
    DROPITEM_IS_CONTAINER = (1 << 0),
    DROPITEM_IS_VENDOR = (1 << 1),
    DROPITEM_IS_SHORTCUT = (1 << 2),
    DROPITEM_IS_SALVAGE = (1 << 3),
    DROPITEM_IS_ALIAS = DROPITEM_IS_VENDOR | DROPITEM_IS_SHORTCUT | DROPITEM_IS_SALVAGE // 14
}

public enum INVENTORY_LOC {
    NONE_LOC = 0,
    HEAD_WEAR_LOC = (1 << 0),
    CHEST_WEAR_LOC = (1 << 1),
    ABDOMEN_WEAR_LOC = (1 << 2),
    UPPER_ARM_WEAR_LOC = (1 << 3),
    LOWER_ARM_WEAR_LOC = (1 << 4),
    HAND_WEAR_LOC = (1 << 5),
    UPPER_LEG_WEAR_LOC = (1 << 6),
    LOWER_LEG_WEAR_LOC = (1 << 7),
    FOOT_WEAR_LOC = (1 << 8),
    CHEST_ARMOR_LOC = (1 << 9),
    ABDOMEN_ARMOR_LOC = (1 << 10),
    UPPER_ARM_ARMOR_LOC = (1 << 11),
    LOWER_ARM_ARMOR_LOC = (1 << 12),
    UPPER_LEG_ARMOR_LOC = (1 << 13),
    LOWER_LEG_ARMOR_LOC = (1 << 14),
    NECK_WEAR_LOC = (1 << 15),
    WRIST_WEAR_LEFT_LOC = (1 << 16),
    WRIST_WEAR_RIGHT_LOC = (1 << 17),
    FINGER_WEAR_LEFT_LOC = (1 << 18),
    FINGER_WEAR_RIGHT_LOC = (1 << 19),
    MELEE_WEAPON_LOC = (1 << 20),
    SHIELD_LOC = (1 << 21),
    MISSILE_WEAPON_LOC = (1 << 22),
    MISSILE_AMMO_LOC = (1 << 23),
    HELD_LOC = (1 << 24),
    TWO_HANDED_LOC = (1 << 25),
    TRINKET_ONE_LOC = (1 << 26),
    CLOAK_LOC = (1 << 27),
    SIGIL_ONE_LOC = (1 << 28),
    SIGIL_TWO_LOC = (1 << 29),
    SIGIL_THREE_LOC = (1 << 30),
    CLOTHING_LOC = (1 << 31) | HEAD_WEAR_LOC | CHEST_WEAR_LOC | ABDOMEN_WEAR_LOC | UPPER_ARM_WEAR_LOC | LOWER_ARM_WEAR_LOC | HAND_WEAR_LOC | UPPER_LEG_WEAR_LOC | LOWER_LEG_WEAR_LOC | FOOT_WEAR_LOC, // 134218239
    ARMOR_LOC = CHEST_ARMOR_LOC | ABDOMEN_ARMOR_LOC | UPPER_ARM_ARMOR_LOC | LOWER_ARM_ARMOR_LOC | UPPER_LEG_ARMOR_LOC | LOWER_LEG_ARMOR_LOC, // 32256
    JEWELRY_LOC = NECK_WEAR_LOC | WRIST_WEAR_LEFT_LOC | WRIST_WEAR_RIGHT_LOC | FINGER_WEAR_LEFT_LOC | FINGER_WEAR_RIGHT_LOC | TRINKET_ONE_LOC | CLOAK_LOC | SIGIL_ONE_LOC | SIGIL_TWO_LOC | SIGIL_THREE_LOC, // 2081390592
    WRIST_WEAR_LOC = WRIST_WEAR_LEFT_LOC | WRIST_WEAR_RIGHT_LOC, // 196608
    FINGER_WEAR_LOC = FINGER_WEAR_LEFT_LOC | FINGER_WEAR_RIGHT_LOC, // 786432
    SIGIL_LOC = SIGIL_ONE_LOC | SIGIL_TWO_LOC | SIGIL_THREE_LOC, // 1879048192
    READY_SLOT_LOC = HELD_LOC | TWO_HANDED_LOC | TRINKET_ONE_LOC | CLOAK_LOC | SIGIL_ONE_LOC | SIGIL_TWO_LOC, // 66060288
    WEAPON_LOC = SIGIL_TWO_LOC | TRINKET_ONE_LOC | HELD_LOC, // 38797312
    WEAPON_READY_SLOT_LOC = SIGIL_ONE_LOC | SIGIL_TWO_LOC | TRINKET_ONE_LOC | HELD_LOC, // 55574528
    ALL_LOC = 2147483647,
    CAN_GO_IN_READY_SLOT_LOC = 2147483647
}

public enum DestinationType {
    Undef_DestinationType = 0,
    Contain_DestinationType = (1 << 0),
    Wield_DestinationType = (1 << 1),
    Shop_DestinationType = (1 << 2),
    Treasure_DestinationType = (1 << 3),
    HouseBuy_DestinationType = (1 << 4),
    HouseRent_DestinationType = (1 << 5),
    Checkpoint_DestinationType = Contain_DestinationType | Wield_DestinationType | Shop_DestinationType, // 7
    ContainTreasure_DestinationType = Contain_DestinationType | Treasure_DestinationType, // 9
    WieldTreasure_DestinationType = Wield_DestinationType | Treasure_DestinationType, // 10
    ShopTreasure_DestinationType = Shop_DestinationType | Treasure_DestinationType // 12
}

public enum RegenerationType {
    Undef_RegenerationType = 0,
    Destruction_RegenerationType = (1 << 0),
    PickUp_RegenerationType = (1 << 1),
    Death_RegenerationType = (1 << 2)
}

public enum RegenLocationType {
    Undef_RegenLocationType = 0,
    OnTop_RegenLocationType = (1 << 0),
    Scatter_RegenLocationType = (1 << 1),
    Specific_RegenLocationType = (1 << 2),
    Contain_RegenLocationType = (1 << 3),
    Wield_RegenLocationType = (1 << 4),
    Shop_RegenLocationType = (1 << 5),
    Treasure_RegenLocationType = (1 << 6),
    Checkpoint_RegenLocationType = Contain_RegenLocationType | Wield_RegenLocationType | Shop_RegenLocationType, // 56
    OnTopTreasure_RegenLocationType = OnTop_RegenLocationType | Treasure_RegenLocationType, // 65
    ScatterTreasure_RegenLocationType = Scatter_RegenLocationType | Treasure_RegenLocationType, // 66
    SpecificTreasure_RegenLocationType = Specific_RegenLocationType | Treasure_RegenLocationType, // 68
    ContainTreasure_RegenLocationType = Contain_RegenLocationType | Treasure_RegenLocationType, // 72
    WieldTreasure_RegenLocationType = Wield_RegenLocationType | Treasure_RegenLocationType, // 80
    ShopTreasure_RegenLocationType = Shop_RegenLocationType | Treasure_RegenLocationType // 96
}

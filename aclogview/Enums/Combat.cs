using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum COMBAT_MODE {
    UNDEF_COMBAT_MODE = 0,
    NONCOMBAT_COMBAT_MODE = (1 << 0),
    MELEE_COMBAT_MODE = (1 << 1),
    MISSILE_COMBAT_MODE = (1 << 2),
    MAGIC_COMBAT_MODE = (1 << 3),
    VALID_COMBAT_MODES = NONCOMBAT_COMBAT_MODE | MELEE_COMBAT_MODE | MISSILE_COMBAT_MODE | MAGIC_COMBAT_MODE, // 15
    COMBAT_COMBAT_MODE = MELEE_COMBAT_MODE | MISSILE_COMBAT_MODE | MAGIC_COMBAT_MODE // 14
}

public enum CombatStyle {
    Undef_CombatStyle = 0,
    Unarmed_CombatStyle = (1 << 0),
    OneHanded_CombatStyle = (1 << 1),
    OneHandedAndShield_CombatStyle = (1 << 2),
    TwoHanded_CombatStyle = (1 << 3),
    Bow_CombatStyle = (1 << 4),
    Crossbow_CombatStyle = (1 << 5),
    Sling_CombatStyle = (1 << 6),
    ThrownWeapon_CombatStyle = (1 << 7),
    DualWield_CombatStyle = (1 << 8),
    Magic_CombatStyle = (1 << 9),
    Atlatl_CombatStyle = (1 << 10),
    ThrownShield_CombatStyle = (1 << 11),
    Reserved1_CombatStyle = (1 << 12),
    Reserved2_CombatStyle = (1 << 13),
    Reserved3_CombatStyle = (1 << 14),
    Reserved4_CombatStyle = (1 << 15),
    StubbornMagic_CombatStyle = (1 << 16),
    StubbornProjectile_CombatStyle = (1 << 17),
    StubbornMelee_CombatStyle = (1 << 18),
    StubbornMissile_CombatStyle = (1 << 19),
    Melee_CombatStyles = Unarmed_CombatStyle | OneHanded_CombatStyle | OneHandedAndShield_CombatStyle | TwoHanded_CombatStyle | DualWield_CombatStyle, // 271
    Missile_CombatStyles = Bow_CombatStyle | Crossbow_CombatStyle | Sling_CombatStyle | ThrownWeapon_CombatStyle | Atlatl_CombatStyle | ThrownShield_CombatStyle, // 3312
    Magic_CombatStyles = Magic_CombatStyle, // 512
    All_CombatStyle = 65535
}

public enum BODY_HEIGHT {
    UNDEF_BODY_HEIGHT,
    HIGH_BODY_HEIGHT,
    MEDIUM_BODY_HEIGHT,
    LOW_BODY_HEIGHT,
    NUM_BODY_HEIGHTS
}

public enum DAMAGE_TYPE {
    UNDEF_DAMAGE_TYPE = 0,
    SLASH_DAMAGE_TYPE = (1 << 0),
    PIERCE_DAMAGE_TYPE = (1 << 1),
    BLUDGEON_DAMAGE_TYPE = (1 << 2),
    COLD_DAMAGE_TYPE = (1 << 3),
    FIRE_DAMAGE_TYPE = (1 << 4),
    ACID_DAMAGE_TYPE = (1 << 5),
    ELECTRIC_DAMAGE_TYPE = (1 << 6),
    HEALTH_DAMAGE_TYPE = (1 << 7),
    STAMINA_DAMAGE_TYPE = (1 << 8),
    MANA_DAMAGE_TYPE = (1 << 9),
    NETHER_DAMAGE_TYPE = (1 << 10),
    // NOTE: Skip 17
    BASE_DAMAGE_TYPE = (1 << 28),
}

public enum AttackType {
    Undef_AttackType = 0,
    Punch_AttackType = (1 << 0),
    Thrust_AttackType = (1 << 1),
    Slash_AttackType = (1 << 2),
    Kick_AttackType = (1 << 3),
    OffhandPunch_AttackType = (1 << 4),
    DoubleSlash_AttackType = (1 << 5),
    TripleSlash_AttackType = (1 << 6),
    DoubleThrust_AttackType = (1 << 7),
    TripleThrust_AttackType = (1 << 8),
    OffhandThrust_AttackType = (1 << 9),
    OffhandSlash_AttackType = (1 << 10),
    OffhandDoubleSlash_AttackType = (1 << 11),
    OffhandTripleSlash_AttackType = (1 << 12),
    OffhandDoubleThrust_AttackType = (1 << 13),
    OffhandTripleThrust_AttackType = (1 << 14),
    Unarmed_AttackType = Punch_AttackType | Kick_AttackType | OffhandPunch_AttackType, // 25
    MultiStrike_AttackType = DoubleSlash_AttackType | TripleSlash_AttackType | DoubleThrust_AttackType | TripleThrust_AttackType | OffhandDoubleSlash_AttackType | OffhandTripleSlash_AttackType | OffhandDoubleThrust_AttackType | OffhandTripleThrust_AttackType // 31200
}

public enum eCombatMode {
    eCombatModeUndef = 0,
    eCombatModeNonCombat = (1 << 0),
    eCombatModeMelee = (1 << 1),
    eCombatModeMissile = (1 << 2),
    eCombatModeMagic = (1 << 3)
}

public enum ATTACK_HEIGHT {
    UNDEF_ATTACK_HEIGHT,
    HIGH_ATTACK_HEIGHT,
    MEDIUM_ATTACK_HEIGHT,
    LOW_ATTACK_HEIGHT,
    NUM_ATTACK_HEIGHTS
}

public enum PowerBarMode {
    PBM_UNDEF,
    PBM_COMBAT,
    PBM_ADVANCED_COMBAT,
    PBM_JUMP,
    PBM_DDD
}

public enum BodyPart
{
    UNDEFINED = -1,
    HEAD = 0,
    CHEST = 1,
    ABDOMEN = 2,
    UPPER_ARM = 3,
    LOWER_ARM = 4,
    HAND = 5,
    UPPER_LEG = 6,
    LOWER_LEG = 7,
    FOOT = 8,
    HORN = 9,
    FRONT_LEG = 10,
    // Skip 11
    FRONT_FOOT = 12,
    REAR_LEG = 13,
    // Skip 14
    REAR_FOOT = 15,
    TORSO = 16,
    TAIL = 17,
    ARM = 18,
    LEG = 19,
    CLAW = 20,
    WINGS = 21,
    BREATH = 22,
    TENTACLE = 23,
    UPPER_TENTACLE = 24,
    LOWER_TENTACLE = 25,
    CLOAK = 26,
    NUM = 27
}

public enum AttackConditions
{
    // Could not find these defined as constants, so came up with appropriate names
    CriticalAugPreventedCritical = (1 << 0), // "Your Critical Protection augmentation allows you to avoid a critical hit!"
    Reckless = (1 << 1), // "Reckless!"
    SneakAttack = (1 << 2), // "Sneak Attack!"
}


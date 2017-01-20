using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum PhysicsState {
    STATIC_PS = (1 << 0),
    UNUSED1_PS = (1 << 1),
    ETHEREAL_PS = (1 << 2),
    REPORT_COLLISIONS_PS = (1 << 3),
    IGNORE_COLLISIONS_PS = (1 << 4),
    NODRAW_PS = (1 << 5),
    MISSILE_PS = (1 << 6),
    PUSHABLE_PS = (1 << 7),
    ALIGNPATH_PS = (1 << 8),
    PATHCLIPPED_PS = (1 << 9),
    GRAVITY_PS = (1 << 10),
    LIGHTING_ON_PS = (1 << 11),
    PARTICLE_EMITTER_PS = (1 << 12),
    UNNUSED2_PS = (1 << 13),
    HIDDEN_PS = (1 << 14),
    SCRIPTED_COLLISION_PS = (1 << 15),
    HAS_PHYSICS_BSP_PS = (1 << 16),
    INELASTIC_PS = (1 << 17),
    HAS_DEFAULT_ANIM_PS = (1 << 18),
    HAS_DEFAULT_SCRIPT_PS = (1 << 19),
    CLOAKED_PS = (1 << 20),
    REPORT_COLLISIONS_AS_ENVIRONMENT_PS = (1 << 21),
    EDGE_SLIDE_PS = (1 << 22),
    SLEDDING_PS = (1 << 23),
    FROZEN_PS = (1 << 24)
}

namespace PhysicsObjHook {
    public enum HookType {
        SCALING,
        TRANSLUCENCY,
        PART_TRANSLUCENCY,
        LUMINOSITY,
        DIFFUSION,
        PART_LUMINOSITY,
        PART_DIFFUSION,
        CALL_PES
    }
}

public enum HookTypeEnum {
    Undef_HookTypeEnum = 0,
    Floor_HookTypeEnum = (1 << 0),
    Wall_HookTypeEnum = (1 << 1),
    Ceiling_HookTypeEnum = (1 << 2),
    Yard_HookTypeEnum = (1 << 3),
    Roof_HookTypeEnum = (1 << 4)
}

public enum PhysicsTimeStamp {
    POSITION_TS,
    MOVEMENT_TS,
    STATE_TS,
    VECTOR_TS,
    TELEPORT_TS,
    SERVER_CONTROLLED_MOVE_TS,
    FORCE_POSITION_TS,
    OBJDESC_TS,
    INSTANCE_TS,
    NUM_PHYSICS_TS
}

public enum SetPositionError {
    OK_SPE,
    GENERAL_FAILURE_SPE,
    NO_VALID_POSITION_SPE,
    NO_CELL_SPE,
    COLLIDED_SPE,
    INVALID_ARGUMENTS = 256
}

public enum SetPositionFlag {
    PLACEMENT_SPF = (1 << 0),
    TELEPORT_SPF = (1 << 1),
    RESTORE_SPF = (1 << 2),
    // NOTE: Skip 1
    SLIDE_SPF = (1 << 4),
    DONOTCREATECELLS_SPF = (1 << 5),
    // NOTE: Skip 2
    SCATTER_SPF = (1 << 8),
    RANDOMSCATTER_SPF = (1 << 9),
    LINE_SPF = (1 << 10),
    // NOTE: Skip 1
    SEND_POSITION_EVENT_SPF = (1 << 12)
}

public enum ObjCollisionProfile_Bitfield {
    Undef_ECPB = 0,
    Creature_OCPB = (1 << 0),
    Player_OCPB = (1 << 1),
    Attackable_OCPB = (1 << 2),
    Missile_OCPB = (1 << 3),
    Contact_OCPB = (1 << 4),
    MyContact_OCPB = (1 << 5),
    Door_OCPB = (1 << 6),
    Cloaked_OCPB = (1 << 7)
}

public enum EnvCollisionProfile_Bitfield {
    Undef_ECPB = 0,
    MyContact_ECPB = (1 << 0)
}

public enum TransientState {
    CONTACT_TS = (1 << 0),
    ON_WALKABLE_TS = (1 << 1),
    SLIDING_TS = (1 << 2),
    WATER_CONTACT_TS = (1 << 3),
    STATIONARY_FALL_TS = (1 << 4),
    STATIONARY_STOP_TS = (1 << 5),
    STATIONARY_STUCK_TS = (1 << 6),
    ACTIVE_TS = (1 << 7),
    CHECK_ETHEREAL_TS = (1 << 8)
}

public enum TransitionState {
    INVALID_TS,
    OK_TS,
    COLLIDED_TS,
    ADJUSTED_TS,
    SLID_TS
}

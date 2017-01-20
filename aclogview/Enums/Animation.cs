using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAnimHook {
    public enum AnimHookDir {
        UNKNOWN_ANIMHOOK = -2,
        BACKWARD_ANIMHOOK = -1,
        BOTH_ANIMHOOK = 0,
        FORWARD_ANIMHOOK = 1
    }

    public enum HookType__guessedname {
        NOOP_HOOK,
        SOUND_HOOK,
        SOUND_TABLE_HOOK,
        ATTACK_HOOK,
        ANIMDONE_HOOK,
        REPLACE_OBJECT_HOOK,
        ETHEREAL_HOOK,
        TRANSPARENT_PART_HOOK,
        LUMINOUS_HOOK,
        LUMINOUS_PART_HOOK,
        DIFFUSE_HOOK,
        DIFFUSE_PART_HOOK,
        SCALE_HOOK,
        CREATE_PARTICLE_HOOK,
        DESTROY_PARTICLE_HOOK,
        STOP_PARTICLE_HOOK,
        NODRAW_HOOK,
        DEFAULT_SCRIPT_HOOK,
        DEFAULT_SCRIPT_PART_HOOK,
        CALL_PES_HOOK,
        TRANSPARENT_HOOK,
        SOUND_TWEAKED_HOOK,
        SET_OMEGA_HOOK,
        TEXTURE_VELOCITY_HOOK,
        TEXTURE_VELOCITY_PART_HOOK,
        SET_LIGHT_HOOK,
        CREATE_BLOCKING_PARTICLE_HOOK,
        UNKNOWN_HOOK = -1
    }
}

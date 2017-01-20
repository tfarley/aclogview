using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ProgramType {
    Undef_ProgramType,
    Client_ProgramType,
    Server_ProgramType,
    GameClient_ProgramType = 1073741825,
    GameServer_ProgramType = 1073741825,
    ClientAdder_ProgramType = -2147483647,
    ServerAdder_ProgramType = -2147483647,
    WorldBuilder_ProgramType = -2147483647,
    RuntimeProgram_ProgramType = 1073741825,
    PreprocProgram_ProgramType = -2147483647,
    PathMap_ProgramType = 256,
    PathGen_ProgramType = -2147483647
}

public enum DetectionType {
    NoChangeDetection,
    EnteredDetection,
    LeftDetection
}

public enum Parts {
    Q_X,
    Q_Y,
    Q_Z,
    Q_W
}

public enum CameraTarget {
    INVALID_TARGET = 0,
    LOOK_IN_DIRECTION = (1 << 0),
    LOOK_AT_OBJECT = (1 << 1),
    LOOK_AT_PIVOT = (1 << 2),
    ALIGN_WITH_PIVOT = (1 << 3),
    ALIGN_WITH_PLANE = (1 << 4)
}

public enum CG_VERIFICATION_RESPONSE {
    UNDEF_CG_VERIFICATION_RESPONSE,
    CG_VERIFICATION_RESPONSE_OK,
    CG_VERIFICATION_RESPONSE_PENDING,
    CG_VERIFICATION_RESPONSE_NAME_IN_USE,
    CG_VERIFICATION_RESPONSE_NAME_BANNED,
    CG_VERIFICATION_RESPONSE_CORRUPT,
    CG_VERIFICATION_RESPONSE_DATABASE_DOWN,
    CG_VERIFICATION_RESPONSE_ADMIN_PRIVILEGE_DENIED,
    NUM_CG_VERIFICATION_RESPONSES
}

public enum NameType {
    NAME_SINGULAR,
    NAME_PLURAL,
    NAME_APPROPRIATE
}


public enum ExperienceHandlingType {
    Undef_ExperienceHandlingType = 0,
    ApplyLevelMod_ExperienceHandlingType = (1 << 0),
    ShareWithFellows_ExperienceHandlingType = (1 << 1),
    AddFellowshipBonus_ExperienceHandlingType = (1 << 2),
    ShareWithAllegiance_ExperienceHandlingType = (1 << 3),
    ApplyToVitae_ExperienceHandlingType = (1 << 4),
    EarnsCP_ExperienceHandlingType = (1 << 5),
    ReducedByDistance_ExperienceHandlingType = (1 << 6),
    Monster_ExperienceHandlingType = ApplyLevelMod_ExperienceHandlingType | ShareWithFellows_ExperienceHandlingType | AddFellowshipBonus_ExperienceHandlingType | ShareWithAllegiance_ExperienceHandlingType | ApplyToVitae_ExperienceHandlingType | ReducedByDistance_ExperienceHandlingType, // 95
    NormalQuest_ExperienceHandlingType = ShareWithAllegiance_ExperienceHandlingType | ApplyToVitae_ExperienceHandlingType | ShareWithFellows_ExperienceHandlingType, // 26
    NoShareQuest_ExperienceHandlingType = ApplyToVitae_ExperienceHandlingType, // 16
    PassupQuest_ExperienceHandlingType = ShareWithAllegiance_ExperienceHandlingType | ApplyToVitae_ExperienceHandlingType, // 24
    ReceivedFromFellowship_ExperienceHandlingType = ShareWithAllegiance_ExperienceHandlingType | ApplyToVitae_ExperienceHandlingType, // 24
    PPEarnedFromUse_ExperienceHandlingType = ApplyLevelMod_ExperienceHandlingType | ShareWithFellows_ExperienceHandlingType | AddFellowshipBonus_ExperienceHandlingType | ShareWithAllegiance_ExperienceHandlingType | ApplyToVitae_ExperienceHandlingType | EarnsCP_ExperienceHandlingType | ReducedByDistance_ExperienceHandlingType, // 127
    AdminRaiseXP_ExperienceHandlingType = ApplyToVitae_ExperienceHandlingType, // 16
    AdminRaiseSkillXP_ExperienceHandlingType = ApplyToVitae_ExperienceHandlingType, // 16
    ReceivedFromAllegiance_ExperienceHandlingType = Undef_ExperienceHandlingType, // 0
}

public enum ComponentTrackerUpdate {
    CT_CHANGE_NONE,
    CT_CHANGE_ADD,
    CT_CHANGE_REMOVE
}

public enum ContractSortCriteria {
    eName,
    eStatus
}

public enum JournalSortCriteria {
    ePageNumber,
    eTitle,
    eLabel,
    eTimer
}

public enum FriendsUpdateType {
    FRIENDS_UPDATE,
    FRIENDS_UPDATE_ADD,
    FRIENDS_UPDATE_REMOVE,
    FRIENDS_UPDATE_REMOVE_SILENT,
    FRIENDS_UPDATE_ONLINE_STATUS
}

public enum TargetStatus {
    Undef_TargetStatus,
	Ok_TargetStatus,
	ExitWorld_TargetStatus,
	Teleported_TargetStatus,
	Contained_TargetStatus,
	Parented_TargetStatus,
	TimedOut_TargetStatus
}

public enum LandChangeType {
    CHANGE_LAND_HEIGHT,
    CHANGE_LAND_ROAD,
    CHANGE_LAND_TERRAIN,
    CHANGE_LAND_SCENE,
    CHANGE_LAND_ENCOUNTER
}

public enum InterpolationNode {
    INVALID_TYPE,
    POSITION_TYPE,
    JUMP_TYPE,
    VELOCITY_TYPE
}

public enum EnchantmentRegistryPackHeader {
    Packed_None = 0,
    Packed_MultList = (1 << 0),
    Packed_AddList = (1 << 1),
    Packed_Vitae = (1 << 2),
    Packed_Cooldown = (1 << 3)
}

namespace OldPublicWeenieDesc {
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
    }
}

public enum HookGroupDataVersion {
    Undef_HookGroupDataVersion,
    Initial_HookGroupDataVersion,
    Current_HookGroupDataVersion = Initial_HookGroupDataVersion
}

public enum CharCase {
    CASE_UPPER,
    CASE_LOWER,
    CASE_EITHER
}

public enum EnterChargen {
    ENTER_NEW,
    ENTER_CONTINUE,
    ENTER_RANDOM
}

public enum ExperienceType {
    Undef_ExperienceType,
	Attribute_ExperienceType,
	Attribute2nd_ExperienceType,
	TrainedSkill_ExperienceType,
	SpecializedSkill_ExperienceType,
	Level_ExperienceType,
	Credit_ExperienceType
}

public enum ATTRIBUTE_CACHE_MASK {
    UNDEF_MASK = 0,
    STRENGTH_MASK = (1 << 0),
    ENDURANCE_MASK = (1 << 1),
    QUICKNESS_MASK = (1 << 2),
    COORDINATION_MASK = (1 << 3),
    FOCUS_MASK = (1 << 4),
    SELF_MASK = (1 << 5),
    HEALTH_MASK = (1 << 6),
    STAMINA_MASK = (1 << 7),
    MANA_MASK = (1 << 8)
}

public enum SecurityLevelEnum {
    Undef_SecurityLevel,
    Advocate1_SecurityLevel,
    Advocate2_SecurityLevel,
    Advocate3_SecurityLevel,
    Advocate4_SecurityLevel,
    Advocate5_SecurityLevel,
    Sentinel1_SecurityLevel,
    Sentinel2_SecurityLevel,
    Sentinel3_SecurityLevel,
    Turbine_SecurityLevel,
    Arch_SecurityLevel,
    Admin_SecurityLevel,
    Player_SecurityLevel = Undef_SecurityLevel,
    MaxAdvocate_SecurityLevel = Advocate5_SecurityLevel,
    MaxSentinel_SecurityLevel = Sentinel3_SecurityLevel,
    Max_SecurityLevel = Admin_SecurityLevel
}

public enum GeneratorTimeType {
    Undef_GeneratorTimeType,
    RealTime_GeneratorTimeType,
    Defined_GeneratorTimeType,
    Event_GeneratorTimeType,
    Night_GeneratorTimeType,
    Day_GeneratorTimeType
}

public enum GeneratorDefinedTimes {
    Undef_GeneratorDefinedTimes,
    Dusk_GeneratorDefinedTimes,
    Dawn_GeneratorDefinedTimes
}

public enum GeneratorType {
    Undef_GeneratorType,
    Relative_GeneratorType,
    Absolute_GeneratorType
}

public enum GeneratorDestruct {
    Undef_GeneratorDestruct,
    Nothing_GeneratorDestruct,
    Destroy_GeneratorDestruct,
    Kill_GeneratorDestruct
}

public enum PackItVersionEnum {
    Undef_PackItVersionEnum,
	Original_GeneratorTimeType,
	VersionTwo_GeneratorTimeType,
	NewestVersion_PackItVersionEnum = VersionTwo_GeneratorTimeType
}

namespace Heading {
    public enum unit_type {
        DEGREES,
        RADIANS
    }
}

public enum PropertyDatFileType {
    ClientOnlyData,
    ServerOnlyData,
    SharedData
}

public enum PropertyPropagationType {
    NetPredictedSharedVisually,
	NetPredictedSharedPrivately,
	NetSharedVisually,
	NetSharedPrivately,
	NetNotShared,
	WorldSharedWithServers,
	WorldSharedWithServersAndClients
}

public enum PropertyCachingType {
    Global,
    Internal
}

public enum DATFILE_TYPE {
    UNDEF_DISK,
	PORTAL_DATFILE,
	CELL_DATFILE,
	LOCAL_DATFILE
}

public enum WaveformType {
    WAVEFORM_INVALID,
    WAVEFORM_NONE,
    WAVEFORM_SPEED,
    WAVEFORM_NOISE,
    WAVEFORM_SINE,
    WAVEFORM_SQUARE,
    WAVEFORM_BOUNCE,
    WAVEFORM_PERLIN,
    WAVEFORM_FRACTAL,
    WAVEFORM_FRAMELOOP,
    NUM_WAVEFORMS
}

public enum DDDEvent {
    DDD_PatchtimeInterrogation,
    DDD_RegionSet,
    DDD_PatchtimePending,
    DDD_PatchtimeBegin,
    DDD_PatchtimeEnd,
    DDD_DataDownloaded,
    DDD_DynamicDataRequested,
    DDD_DataError
}

public enum GRVDataType {
    GRVDataType_Unknown,
    GRVDataType_DataID,
    GRVDataType_Bool,
    GRVDataType_Int32,
    GRVDataType_UInt32,
    GRVDataType_Int16,
    GRVDataType_UInt16,
    GRVDataType_Int8,
    GRVDataType_UInt8,
    GRVDataType_Float32,
    GRVDataType_Float64,
    GRVDataType_Vector3,
    GRVDataType_RGBAColor,
    GRVDataType_PString,
    GRVDataType_Waveform
}

public enum Target_Mode {
    TARGET_MODE_NONE,
    TARGET_MODE_USE,
    TARGET_MODE_EXAMINE,
    TARGET_MODE_USE_TARGET
}

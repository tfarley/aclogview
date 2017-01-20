using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum SURFCHAR {
    SOLID,
    WATER
}

namespace LandDefs {
    public enum TerrainType {
        BarrenRock,
        Grassland,
        Ice,
        LushGrass,
        MarshSparseSwamp,
        MudRichDirt,
        ObsidianPlain,
        PackedDirt,
        PatchyDirt,
        PatchyGrassland,
        SandYellow,
        SandGrey,
        SandRockStrewn,
        SedimentaryRock,
        SemiBarrenRock,
        Snow,
        WaterRunning,
        WaterStandingFresh,
        WaterShallowSea,
        WaterShallowStillSea,
        WaterDeepSea,
        Reserved21,
        Reserved22,
        Reserved23,
        Reserved24,
        Reserved25,
        Reserved26,
        Reserved27,
        Reserved28,
        Reserved29,
        Reserved30,
        Reserved31,
        RoadType
    }

    public enum WaterType {
        NOT_WATER,
        PARTIALLY_WATER,
        ENTIRELY_WATER
    }

    public enum PalType {
        SWTerrain,
        SETerrain,
        NETerrain,
        NWTerrain,
        Road
    }

    public enum Rotation {
        ROT_0,
        ROT_90,
        ROT_180,
        ROT_270
    }

    public enum Direction {
        IN_VIEWER_BLOCK,
        NORTH_OF_VIEWER,
        SOUTH_OF_VIEWER,
        EAST_OF_VIEWER,
        WEST_OF_VIEWER,
        NORTHWEST_OF_VIEWER,
        SOUTHWEST_OF_VIEWER,
        NORTHEAST_OF_VIEWER,
        SOUTHEAST_OF_VIEWER,
        UNKNOWN
    }
}

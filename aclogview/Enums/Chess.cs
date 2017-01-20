using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum MoveType {
    MoveType_Invalid,
    MoveType_Pass,
    MoveType_Resign,
    MoveType_Stalemate,
    MoveType_Grid,
    MoveType_FromTo,
    MoveType_SelectedPiece
}

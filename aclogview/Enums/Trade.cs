using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum TradeListIDEnum {
    Undef_TradeListID,
    Self_TradeListID,
    Partner_TradeListID
}

public enum TradeStatusEnum {
    Undef_TradeStatus = 0,
    Pending_TradeStatus = (1 << 0),
    Open_TradeStatus = (1 << 1),
    WaitingToClose_TradeStatus = (1 << 2)
}

public enum eTradeListID {
    TradeListIDUndef,
    TradeListIDSelf,
    TradeListIDPartner
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum charError {
    CHAR_ERROR_UNDEF,
    CHAR_ERROR_LOGON,
    CHAR_ERROR_LOGGED_ON,
    CHAR_ERROR_ACCOUNT_LOGON,
    CHAR_ERROR_SERVER_CRASH,
    CHAR_ERROR_LOGOFF,
    CHAR_ERROR_DELETE,
    CHAR_ERROR_NO_PREMADE,
    CHAR_ERROR_ACCOUNT_IN_USE,
    CHAR_ERROR_ACCOUNT_INVALID,
    CHAR_ERROR_ACCOUNT_DOESNT_EXIST,
    CHAR_ERROR_ENTER_GAME_GENERIC,
    CHAR_ERROR_ENTER_GAME_STRESS_ACCOUNT,
    CHAR_ERROR_ENTER_GAME_CHARACTER_IN_WORLD,
    CHAR_ERROR_ENTER_GAME_PLAYER_ACCOUNT_MISSING,
    CHAR_ERROR_ENTER_GAME_CHARACTER_NOT_OWNED,
    CHAR_ERROR_ENTER_GAME_CHARACTER_IN_WORLD_SERVER,
    CHAR_ERROR_ENTER_GAME_OLD_CHARACTER,
    CHAR_ERROR_ENTER_GAME_CORRUPT_CHARACTER,
    CHAR_ERROR_ENTER_GAME_START_SERVER_DOWN,
    CHAR_ERROR_ENTER_GAME_COULDNT_PLACE_CHARACTER,
    CHAR_ERROR_LOGON_SERVER_FULL,
    CHAR_ERROR_CHARACTER_IS_BOOTED,
    CHAR_ERROR_ENTER_GAME_CHARACTER_LOCKED,
    CHAR_ERROR_SUBSCRIPTION_EXPIRED,
    CHAR_ERROR_NUM_ERRORS
}

public enum ServerSwitchType {
    ssWorldSwitch,
    ssLogonSwitch
}

public enum ReceiverState {
    UNDEF_STATE,
    NAK_STATE,
    NO_NAK_STATE,
    NO_STATE
}

public enum ConnectionState {
    cs_Disconnected,
    cs_AwaitingWorldAuth,
    cs_AuthSent,
    cs_ConnectionRequestSent,
    cs_ConnectionRequestAcked,
    cs_Connected,
    cs_DisconnectReceived,
    cs_DisconnectSent
}

public enum ICMDCommandEnum {
    cmdNOP = 1,
    cmdEchoRequest = 1902465605,
    cmdEchoReply = 1819300421
}

public enum SEND_CODE {
    UNDEF_SEND,
    OK_SEND,
    NET_FAIL_SEND,
    FLOW_FAIL_SEND
}

public enum DEFAULT_AUTHFLAGS {
    AUTHFLAGS_ENABLECRYPTO = (1 << 0),
    AUTHFLAGS_ADMINACCTOVERRIDE = (1 << 1),
    AUTHFLAGS_EXTRADATA = (1 << 2),
    AUTHFLAGS_LASTDEFAULT = AUTHFLAGS_EXTRADATA
}

namespace ClientNet {
    public enum WAIT_RESULT {
        UNDEF_WAIT_RESULT,
        DONE_WAIT_RESULT,
        FAILED_WAIT_RESULT,
        ROUTED_WAIT_RESULT,
        NO_LOGON_SERVER_WAIT_RESULT,
        EXPIRED_ZONE_TICKET_RESULT
    }
}

namespace PacketInfo {
    public enum Protocol {
        fe_tcp,
        be_tcp,
        fe_udp
    }
}

public enum NetStatus {
    Net_Initializing,
    Net_LoginAuthenticating,
    Net_LoginConnecting,
    Net_LoginConnected,
    Net_LoginConnectionError,
    Net_WorldConnectionError
}

public enum NetBlobProcessedStatus {
    NETBLOB_UNDEF_STATUS,
    NETBLOB_PROCESSED_OK,
    NETBLOB_OLD_INSTANCE,
    NETBLOB_ERROR,
    NETBLOB_QUEUED
}

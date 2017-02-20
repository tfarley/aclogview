using System.Collections.Generic;

namespace aclogview
{
    class PacketRecord
    {
        public int index;
        public bool isSend;
        public uint tsSec;
        public string packetHeadersStr;
        public string packetTypeStr;
        public byte[] data;
        public int optionalHeadersLen;
        public NetPacket netPacket;
        public List<PacketOpcode> opcodes = new List<PacketOpcode>();
        public string extraInfo;
    }
}

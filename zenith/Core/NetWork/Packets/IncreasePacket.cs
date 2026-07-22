using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.NetWork.Packets
{

    [ProtoContract]
    public class IncreasePacket
    {
        [ProtoMember(1)]
        public string KeybindIncrease { get; private set; }
    }
}

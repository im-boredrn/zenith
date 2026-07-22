using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.NetWork.Packets
{

    [ProtoContract]
    public class DecreasePacket
    {

        [ProtoMember(1)]
        public string KeybindDecrease { get; private set; }
    }
}

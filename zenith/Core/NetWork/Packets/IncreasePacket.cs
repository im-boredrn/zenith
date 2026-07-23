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
        public bool ShiftHeld { get; set; }

        [ProtoMember(2)]
        public bool AltHeld { get; set; }
    }
}

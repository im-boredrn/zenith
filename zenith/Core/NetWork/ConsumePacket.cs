using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.NetWork
{
    [ProtoContract]
    public class ConsumePacket
    {
        [ProtoMember(1)]
        public string KeybindAssim { get; private set; }
    }
}

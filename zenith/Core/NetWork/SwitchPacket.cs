using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.NetWork
{
    [ProtoContract]
    public class SwitchPacket
    {
        [ProtoMember(1)]
        public string KeybindSwitch { get; private set; }
    }
}

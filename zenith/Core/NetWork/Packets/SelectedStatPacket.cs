using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.NetWork.Packets
{

    [ProtoContract]
    public class SelectedStatPacket
    {

        [ProtoMember(1)]

        public int SelectedStat { get; set; }
    }
}

using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.NetWork.Packets
{
    internal class Sounds
    {

        [ProtoContract]
        public class AdaptationGainedSoundPacket
        {
            [ProtoMember(1)]
            public long EntityID { get; set; }

            [ProtoMember(2)]
            public string SoundCode { get; set; }

            [ProtoMember(3)]
            public float PitchMin { get; set; }


            [ProtoMember(4)]
            public float PitchMax { get; set; }
        }

        [ProtoContract]
        public class AssimedSoundPacket
        {
            [ProtoMember(1)]
            public long EntityID { get; set; }


            [ProtoMember(2)]
            public string SoundCode { get; set; }

            [ProtoMember(3)]
            public float PitchMin { get; set; }


            [ProtoMember(4)]
            public float PitchMax { get; set; }
        }


        [ProtoContract]
        public class EatingSoundPacket
        {

            [ProtoMember(1)]
            public long EntityID { get; set; }


            [ProtoMember(2)]
            public string SoundCode { get; set; }

            [ProtoMember(3)]
            public float PitchMin { get; set; }


            [ProtoMember(4)]
            public float PitchMax { get; set; }
        }

        [ProtoContract]
        public class SizzleSoundPacket
        {

            [ProtoMember(1)]
            public long EntityID { get; set; }


            [ProtoMember(2)]
            public string SoundCode { get; set; }

            [ProtoMember(3)]
            public float PitchMin { get; set; }


            [ProtoMember(4)]
            public float PitchMax { get; set; }
        }


    }
}

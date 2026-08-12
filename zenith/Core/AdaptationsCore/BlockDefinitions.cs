using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.AdaptationsCore
{
    public class BlockDefinitions
    {


        public bool HasAdaptation { get; set; }
        public bool IsLocked { get; set; } = false;
        public int Threshold { get; set; } 
        public float BiomatterVal { get; set; }
        public bool IsUnknown { get; set; }


        public Type AdaptationType { get; set; }
    }
}

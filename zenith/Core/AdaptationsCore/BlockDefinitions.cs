using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.AdaptationsCore
{
    public class BlockDefinitions
    {

        public enum AdaptationValue // Make enum for category so I can .Contains(Category)
        {
            Biomatter,
            Clay,
            Fungal,
        }

        public bool HasAdaptation { get; set; }
        public bool IsLocked { get; set; } = false;
        public float Value { get; set; }
        public bool IsUnknown { get; set; }


        public Type AdaptationType { get; set; }
    }
}

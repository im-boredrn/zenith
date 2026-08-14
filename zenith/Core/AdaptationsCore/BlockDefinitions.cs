using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

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

        //public static readonly Dictionary<Block, BlockDefinitions> BlockStat = new()
        //{
        //    [] = new BlockDefinitions()
        //    {

        //    }
        //};

        public Dictionary<AdaptationValue, float> BlockGains { get; set; } = [];

        public Type AdaptationType { get; set; }
    }
}

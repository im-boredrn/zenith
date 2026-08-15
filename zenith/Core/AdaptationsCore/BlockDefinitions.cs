using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using zenith.Core.AdaptationsCore.AdaptationsFactory;

namespace zenith.Core.AdaptationsCore
{
    public class BlockDefinitions
    {

        public enum BlockCategory // Make enum for category so I can .Contains(Category)
        {
            Biomatter,
            Clay,
            Fungal,
        }

        public bool IsLocked { get; set; } = false;
        public int Threshold { get; set; }
        public int BlockLVL { get; set; } = 0;

        public static IReadOnlyDictionary<AssetLocation, BlockDefinitions> BlockLibrary => BlockDefinition;
        public static readonly Dictionary<AssetLocation, BlockDefinitions> BlockDefinition = new()
        {
            [new AssetLocation("game:clay-blue")] = new() // too rigid
            {
                AdaptationType = typeof(ClayAdaptation),
                   Threshold = 100 
                
            }
        };

       

        public Type AdaptationType { get; set; }

    }
}

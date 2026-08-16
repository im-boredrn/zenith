using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using zenith.Core.AdaptationsCore.AdaptationsFactory;

namespace zenith.Core.Definitions
{
    public class BlockDefinitions
    {

        public enum BlockCategory 
        {
            Biomatter,
            Clay,
            Fungal,
        }


        public static IReadOnlyDictionary<string, BlockDefinitions> BlockLibrary => BlockDefinition;
        public static readonly Dictionary<string, BlockDefinitions> BlockDefinition = new()
        {
            [("clay")] = new() 
            {
                AdaptationType = typeof(ClayDefinition),
                   Threshold = 100 
                
            }
        };

        public int Threshold { get; init; } 

        public Type AdaptationType { get; init; }

    }
}

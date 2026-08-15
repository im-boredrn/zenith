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
                AdaptationType = typeof(ClayAdaptation),
                   Threshold = 100 
                
            }
        };
        //TODO : Fix shared state issue between adaptationss

        public int Threshold { get; init; } 

        public Type AdaptationType { get; init; }

    }
}

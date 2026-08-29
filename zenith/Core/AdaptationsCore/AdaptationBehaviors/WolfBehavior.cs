using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using zenith.Core.Definitions;
using zenith.Core.NetWork.Packets;
using static zenith.Core.Definitions.CreatureDefinition;

namespace zenith.Core.AdaptationsCore.AdaptationBehaviors
{
    public class WolfBehavior(EntityPlayer entity) : AdaptationBehavior
    {
        //Add option to disable sounds
        public  void OnAssimilate(CreatureType creatureType) 
        {
            
            float Sat;
            Sat = CreatureDefinitions[creatureType].NutritionVal * 100f;


            entity.ReceiveSaturation(Sat, EnumFoodCategory.Protein, 10f, 2f);


            var sapi = entity.World.Api as ICoreServerAPI;
            var zenithNetwork = sapi.ModLoader.GetModSystem<ZenithCore>().ZenithNetwork;

            var packet = new Sounds.EatingSoundPacket
            {
                EntityID = entity.EntityId,
                SoundCode = "sounds/wolf/eating",
                PitchMin = 0.8f,
                PitchMax = 1f
            };

            var player = entity.Player as IServerPlayer;
            zenithNetwork.ServerChannel.SendPacket<Sounds.EatingSoundPacket>(packet, player);


        }
    }
}

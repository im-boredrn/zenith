using CompactExifLib;
using HarmonyLib;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using zenith.Config;

namespace zenith.Core
{
    internal class ZenithBehavior : EntityBehavior
    {
        private bool DebugMode = true; // For Debug Mode 
        private EntityPlayer Player => (EntityPlayer)entity; // assignment operator is saying assign the value on the left to the value on the right.

        public ZenithBehavior(Entity entity) : base(entity) // no need to pass Entityplayer entity anymore since we are attaching it to them.
        {
            if (entity.HasBehavior<ZenithBehavior>()) return;

            if (entity.World.Side == EnumAppSide.Server)
            {
                (entity.World.Api as ICoreServerAPI)?.Logger.Warning("Zenith behavior attached");
            }
        }
        public override void OnEntityReceiveDamage(DamageSource damageSource, ref float damage)
        {
            GetDamageType(damageSource); // extracts damage type

            EnumDamageType type = GetDamageType(damageSource); // catches returned type
          
            DomainEnum domain = IdentifyDomain(type);
            if (domain == DomainEnum.None && DebugMode)
            {
                Player.World.Logger.Warning("[DATA] Returning domain Not found  ");
                return;
            }
            ReduceDamage(domain, ref damage);
            ProcessDomain(domain, ref damage);

            if (DebugMode)
            {
                Player.World.Logger.Warning($"Domain :{domain}\n KineticTier: {kineticTier} \n Kinetic Counter: {kineticCounter}/{threshold} \n Damage Taken: {damage}");

            }
        }
       
        private EnumDamageType GetDamageType(DamageSource source) // Extractor
        {
            return source.Type;

        }

        private const string BluntDomain = "bluntdamage";


          public enum DomainEnum
        {
            Kinetic,
            Thermal,
            Frost,
            Toxic,
            None

        }

        public DomainEnum IdentifyDomain(EnumDamageType type) // Pure Translator
        {
            if (DebugMode)
            {
                Player.World.Logger.Warning("[FLOW] Calling IdentifyDomain");
            }

            switch (type)
            {
                case EnumDamageType.BluntAttack:
                    {
                        if (DebugMode) Player.World.Logger.Warning("[DATA] Returning Kinetic Domain ");

                        return DomainEnum.Kinetic;

                    }


                case EnumDamageType.Fire:
                    {
                        if (DebugMode)
                        {
                            Player.World.Logger.Warning("[DATA] Returning Thermal Domain ");
                        }
                        return DomainEnum.Thermal;

                    }

                default:
                    {
                        if (DebugMode)
                        {
                            Player.World.Logger.Warning("[DATA] Returning No Domain ");
                        }

                        return DomainEnum.None;

                    }
            }        
        }   
        public void ReduceDamage(DomainEnum domain, ref float damage)
        {
            if (DebugMode)
            {
                Player.World.Logger.Warning("[FLOW] Calling ReduceDamage");
            }

            switch (domain)
            {
                case DomainEnum.Kinetic:
                    {
                        if (kineticTier == 1)
                        {
                            damage *= (float)0.9;
                        }

                        if (kineticTier == 2)
                        {
                            damage *= (float)0.75;
                        }

                        if (kineticTier == 3)
                        {
                            damage *= (float)0.50;
                        }

                        break;
                    }
                case DomainEnum.Thermal:
                    {
                        if (thermalTier == 1)
                        damage *= (float)0.9;

                        if (thermalTier == 2)
                        {
                            damage *= (float)0.75;
                        }

                        if (thermalTier == 3 )
                        {
                            damage *= (float)0.50;
                        }
                        break;
                    }
            }
            if (DebugMode)
            {
                Player.World.Logger.Warning("[FLOW] Finished Calling ReduceDamage");
            }
        }

        public void ProcessDomain(DomainEnum domain, ref float damage)
        {


            if (DebugMode)
            {
                Player.World.Logger.Warning("[FLOW] Calling ProcessDomain");
            }
            switch (domain)
            {
                case DomainEnum.Kinetic:
                    {
                        kineticCounter += damage;

                        HandleTierUP(ref kineticTier, ref kineticCounter, threshold, DomainEnum.Kinetic);

                        break;
                    }


                case DomainEnum.Thermal:
                    {
                        thermalCounter += damage;

                        HandleTierUP(ref thermalTier, ref thermalCounter, threshold, DomainEnum.Thermal);

                        break;

                    }

                default:
                    {
                        return;

                    }                         
            }
            if (DebugMode)
            {
                Player.World.Logger.Warning("[FLOW] Finished Calling ProcessDomain");
            }

        }

        private void HandleTierUP(ref int tier, ref float counter, float threshold, DomainEnum domain  )
        {
            if(DebugMode)
            {
                Player.World.Logger.Warning("[FLOW] Calling Handle Tier Up");
                Player.World.Logger.Warning($"[DATA] Counter {counter}");

            }

            ICoreServerAPI sapi = entity.World.Api as ICoreServerAPI;

            if (tier < maxTier && counter >= threshold)
            {
                tier++; // modifes callers tier
                counter = 0; // modifies callers counter
                
            }

            switch (tier)
            {
                case 1:
                    {
                        sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"{domain} Raised | Tier is Now {tier}! \n " +
                            $"{domain} damage hurts you less", EnumChatType.Notification);
                        break;
                    }
                case 2:
                    {
                        sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"{domain} Raised | Tier is Now {tier}! \n " +
                          $"Your body is more resilient to {domain} damage", EnumChatType.Notification);
                        break;
                    }

                case 3:
                    {
                        sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"{domain} Raised | Tier is Now {tier}! \n " +
                       $"Your body has adapted to {domain} damage", EnumChatType.Notification);
                        break;
                    }
            }
        }

        private float threshold = 10;

        private float kineticCounter;
        private float thermalCounter;
        private float frostCounter;
        private float toxicCounter;

        private int kineticTier;
        private int thermalTier;


        private const int maxTier = 3;





        public override string PropertyName()
        {
            return "Zenith";
        }
    } 
} 

using CompactExifLib;
using HarmonyLib;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
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
        public enum DomainEnum
        {
            Kinetic,
            Thermal,
            Frost,
            Toxic,
            Hemorrhage,
            None

        }
        //#REF Dictionary Creation
        Dictionary<DomainEnum, DomainSponge> domains;

        //Summary: DomainEnum is The Key and sponge is the returned object from said key.
        //Example domains[DomainEnum.Kinetic].Counter     Counter Comes from DomainSponge object

        private bool DebugMode = false; // For Debug Mode 
        private EntityPlayer Player => (EntityPlayer)entity; // assignment operator is saying assign the value on the left to the value on the right.

        public ZenithBehavior(Entity entity) : base(entity) // no need to pass Entityplayer entity anymore since we are attaching it to them.
        {

            
            if (entity.HasBehavior<ZenithBehavior>()) return;      
          
             if (entity.World.Side == EnumAppSide.Client)
            {
                Log($"Current Side {entity.World.Side} returning");
                return;
            }
            else if (entity.World.Side == EnumAppSide.Server)
            {
                (entity.World.Api as ICoreServerAPI)?.Logger.Warning($"Zenith behavior attached to {entity.World.Side}");
            }

            foreach (var domainPair in domains)
            {
                DomainEnum domain = domainPair.Key;
                DomainSponge sponge = domainPair.Value;

                sponge.OnTierUp += (s) =>
                {
                    Log($"[EVENT] {domain} domain tier increased! New tier: {s.Tier}");

                    ICoreServerAPI sapi = entity.World.Api as ICoreServerAPI;
                    sapi.SendMessage(Player.Player,
                        GlobalConstants.AllChatGroups,
                        $"{domain} domain tier increased! New tier: {s.Tier}",
                        EnumChatType.Notification);
             
                };

            }

            domains = new Dictionary<DomainEnum, DomainSponge>();

           RegisterDomain(DomainEnum.Kinetic, new DomainSponge()); // Register domain auto assigns event listeners and creates entry

            RegisterDomain(DomainEnum.Thermal, new DomainSponge { Threshold = 8, MaxTier = 4 }); // You can change the initial properties here too
            RegisterDomain(DomainEnum.Frost, new DomainSponge()); //Initialization Essentially saying create the sponges that exist in the world 
            RegisterDomain(DomainEnum.Toxic, new DomainSponge());

            // domains[DomainEnum.Thermal] = new DomainSponge { Threshold = 8, MaxTier = 4 }; this is index assignment If This key doesnt exist it will add it if it does it will overwrite it
            // This is another way to insert an entry . Add is the second one, and it is more strict BUT it protects against duplicate keys so Its preferred

//            domains = new Dictionary<DomainEnum, DomainSponge>                This is just a shortcut for multiple .Add calls, same idea but much cleaner
//{
//    { DomainEnum.Kinetic, new DomainSponge() },
//    { DomainEnum.Thermal, new DomainSponge { Threshold = 8, MaxTier = 4 } },  
//    { DomainEnum.Frost, new DomainSponge() },
//    { DomainEnum.Toxic, new DomainSponge() }
//};
        }
        public override void OnEntityReceiveDamage(DamageSource damageSource, ref float damage)
        {
            GetDamageType(damageSource); // extracts damage type

            EnumDamageType type = GetDamageType(damageSource); // catches returned type
          
            DomainEnum domain = IdentifyDomain(type); // translates type into domain

            if (domain == DomainEnum.None )
            {
                Log("[DATA] Returning domain Not found  ");
                return;
            }
            ReduceDamage(domain, ref damage);
            ProcessDomain(domain, ref damage);

            
               Log($"Domain :{domain}\n KineticTier: {domains[domain].Tier} \n Kinetic Counter: {domains[domain].Counter}/{domains[domain].Threshold} \n Damage Taken: {damage}");
            
        }
       
        private EnumDamageType GetDamageType(DamageSource source) // #Extractor
        {
            return source.Type;
        }

        private const string BluntDomain = "bluntdamage";
    
        public DomainEnum IdentifyDomain(EnumDamageType type) // #Translator
        {
            
                Log("[FLOW] Calling IdentifyDomain");
            

            switch (type)
            {
                case EnumDamageType.BluntAttack:
                    {
                        Log("[DATA] Returning Kinetic Domain ");
                        return DomainEnum.Kinetic;
                    }
                case EnumDamageType.Gravity:
                    {
                        Log("[DATA] Returning Kinetic Domain ");
                        return DomainEnum.Kinetic;
                    }
                case EnumDamageType.Poison:
                    {
                        Log("[DATA] Returning Toxic Domain ");
                        return DomainEnum.Toxic;
                    }


                case EnumDamageType.Fire:
                    {
                        Log("[DATA] Returning Thermal Domain ");                        
                        return DomainEnum.Thermal;

                    }
                case EnumDamageType.Heat:
                    {
                        Log("[DATA] Returning Thermal Domain ");
                        return DomainEnum.Thermal;
                    }
                case EnumDamageType.Acid:
                    {
                        Log("[DATA] Returning Toxic Domain ");
                        return DomainEnum.Toxic;
                    }
                case EnumDamageType.SlashingAttack:
                    {
                        Log("[DATA] Returning Hemorrhage");
                        return DomainEnum.Hemorrhage;
                    }
                case EnumDamageType.PiercingAttack:
                    {
                        Log("[DATA] Returning Hemorrhage");
                        return DomainEnum.Hemorrhage;
                    }

                default:
                    {
                        Log("[DATA] Returning No Domain ");
                        return DomainEnum.None;
                    }
            }        
        }   
        public void ReduceDamage(DomainEnum domain, ref float damage)
        {
            
               Log("[FLOW] Calling ReduceDamage");
            
            var domainstate = domains[domain];
            domainstate.Resistance(ref damage);
            
            
               Log("[EXIT] Finished Calling ReduceDamage");
            
        }

        public void ProcessDomain(DomainEnum domain, ref float damage) //#Processor
        {


            ICoreServerAPI sapi = entity.World.Api as ICoreServerAPI; // Message

            Log("[FLOW] Calling ProcessDomain");
            

            var domainstate = domains[domain]; // auto updates dictionary, can be used to modify every Property e.g Threshold

          
            domainstate.ProcessDamage(damage); // Handles Everything inside itself

          

            Log("[EXIT] Finished Calling ProcessDomain");
            
        }

        void RegisterDomain(DomainEnum domain, DomainSponge sponge)
        {
            domains[domain] = sponge;

            sponge.OnTierUp += (s) =>
            {
                Log($"[EVENT] {domain} tier increased to {s.Tier}");

                ICoreServerAPI sapi = entity.World.Api as ICoreServerAPI;

                sapi.SendMessage(
                    Player.Player,
                    GlobalConstants.AllChatGroups,
                    $"{domain} domain tier increased! New tier: {s.Tier}",
                    EnumChatType.Notification
                );
            };
        }


        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }

        public override string PropertyName()
        {
            return "Zenith";
        }
    } 
} 
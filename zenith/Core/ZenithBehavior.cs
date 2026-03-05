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
            None

        }

        Dictionary<DomainEnum, DomainSponge> domains;

        //Summary: DomainEnum is The Key and sponge is the returned object from said key.
        //Example domains[DomainEnum.Kinetic].Counter     Counter Comes from DomainSponge object

        private bool DebugMode = true; // For Debug Mode 
        private EntityPlayer Player => (EntityPlayer)entity; // assignment operator is saying assign the value on the left to the value on the right.

        public ZenithBehavior(Entity entity) : base(entity) // no need to pass Entityplayer entity anymore since we are attaching it to them.
        {
            if (entity.HasBehavior<ZenithBehavior>()) return;

            if (entity.World.Side == EnumAppSide.Server)
            {
                (entity.World.Api as ICoreServerAPI)?.Logger.Warning("Zenith behavior attached");
            }

            domains = new Dictionary<DomainEnum, DomainSponge>();

            domains.Add(DomainEnum.Kinetic, new DomainSponge()); //
            domains.Add(DomainEnum.Thermal, new DomainSponge { Threshold = 8, MaxTier = 4 }); // You can change the initial properties here too
            domains.Add(DomainEnum.Frost, new DomainSponge()); //Initialization Essentially saying create the sponges that exist in the world 
            domains.Add(DomainEnum.Toxic, new DomainSponge()); // .Add is strict, if This already exists it will throw an exception
            domains.Add(DomainEnum.None, new DomainSponge()); //

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
                Player.World.Logger.Warning($"Domain :{domain}\n KineticTier: {domains[domain].Tier} \n Kinetic Counter: {domains[domain].Counter}/{domains[domain].Threshold} \n Damage Taken: {damage}");

            }
        }
       
        private EnumDamageType GetDamageType(DamageSource source) // Extractor
        {
            return source.Type;

        }

        private const string BluntDomain = "bluntdamage";


    
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
            var domainstate = domains[domain];
            domainstate.Resistance(ref damage);
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

            var domainstate = domains[domain]; // auto updates dictionary, can be used to modify every Property e.g Threshold

            domainstate.ProcessDamage(damage); // Handles Everything inside itself
                     
            if (DebugMode)
            {
                Player.World.Logger.Warning("[FLOW] Finished Calling ProcessDomain");
            }
        }

       

     







        public override string PropertyName()
        {
            return "Zenith";
        }
    } 
} 

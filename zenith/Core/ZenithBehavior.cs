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
using Vintagestory.GameContent;
using zenith.Config;

namespace zenith.Core
{
    internal class ZenithBehavior : EntityBehavior
    {
        public enum DomainEnum
        {
            Kinetic,
            Thermal,
            Cold,
            Toxic,
            Hemorrhage,
            None

        }
        //#REF Dictionary Creation
        Dictionary<DomainEnum, DomainSponge> domains;



        //Summary: DomainEnum is The Key and sponge is the returned object from said key.
        //Example domains[DomainEnum.Kinetic].Counter     Counter Comes from DomainSponge object

        static readonly Dictionary<EnumDamageType, DomainEnum> DamageDomainMap =
    new Dictionary<EnumDamageType, DomainEnum>()
{
    { EnumDamageType.BluntAttack, DomainEnum.Kinetic },
    { EnumDamageType.Gravity, DomainEnum.Kinetic },

    { EnumDamageType.Fire, DomainEnum.Thermal },
    { EnumDamageType.Heat, DomainEnum.Thermal },

    { EnumDamageType.Acid, DomainEnum.Toxic },
    { EnumDamageType.Poison, DomainEnum.Toxic },

    { EnumDamageType.SlashingAttack, DomainEnum.Hemorrhage },
    { EnumDamageType.PiercingAttack, DomainEnum.Hemorrhage },

    {EnumDamageType.Frost, DomainEnum.Cold }
};
        // Eventually Add Regen Domain unlock for Stage 3
        
        private bool DebugMode = false; // For Debug Mode 
        private EntityPlayer Player => (EntityPlayer)entity; // assignment operator is saying assign the value on the left to the value on the right.

      

        public ZenithBehavior(Entity entity) : base(entity) // no need to pass Entityplayer entity anymore since we are attaching it to them.
        {
            ModConfig config = new ModConfig();


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

      

            domains = new Dictionary<DomainEnum, DomainSponge>();

           RegisterDomain(DomainEnum.Kinetic, new DomainSponge (config)); // Register domain auto assigns event listeners and creates entry

            RegisterDomain(DomainEnum.Thermal, new DomainSponge(config)); // You can change the initial properties here too
            RegisterDomain(DomainEnum.Cold, new DomainSponge (config)); //Initialization Essentially saying create the sponges that exist in the world 
            RegisterDomain(DomainEnum.Toxic, new DomainSponge(config));

            // domains[DomainEnum.Thermal] = new DomainSponge { Threshold = 8, MaxTier = 4 }; this is index assignment If This key doesnt exist it will add it if it does it will overwrite it
            // This is another way to insert an entry . Add is the second one, and it is more strict BUT it protects against duplicate keys so Its preferred

            //            domains = new Dictionary<DomainEnum, DomainSponge>                This is just a shortcut for multiple .Add calls, same idea but much cleaner
            //{
            //    { DomainEnum.Kinetic, new DomainSponge() },
            //    { DomainEnum.Thermal, new DomainSponge { Threshold = 8, MaxTier = 4 } },  
            //    { DomainEnum.Frost, new DomainSponge() },
            //    { DomainEnum.Toxic, new DomainSponge() }
            //};
          
            
             

                
            
           
            foreach (var domainState in domains)
            {
                DomainEnum domain = domainState.Key; 
                DomainSponge sponge = domainState.Value;
                string keyCounter = "zenith." + domain + ".counter";
                string keyTier = "zenith." + domain + ".tier";

                sponge.Tier = entity.WatchedAttributes.GetAsInt(keyTier);
                sponge.Counter = entity.WatchedAttributes.GetAsInt(keyCounter);
            }
            EntityBehaviorHealth healthBehavior = entity.GetBehavior<EntityBehaviorHealth>();
            if (healthBehavior != null)
            {

                healthBehavior.onDamaged += (damage, source) =>
                {
                    return ReduceDamage(damage, source);
                };
            }


        }


        public override void OnEntityReceiveDamage(DamageSource damageSource, ref float damage)
        {

            EnumDamageType type = GetDamageType(damageSource); // catches returned type
          
            DomainEnum domain = IdentifyDomain(type); // translates type into domain

            if (domain == DomainEnum.None )
            {
                Log("[DATA] Returning domain Not found  ");
                return;
            }
            ProcessDomain(domain, ref damage);

            
               Log($"Domain :{domain}\n {domain} Tier: {domains[domain].Tier} \n {domain} Counter: {domains[domain].Counter}/{domains[domain].Threshold} \n Damage Taken: {damage}");
            
        }
       
        private EnumDamageType GetDamageType(DamageSource source) // #Extractor
        {
            return source.Type;
        }

        private const string BluntDomain = "bluntdamage";
    
        public DomainEnum IdentifyDomain(EnumDamageType type) // #Translator
        {
            
                Log("[FLOW] Calling IdentifyDomain");

            if(DamageDomainMap.TryGetValue(type, out DomainEnum domain))
            {
                Log($"[DATA] Returning {domain} Domain");
                return domain;
            }

            Log("[DATA] Returning No Domain");
            return DomainEnum.None;
        }

        public float ReduceDamage(float damage, DamageSource dmgSource)
        {
            
               Log("[FLOW] Calling ReduceDamage");


            EnumDamageType type = GetDamageType(dmgSource);
            DomainEnum domain = IdentifyDomain(type);

            if (domain == DomainEnum.None) return damage;

            var domainstate = domains[domain];

            damage = domainstate.Resistance(damage);
            Log("[EXIT] Finished Calling ReduceDamage");

            return damage;

            
        }

        int processCalls = 0;
        public void ProcessDomain(DomainEnum domain, ref float damage) //#Processor
        {
            processCalls++;
            Log($"[FLOW] ProcessDomain call #{processCalls}");

            Log("[FLOW] Calling ProcessDomain");
            
            var domainstate = domains[domain]; // auto updates dictionary, can be used to modify every Property e.g Threshold
          
            domainstate.ProcessDamage(damage); // Handles Everything inside itself

         
        
            Log("[EXIT] Finished Calling ProcessDomain");
        }

        void RegisterDomain(DomainEnum domain, DomainSponge sponge)
        {
            if (domains.ContainsKey(domain))
            {
                Log($"[WARN] Domain {domain} already registered");
                return;
            }
            
            domains[domain] = sponge;

            if (!sponge.TierEventRegistered)
            {
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

                    SaveDomains();
                };

                sponge.TierEventRegistered = true; // simple bool inside DomainSponge
            }
        }

       public void SaveDomains()
        {
            foreach (var domainState in domains) 
            {
                DomainEnum domain = domainState.Key; //the enum (DomainEnum.Kinetic, Thermal, etc.)
                DomainSponge sponge = domainState.Value; // the DomainSponge object

                string keyCounter = "zenith." + domain + ".counter";
                string keyTier = "zenith." + domain + ".tier";

                entity.WatchedAttributes.SetInt(keyTier, sponge.Tier);
                entity.WatchedAttributes.SetFloat(keyCounter, sponge.Counter);

                entity.WatchedAttributes.MarkPathDirty(keyTier);
                entity.WatchedAttributes.MarkPathDirty(keyCounter);

            }
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
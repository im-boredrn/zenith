using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using zenith.Config;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core
{
    public class DomainManager
    {
        public bool DebugMode => ZenithSettings.ZDebugMode;
        public bool ContributedToEvolution { get; private set; } = false;

        private EntityPlayer Player => entity as EntityPlayer;

        private readonly Entity entity;


        //#REF Dictionary Creation
        public Dictionary<DomainEnum, DomainSponge> domains { get; private set; }

        public DomainManager(Entity entity, ModConfig config)
        {
            this.entity = entity;

            domains = new Dictionary<DomainEnum, DomainSponge>();

            // Register all domains
            foreach (DomainEnum domain in Enum.GetValues(typeof(DomainEnum)))
            {
                if (domain == DomainEnum.None) continue;


                var sponge = new DomainSponge(config, entity, domain);
                RegisterDomain(domain, sponge);

                sponge.DomainMaxed += (d) => DomainMaxed?.Invoke(d);
            }


        }
        public void ProcessDomain(DomainEnum domain, ref float damage) //#Processor
        {

            Log("[FLOW] Calling ProcessDomain");

            if (!domains.TryGetValue(domain, out var domainstate)) 
                return;            

            domainstate.ProcessDamage(damage); // Handles Everything inside itself

            

            Log("[EXIT] Finished Calling ProcessDomain");
        }
        public void LoadDomains()
        {
            foreach (var kv in domains)
            {
                string keyCounter = $"zenith.{kv.Key}.counter";
                string keyTier = $"zenith.{kv.Key}.tier";
                kv.Value.Counter = entity.WatchedAttributes.GetFloat(keyCounter);
                kv.Value.Tier = entity.WatchedAttributes.GetAsInt(keyTier);
            }
        }


        public event Action<DomainSponge> DomainMaxed;
        public event Action<DomainSponge> TierUp; // DONT TOUCH

        void RegisterDomain(DomainEnum domain, DomainSponge sponge)
        {
            if (!domains.TryAdd(domain, sponge))
            {
                Log($"[WARN] Domain {domain} already registered");
            }

            domains[domain] = sponge;



            
                sponge.OnTierUp += (s) =>
                {
                    Log($"[EVENT] {domain} tier increased to {s.Tier}");
                    
                    var sapi = entity.World.Api as ICoreServerAPI;
                    if (sapi == null) return;

                    
                    var player = Player.Player;

                    sapi.SendMessage(
                        player,
                        GlobalConstants.AllChatGroups,
                        $"{domain} domain tier increased! New tier: {s.Tier}",
                        EnumChatType.Notification
                    );

                    SaveDomains();
                };

            
            
            
            



        }

     

        public void SaveDomains()
        {
            foreach (var kv in domains)
            {
                string keyCounter = $"zenith.{kv.Key}.counter";
                string keyTier = $"zenith.{kv.Key}.tier";
                entity.WatchedAttributes.SetFloat(keyCounter, kv.Value.Counter);
                entity.WatchedAttributes.SetInt(keyTier, kv.Value.Tier);
                entity.WatchedAttributes.MarkPathDirty(keyCounter);
                entity.WatchedAttributes.MarkPathDirty(keyTier);
            }
        }
        
        public void ResetDomains()
        {
            foreach (var sponge in domains.Values)
            {
                sponge.Counter = 0;
                sponge.Tier = 0;
            }
        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            entity.World.Logger.Warning(message);
        }
    }
}

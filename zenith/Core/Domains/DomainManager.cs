using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using zenith.Config;
using zenith.GUI;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core.Domains
{
    public class DomainManager
    {
        public bool DebugMode => ZenithSettings.ZDebugMode;
        public bool ContributedToEvolution { get; private set; } = false;

        private EntityPlayer Player => entity as EntityPlayer;

        private readonly Entity entity;


        //#REF Dictionary Creation
        public Dictionary<DomainEnum, DomainSponge> domains { get; private set; }

        //#REF TreeAttribute Usage
        public TreeAttribute watchedZenith;

        public DomainManager(Entity entity, ModConfig config)
        {
            this.entity = entity;

            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;

            domains = new Dictionary<DomainEnum, DomainSponge>();

            // Register all domains
            foreach (DomainEnum domain in Enum.GetValues(typeof(DomainEnum)))
            {
                if (domain == DomainEnum.None) continue;


                var sponge = new DomainSponge(config, entity, domain);
                RegisterDomain(domain, sponge);

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
                    TierUp?.Invoke(s);
                };

            sponge.DomainMaxed += (s) =>
            {
                var sapi = entity.World.Api as ICoreServerAPI;
                if (sapi == null) return;
                var player = Player.Player;


                sapi.SendMessage(player,
                    GlobalConstants.AllChatGroups,
                    $"Domain Maxed!",
                    EnumChatType.Notification
                    );

                DomainMaxed?.Invoke(s);
                SaveDomains();
            };
        }

     

        public void SaveDomains()
        {

            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;
            foreach (var kv in domains)
            {
                var domainTree = watchedZenith[kv.Key.ToString()] as TreeAttribute ?? new TreeAttribute();
                domainTree.SetInt("Tier", kv.Value.Tier);
                domainTree.SetFloat("Counter", kv.Value.Counter);
                domainTree.SetBool("Maxed", kv.Value.IsMaxed);
                watchedZenith[kv.Key.ToString()] = domainTree;


                
            }
            entity.WatchedAttributes.MarkPathDirty("zenith");
        }

        public void LoadDomains()
        {

            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;

            foreach (var kv in domains)
            {
                var domainTree = watchedZenith[kv.Key.ToString()] as TreeAttribute ?? new TreeAttribute();


                kv.Value.Counter = domainTree.GetFloat("Counter", 0);
                kv.Value.Tier = domainTree.GetInt("Tier", 0);
                kv.Value.IsMaxed = domainTree.GetBool("Maxed", false);
              
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

        public string[] GetDomainNames()
        {
            return domains.Keys.Select(d => d.ToString()).ToArray();
        }

        //domains.Keys // Get the Keys
        //.Select(d => d.ToString()) // Convert Each Enum to string
        //.ToArray(); // Convert to array




        private void Log(string message)
        {
            if (!DebugMode) return;
            entity.World.Logger.Warning(message);
        }
    }
}
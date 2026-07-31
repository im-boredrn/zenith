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
        public Dictionary<DomainEnum, IDomainInfo> Domains { get; private set; }

        //#REF TreeAttribute Usage
        public TreeAttribute watchedZenith;

        public DomainManager(Entity entity, ModConfig config)
        {
            this.entity = entity;

            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;

            Domains = new Dictionary<DomainEnum, IDomainInfo>();

            // Register all domains
            foreach (DomainEnum domain in Enum.GetValues(typeof(DomainEnum)))
            {
                if (domain == DomainEnum.None) continue;


                var sponge = new DomainSponge(config, entity, domain); 
                IDomainInfo domainInfo = sponge; // Needed for reference
                
                RegisterDomain(domain, sponge);

            }

          

        }
        public void ProcessDomain(DomainEnum domain, ref float damage) //#Processor
        {

            Log("[FLOW] Calling ProcessDomain");

            if (!Domains.TryGetValue(domain, out var domainstate)) 
                return;            

            domainstate.ProcessDamage(damage); // Handles Everything inside itself

            

            Log("[EXIT] Finished Calling ProcessDomain");
        }
      


        // public event Action<DomainSponge> DomainMaxed; // Mixed - wait could be merged - Trying to merge rn 6/5/26
       // public event Action<DomainSponge> TierUp; // - Mixed



        void RegisterDomain(DomainEnum key, IDomainInfo domainInfo)
        {
            if (!Domains.TryAdd(key, domainInfo))
            {
                Log($"[WARN] Domain {domainInfo.GetDomainName()} already registered");
            }


            
                domainInfo.OnTierUp += (s) =>
                {
                    Log($"[EVENT] {domainInfo.GetDomainName()} tier increased to {s.GetTier()}");

                     var sapi = entity.World.Api as ICoreServerAPI;
                    if (sapi == null) return;

                    
                    var player = Player.Player;

                    sapi.SendMessage(
                        player,
                        GlobalConstants.AllChatGroups,
                        $"{domainInfo.GetDomainName()} domain tier increased! New tier: {s.GetTier()}",
                        EnumChatType.Notification
                    );

                    SaveDomains();
                };

            domainInfo.DomainMaxed += () =>
            {
                var sapi = entity.World.Api as ICoreServerAPI;
                if (sapi == null) return;
                var player = Player.Player;


                sapi.SendMessage(player,
                    GlobalConstants.AllChatGroups,
                    $"Domain Maxed!",
                    EnumChatType.Notification
                    );

                SaveDomains();
            };
        }

     

        public void SaveDomains() // Not saving correctly
        {
            Log($"[FLOW] Calling SaveDomains");
            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;
            foreach (var domain in Domains.Values)
            {
                var domainTree = watchedZenith[domain.GetDomainName()] as TreeAttribute ?? new TreeAttribute();


               

                domainTree.SetInt("Tier", domain.GetTier());
                domainTree.SetFloat("Counter", domain.GetCounter());
                domainTree.SetBool("Maxed", domain.IsDMaxed());


                watchedZenith[domain.GetDomainName()] = domainTree;

              //  Log($"[DATA] Domain : {domain.GetDomainName()} | Counter : {domainTree.GetFloat("Counter")} | Tier : {domainTree.GetInt("Tier")} | Maxed? : {domainTree.GetBool("Maxed")}");


            }
            //foreach (var key in watchedZenith.Keys)
            //{
            //    var attr = watchedZenith[key];

            //    Log($"KEY={key}");

            //    if (attr == null)
            //    {
            //        Log($"NULL ATTRIBUTE: {key}");
            //    }
            //    else
            //    {
            //        Log($"TYPE={attr.GetType().Name}");
            //    }
            //}

            entity.WatchedAttributes.MarkPathDirty("zenith");
        }

        public void LoadDomains() 
        {
         
            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;

            foreach (var domain in Domains.Values)
            {
                var domainTree = watchedZenith[domain.GetDomainName()] as TreeAttribute ?? new TreeAttribute();


                domain.LoadState(
                    domainTree.GetFloat("Counter", 0),
                    domainTree.GetInt("Tier", 0),
                    domainTree.GetBool("Maxed", false));

               
               // Log($"[DATA] Domain : {domain.GetDomainName()} | Counter : {domainTree.GetFloat("Counter")} | Tier : {domainTree.GetInt("Tier")} | Maxed? : {domainTree.GetBool("Maxed")}");

            }

        }

        public void ResetDomains()
        {
            foreach (var sponge in Domains.Values)
            {
             // sponge.Counter = 0;
              //  sponge.Tier = 0;
            }
        }

        public string[] GetDomainNames() // Obselete
        {
            return Domains.Keys.Select(d => d.ToString()).ToArray();
        }



        private void Log(string message)
        {
            if (!DebugMode) return;
            entity.World.Logger.Warning(message);
        }
    }
}
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
using zenith.Core.AdaptationsCore;
using zenith.Core.AdaptationsCore.Adaptation_Definitions;
using zenith.Core.AdaptationsCore.AdaptationBehaviors;
using zenith.Core.AdaptationsCore.AdaptationData;
using zenith.Core.AdaptationsCore.AdaptationsFactory;
using zenith.Core.Helper;
using zenith.GUI;
using static zenith.Core.ZenithBehaviorServer;

namespace zenith.Core.Domains
{
    public class DomainManager
    {
        private EntityPlayer Player => entity as EntityPlayer;

        private readonly Entity entity;


        //#REF Dictionary Creation
        public Dictionary<DomainEnum, Domain> Domains { get; private set; } = [];


        private readonly ZenithData zenithData;



        public DomainManager(Entity entity, ModConfig config, ZenithData Data)
        {
            this.entity = entity;
            this.zenithData = Data;




            // Register all domains
            foreach (DomainEnum domain in Enum.GetValues<DomainEnum>())
            {
                if (domain == DomainEnum.None) continue;

                var state = new DomainState();
                var behavior = new DomainBehavior(Player, state);

                var tdomain = new Domain(state, behavior);
                RegisterDomain(domain, tdomain);





                var domains = zenithData.Tree.GetTreeAttribute("Domains");


                if (domains == null)
                {
                    domains = new TreeAttribute();
                    zenithData.Tree["Domains"] = domains;
                }

                var domainTree = domains.GetTreeAttribute(domain.ToString());

                if (domainTree == null)
                {
                    domainTree = new TreeAttribute();
                    domains[domain.ToString()] = domainTree;
                }


            }
        }

        public void ProcessDomain(DomainEnum domain, ref float damage) //#Processor
        {

            Logger.Log(Player,"[FLOW] Calling ProcessDomain");

            if (!Domains.TryGetValue(domain, out var domainBody)) 
                return;            

            domainBody.DomainBehavior.ProcessDamage(damage); // Handles Everything inside itself



            Logger.Log(Player, "[EXIT] Finished Calling ProcessDomain");
        }
      



        void RegisterDomain(DomainEnum key, Domain domain) 
        {
            if (!Domains.TryAdd(key, domain))
            {
                Logger.Log(Player, $"[WARN] Domain {key} already registered");
            }


            
                domain.DomainBehavior.OnTierUp += () =>
                {
                    Logger.Log(Player,$"[EVENT] {domain.DomainState.Tier} tier increased to {domain.DomainState.Tier}");

                     var sapi = entity.World.Api as ICoreServerAPI;
                    if (sapi == null) return;

                    
                    var player = Player.Player;

                    sapi.SendMessage(
                        player,
                        GlobalConstants.AllChatGroups,
                        $"{key} domain tier increased! New tier: {domain.DomainState.Tier}",
                        EnumChatType.Notification
                    );

                    SaveDomains();
                };

            domain.DomainBehavior.DomainMaxed += () =>
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

     

        public void SaveDomains() 
        {
            Logger.Log(Player, $"[FLOW] Calling SaveDomains");

            var domains = zenithData.Tree.GetTreeAttribute("Domains");



            foreach (DomainEnum domain in Enum.GetValues<DomainEnum>())
            {
                
                
                    var domainTree = domains.GetTreeAttribute(domain.ToString());

                    if (domainTree == null)
                    {
                        domainTree = new TreeAttribute();
                        domains[domain.ToString()] = domainTree;
                    }

                var state = Domains[domain].DomainState;

                    domainTree.SetInt("Tier", state.Tier);
                    domainTree.SetFloat("Counter", state.Counter);
                    domainTree.SetBool("Maxed", state.IsMaxed);


                    //  zenithData.Tree[domain.GetDomainName()] = domainTree;

                    //  Log($"[DATA] Domain : {domain.GetDomainName()} | Counter : {domainTree.GetFloat("Counter")} | Tier : {domainTree.GetInt("Tier")} | Maxed? : {domainTree.GetBool("Maxed")}");

                
            }
                
            entity.WatchedAttributes.MarkPathDirty("zenith");
        }

        public void LoadDomains() 
        {

            var domains = zenithData.Tree.GetTreeAttribute("Domains");


            foreach (DomainEnum domain in Enum.GetValues<DomainEnum>())
            {
                if (domain == DomainEnum.None) continue;

                var domainTree = domains.GetTreeAttribute(domain.ToString());


                var behavior = Domains[domain].DomainBehavior;

                if (domainTree == null) continue;

                behavior.LoadState(
                    domainTree.GetFloat("Counter", 0),
                    domainTree.GetInt("Tier", 0),
                    domainTree.GetBool("Maxed", false));

               
               // Log($"[DATA] Domain : {domain.GetDomainName()} | Counter : {domainTree.GetFloat("Counter")} | Tier : {domainTree.GetInt("Tier")} | Maxed? : {domainTree.GetBool("Maxed")}");

            }

        }

     

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using zenith.Config;
using zenith.Core.Abilities;
using zenith.Core.Assimilation;
using zenith.Core.Domains;
using zenith.Core.NetWork;
using zenith.Core.Progression;
using zenith.Core.Traits;
using zenith.GUI;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core
{
  
        public class ZenithSystems
        {
        public static bool DebugMode => ZenithSettings.ZDebugMode;

        public DomainManager DomainManager { get; }
            public ProgressionManager ProgressionManager { get; }
        public AbilityFactory AbilityFactory { get; }
        public ZenithGui ZenithGui { get; }
        public DomainDetailsGUI DomainDetailsGUI { get; }

        public AssimilationCore AssimilationCore { get; }
        public TraitManager TraitManager { get; }
        public StatOutput StatOutput { get; }

        public Dictionary<DomainEnum, IPassives> Passives;
        public Dictionary<DomainEnum, IAttackAbilities> Attack;
        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;

        public ZenithSystems(Entity entity, ModConfig modConfig, ICoreClientAPI capi)
            {
            this.entity = entity;

            // Core managers
            ProgressionManager = new ProgressionManager(entity);
             AbilityFactory = new AbilityFactory(ProgressionManager ,entity); 
            ProgressionManager.LoadProgression();

            AssimilationCore = new AssimilationCore(entity);
            TraitManager = new TraitManager(entity, AssimilationCore);
            StatOutput = new StatOutput(entity);
            DomainManager = new DomainManager(entity, modConfig);
            DomainManager.LoadDomains();
            RefreshStats();
           
            // GUI
            if (capi != null)
            {
                ZenithGui = new ZenithGui(capi, ProgressionManager, DomainManager, AssimilationCore ); 
            }

            Passives = Enum.GetValues<DomainEnum>()
    .Cast<DomainEnum>()
    .Where(d => d != DomainEnum.None) // skip None
    .ToDictionary(d => d, d => AbilityFactory.CreatePassives(d));

            Attack = Enum.GetValues(typeof(DomainEnum))
                .Cast<DomainEnum>()
                .Where(d => d != DomainEnum.None)
                .ToDictionary(d => d, d => AbilityFactory.CreateAttack(d));

            //   bool isClient = capi != null;


        WireEvents();
            
            }


        bool eventsWired = false;
            void WireEvents()
            {
            if (eventsWired) 
            {
                Log("[FLOW] Events Already Wired Returning...");
                return;
            }
            eventsWired = true;


            foreach ( var domain in DomainManager.Domains.Values) // per domain instance wire domain maxed event
            {
                domain.DomainMaxed += ProgressionManager.HandleDomainMaxed;
                domain.DomainMaxed += () =>
                {
                    Log($"[EVENT] DomainMaxed ZenithGui UpdateStats Called...");
                    ZenithGui?.UpdateStats(); //() INVOKES - immediately call the method once it reaches this line of code
                    if (CanUsePassive(domain))
                    {
                        AbilityFactory.ApplyPassives(domain.GetDomain());
                        Log($"[EVENT] Stats Refreshed!");
                    }
                };

                domain.OnTierUp += (d) => // Action <d> so the parameters contain the object
                {
                    ZenithGui?.UpdateStats();
                    Log($"[EVENT] {domain} tier increased to {d.GetTier()}");

                    if (CanUsePassive(d))
                    {
                        AbilityFactory.ApplyPassives(d.GetDomain());
                        Log($"[EVENT] Stats Refreshed!");
                    }
                };

            }
            // On stage up Update GUI and check if domains can get passives
            ProgressionManager.OnStageUp += () =>
            {
                Log($"[EVENT] StageUp ZenithGui UpdateStats Called...");
                ZenithGui?.UpdateStats();
                RefreshStats();
                Log($"[EVENT] Stats Refreshed!");
                foreach (var domain in DomainManager.Domains.Values)
                {
                    if (domain.GetDomain() == DomainEnum.None) continue;


                    if (CanUsePassive(domain))
                        AbilityFactory.ApplyPassives(domain.GetDomain());
                }
            };

            AssimilationCore.OnAssimChanged += () =>
            {
                TraitManager.Traits.ApplyTraits();
                // Eventually Refresh stats though may be pointless due to OnAssimChanged
            };
            
            
        }

        public void ApplyAttack(DamageSource source, EntityAgent targetEntity)
        {
            foreach (var domains in DomainManager.Domains.Values)
            {

                if (CanUseAttack(domains))
                AbilityFactory.HandleAttack(domains.GetDomain(), source, targetEntity);
            }

        }


        public void OnServerTick(float dt)
        {
            var player = entity as EntityPlayer;
            if (player == null) return;
      //      Log($"[FLOW] OnServerTick Called");

            foreach (var domain in DomainManager.Domains.Values)
            {

                if (CanUsePassive(domain))
                AbilityFactory?.TickPassives(domain.GetDomain());

            }
            //   Log($"[DATA] Current Side is {player.World.Side}");
        }

        private bool CanUsePassive( IDomainInfo domain)
        {
            var passiveReq = DomainManager.Domains[domain.GetDomain()].GetMaxTier() / 2;

            return ProgressionManager.GetStage() >= 2 && domain.GetTier() >= passiveReq;
        }

        private bool CanUseAttack(IDomainInfo domain)
        {
            var attackReq = domain.GetMaxTier() / 2;

            return ProgressionManager.GetStage() >= 2 && domain.GetTier() >= attackReq;
        }

        private void RefreshStats()
        {
            foreach (var domain in DomainManager.Domains.Values)
            {
                if (CanUsePassive(domain))
                   AbilityFactory.ApplyPassives(domain.GetDomain());
            }
        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
    }
}

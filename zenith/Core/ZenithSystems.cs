using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Abilities;
using zenith.Core.AdaptationsCore;
using zenith.Core.AdaptationsCore.AdaptationsFactory;
using zenith.Core.Assimilation;
using zenith.Core.Domains;
using zenith.Core.Helper;
using zenith.Core.Progression;
using zenith.Core.Renderers;
using zenith.Core.Traits;
using zenith.GUI;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core
{
  
        public class ZenithSystems
        {
        public static bool DebugMode => ZenithSettings.ZDebugMode;

        public ZenithData ZenithData { get; }
        public DomainManager DomainManager { get; }
            public ProgressionManager ProgressionManager { get; }
        public AbilityFactory AbilityFactory { get; }
        public ZenithGui ZenithGui { get; }
        public BearSenseRenderer BearSenseRenderer { get; }
        public DomainDetailsGUI DomainDetailsGUI { get; }

        public AssimilationCore AssimilationCore { get; }
        public CreatureAdaptations CreatureAdaptations { get; }
       // public WingedEnabler WingedEnabler;
        public Traits.Traits Traits { get; }
        public StatOutput StatOutput { get; }

        public Dictionary<DomainEnum, IPassives> Passives;
        public Dictionary<DomainEnum, IAttackAbilities> Attack;
        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;

        public ZenithSystems(Entity entity, ModConfig modConfig, ICoreClientAPI capi)
            {
            this.entity = entity;
            ZenithData = new ZenithData(entity);
            // Core managers
            ProgressionManager = new ProgressionManager(entity, ZenithData);
             AbilityFactory = new AbilityFactory(ProgressionManager ,entity); 
            ProgressionManager.LoadProgression();

            AssimilationCore = new AssimilationCore(entity, ZenithData);
            CreatureAdaptations = new CreatureAdaptations(entity, ZenithData);
                StatOutput = new StatOutput(entity, capi, ZenithData);

            Traits = new Traits.Traits(entity, AssimilationCore, StatOutput, ZenithData);

            if (entity.World.Side == EnumAppSide.Server)
            {
                Traits.ApplyTraits();
            }

            DomainManager = new DomainManager(entity, modConfig, ZenithData);
            DomainManager.LoadDomains();
            RefreshStats();
          

            // GUI
            if (capi != null && Player.Player as ICoreServerAPI == null)
            {

                var modSystem = capi.ModLoader.GetModSystem<zenithCore>();

                 
                BearSenseRenderer = new BearSenseRenderer(capi, CreatureAdaptations);


                ZenithGui = new ZenithGui(capi, ProgressionManager, DomainManager, AssimilationCore, StatOutput,modSystem.ZenithNetwork, CreatureAdaptations );
                capi.World.Player.Entity.WatchedAttributes.RegisterModifiedListener("zenith", () =>
                {
                    ZenithGui?.BonusGUI?.UpdateBonusStats();

                    CreatureAdaptations?.ReloadAdapt();


                    //if (CreatureAdaptations.ActiveAdaptations.Any(a => a is WingedAdaptation) && WingedEnabler == null)
                    //{
                    //   WingedEnabler = new WingedEnabler(capi, entity as EntityPlayer);
                    //    WingedEnabler.HasWings = true;
                    //}

                });
            }

            Passives = Enum.GetValues<DomainEnum>()
    .Cast<DomainEnum>()
    .Where(d => d != DomainEnum.None) // skip None
    .ToDictionary(d => d, d => AbilityFactory.CreatePassives(d));

            Attack = Enum.GetValues<DomainEnum>()
                .Cast<DomainEnum>()
                .Where(d => d != DomainEnum.None)
                .ToDictionary(d => d, d => AbilityFactory.CreateAttack(d));

        WireEvents();
            
            }


        bool eventsWired = false;
        void WireEvents()
        {
            if (eventsWired)
            {
                Logger.Log(Player, "[FLOW] Events Already Wired Returning...");
                return;
            }
            eventsWired = true;


            foreach (var domain in DomainManager.Domains.Values) // per domain instance wire domain maxed event
            {
                domain.DomainMaxed += ProgressionManager.HandleDomainMaxed;
                domain.DomainMaxed += () =>
                {
                    Logger.Log(Player,$"[EVENT] DomainMaxed ZenithGui UpdateStats Called...");
                    ZenithGui?.UpdateStats(); //() INVOKES - immediately call the method once it reaches this line of code
                    if (CanUsePassive(domain))
                    {
                        AbilityFactory.ApplyPassives(domain.GetDomain());
                        Logger.Log(Player, $"[EVENT] Stats Refreshed!");
                    }
                };

                domain.OnTierUp += (d) => // Action <d> so the parameters contain the object
                {
                    ZenithGui?.UpdateStats();
                    Logger.Log(Player, $"[EVENT] {domain} tier increased to {d.GetTier()}");

                    if (CanUsePassive(d))
                    {
                        AbilityFactory.ApplyPassives(d.GetDomain());
                        Logger.Log(Player, $"[EVENT] Stats Refreshed!");
                    }
                };

            }
            // On stage up Update GUI and check if domains can get passives
            ProgressionManager.OnStageUp += () =>
            {
                Logger.Log(Player, $"[EVENT] StageUp ZenithGui UpdateStats Called...");
                ZenithGui?.UpdateStats();
                RefreshStats();
                Logger.Log(Player,$"[EVENT] Stats Refreshed!");
                foreach (var domain in DomainManager.Domains.Values)
                {
                    if (domain.GetDomain() == DomainEnum.None) continue;


                    if (CanUsePassive(domain))
                        AbilityFactory.ApplyPassives(domain.GetDomain());
                }

                // ZenithGui?.BonusGUI.UpdateBonusStats();
            };

            AssimilationCore.OnAssimChanged += () =>
            {
                Traits.ApplyTraits();
            };

            AssimilationCore.AssimilationSuccess += (creatureT ) =>
            {
                CreatureAdaptations?.CheckAdaptation(creatureT);
                CreatureAdaptations?.AssimilateLink(creatureT);
            };

            StatOutput.OnOutputChange += () =>
            {
                ZenithGui?.BonusGUI?.UpdateBonusStats();
                Traits.ApplyTraits();
                Logger.Log(Player, "[EVENT]OUTPUT CHANGE EVENT FIRED");
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
             //   CreatureAdaptations?.Tick(dt);
            }

            foreach (var serverTickable in TickManager.ServerTick)
            {
                serverTickable.OnTick(player,dt);
            }
             //Log($"[DATA] Current Side is {player.World.Side}");
        }

        public void OnClientTick(float dt)
        {
            var player = entity as EntityPlayer;
            if (player == null) return;
            //      Log($"[FLOW] OnClientTick Called");
           
            foreach (var tickable in TickManager.ClientTick)
            {
                tickable.OnTick(player,dt);
            }
           
           // Log($"CLIENT COUNT: {CreatureAdaptations.ActiveAdaptations.Count}");
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


        
    }
}

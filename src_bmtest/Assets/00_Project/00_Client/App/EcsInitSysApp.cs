using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class EcsInitSysApp : IEcsInitSystem
    {

        // Injected Config
        readonly EcsCustomInject<GameCfg> _gameConfig = default;

        readonly EcsPoolInject<EcsComBusiness> _poolBusiness = default;
        readonly EcsPoolInject<EcsEventLevelUp> _poolEventOnBusinessLevelUp = default;

        //Create Ui item
        readonly EcsPoolInject<EcsEventUiBusinessViewCreate> _poolEventUiBusinessViewCreate = default;

        //Event recalculate
        readonly EcsPoolInject<EcsEventEarningNeedRecalculate> _poolEventEarningNeedRecalculate = default;
        
        //Restore saved Data
        readonly EcsPoolInject<EcsEventTryRestoreBusiness> _poolEventTryRestoreBusiness = default;

        public void Init(IEcsSystems systems)
        {
            var ecsWorld = systems.GetWorld();
            foreach (var oneBusinessCfg in _gameConfig.Value.Businesses)
            {
                if (!oneBusinessCfg.IsShowInList) continue;
                var entBusiness = ecsWorld.NewEntity();
                _poolBusiness.Value.Add(entBusiness);
                SetupBusiness(oneBusinessCfg, entBusiness);
                //Event to create UI item
                _poolEventUiBusinessViewCreate.Value.Add(entBusiness);
                //Recalculate earnings
                _poolEventEarningNeedRecalculate.Value.Add(entBusiness);
            }
        }

        private void SetupBusiness(BusinessCfg cfg, int entity)
        {
            ref var comp = ref _poolBusiness.Value.Get(entity);
            comp.ID = cfg.ID;
            comp.BusinessName = cfg.BusinessName;
            comp.EarnBaseVal = cfg.BaseEarning;
            comp.EarnDelay = cfg.EarnDelay;
            comp.CurrentTimer = cfg.EarnDelay;
            comp.BasePrice = cfg.BasePrice;
            //Upgrade 1
            comp.UpgradeFirstName = cfg.UpgradeFirstName;
            comp.UpgradeFirstPrice = cfg.UpgradeFirstPrice;
            comp.UpgradeFirstEarnМultiplier = cfg.UpgradeFirstEarnМultiplier;
            //Upgrade 2
            comp.UpgradeSecondName = cfg.UpgradeSecondName;
            comp.UpgradeSecondPrice = cfg.UpgradeSecondPrice;
            comp.UpgradeSecondEarnМultiplier = cfg.UpgradeSecondEarnМultiplier;
            if (cfg.IsOwnAtStartGame)
            {
                //Level Up to 1 for owned business at game start
                _poolEventOnBusinessLevelUp.Value.Add(entity);
            }
            TryRestoreBusiness(entity, cfg.ID);
        }

        private void TryRestoreBusiness(int entity, int IdConfig)
        {
            _poolEventTryRestoreBusiness.Value.Add(entity);
            ref var comp = ref _poolEventTryRestoreBusiness.Value.Get(entity);
            comp.EntityBusiness = entity;
            comp.IDconfig = IdConfig;
        }
    }
}
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class EcsInitSysApp : IEcsInitSystem
    {

        // ���� ����� ��������� ������ �� ������ ������������ ����, ���������� � ����� EcsSystems.Inject(xxx).
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
                //����� �� �������� UI ��������
                _poolEventUiBusinessViewCreate.Value.Add(entBusiness);
                //������� ����� ��������� ��������� � ������ ����������
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
            //��������� ������ +10% ...
            comp.UpgradeFirstEarn�ultiplier = cfg.UpgradeFirstEarn�ultiplier;
            //Upgrade 2
            comp.UpgradeSecondName = cfg.UpgradeSecondName;
            comp.UpgradeSecondPrice = cfg.UpgradeSecondPrice;
            //��������� ������ +10% ...
            comp.UpgradeSecondEarn�ultiplier = cfg.UpgradeSecondEarn�ultiplier;
            //���� ������ ������ �� �������
            if (cfg.IsOwnAtStartGame)
            {
                //� �������� �� ����� 1 ��������
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
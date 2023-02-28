using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.ComponentModel;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Client
{
    sealed class EcsRunSysBusinessLevelUp : IEcsRunSystem
    {

        readonly EcsFilterInject<Inc<EcsEventLevelUp, EcsComBusiness>> _filterEventOnBusinessLevelUp = default;

        //Wallet
        readonly EcsFilterInject<Inc<EcsComWallet>> _filterWallet = default;
        readonly EcsPoolInject<EcsComWallet> _poolWallet = default;
        readonly EcsPoolInject<EcsEventMoneySpent> _poolEventMoneySpent = default;

        //Business
        readonly EcsPoolInject<EcsComBusiness> _poolBusiness = default;
        readonly EcsPoolInject<EcsTagOwnedBusiness> _poolTagOwnedBusiness = default;
        readonly EcsPoolInject<EcsEventLevelUp> _poolEventOnBusinessLevelUp = default;


        //����� Ui ���� ������� �����
        readonly EcsFilterInject<Inc<EcsEventOnLevelUpClicked>> _filterOnLevelUpClicked = default;

        //Event recalculate
        readonly EcsPoolInject<EcsEventEarningNeedRecalculate> _poolEventEarningNeedRecalculate = default;

        public void Run(IEcsSystems systems)
        {
            //�������� ������� �������
            foreach (var entityLevelUpClicked in _filterOnLevelUpClicked.Value)
            {
                Debug.Log("EcsRunSysBusinessLevelUp : OnLevelUpClicked");
                ref var compBusiness = ref _poolBusiness.Value.Get(entityLevelUpClicked);
                int upgradePrice = compBusiness.CurrentLevelUpPrice;
                foreach (var entWallet in _filterWallet.Value)
                {
                    if (IsCanBuy(upgradePrice, entWallet))
                    {
                        SpentMoney(upgradePrice, entWallet);
                        //������� ����� ��� �������� ������
                        _poolEventOnBusinessLevelUp.Value.Add(entityLevelUpClicked);
                    }
                    else Debug.Log("EcsRunSysBusinessLevelUp : not have money!");
                }
            }

            //�������� ������
            foreach (var entBusiness in _filterEventOnBusinessLevelUp.Value)
            {
                ref var compBusiness = ref _poolBusiness.Value.Get(entBusiness);
                compBusiness.Level += 1;
                Debug.Log("EcsRunSysBusinessLevelUp : EventLevelUp LEVEL = " + compBusiness.Level.ToString());
                if (compBusiness.Level == 1)
                {
                    _poolTagOwnedBusiness.Value.Add(entBusiness);
                }
                //������� ����� ��������� ��������� � ������ ����������
                _poolEventEarningNeedRecalculate.Value.Add(entBusiness);
                //������� ����� ����� ���������
                _poolEventOnBusinessLevelUp.Value.Del(entBusiness);
            }
        }

        private bool IsCanBuy(int price, int entWallet)
        {
            ref var compWallet = ref _poolWallet.Value.Get(entWallet);
            if (compWallet.MoneyHave >= price) return true;
            return false;
        }

        private void SpentMoney(int price, int entWallet)
        {
            //����� �� �������
            _poolEventMoneySpent.Value.Add(entWallet);
            ref var compMoneySpent = ref _poolEventMoneySpent.Value.Get(entWallet);
            compMoneySpent.SpentValue = price;
        }
    }
}
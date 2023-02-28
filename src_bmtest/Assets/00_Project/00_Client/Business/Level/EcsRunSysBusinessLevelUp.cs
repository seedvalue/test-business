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


        //Ловим Ui клик поднять левел
        readonly EcsFilterInject<Inc<EcsEventOnLevelUpClicked>> _filterOnLevelUpClicked = default;

        //Event recalculate
        readonly EcsPoolInject<EcsEventEarningNeedRecalculate> _poolEventEarningNeedRecalculate = default;

        public void Run(IEcsSystems systems)
        {
            //Кликнули поднять уровень
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
                        //Бросаем ивент для поднятия уровня
                        _poolEventOnBusinessLevelUp.Value.Add(entityLevelUpClicked);
                    }
                    else Debug.Log("EcsRunSysBusinessLevelUp : not have money!");
                }
            }

            //Поднятие уровня
            foreach (var entBusiness in _filterEventOnBusinessLevelUp.Value)
            {
                ref var compBusiness = ref _poolBusiness.Value.Get(entBusiness);
                compBusiness.Level += 1;
                Debug.Log("EcsRunSysBusinessLevelUp : EventLevelUp LEVEL = " + compBusiness.Level.ToString());
                if (compBusiness.Level == 1)
                {
                    _poolTagOwnedBusiness.Value.Add(entBusiness);
                }
                //Бросаем ивент пересчета заработка и других переменных
                _poolEventEarningNeedRecalculate.Value.Add(entBusiness);
                //Удаляем ивент после обработки
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
            //Ивент на кошелек
            _poolEventMoneySpent.Value.Add(entWallet);
            ref var compMoneySpent = ref _poolEventMoneySpent.Value.Get(entWallet);
            compMoneySpent.SpentValue = price;
        }
    }
}
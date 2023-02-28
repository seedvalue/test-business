using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class EcsRunSysBusinessUpgrade : IEcsRunSystem
    {

        readonly EcsFilterInject<Inc<EcsEventOnBusinessUpgrade1, EcsComBusiness>> _filterEventOnBusinessUpgrade1 = default;
        readonly EcsFilterInject<Inc<EcsEventOnBusinessUpgrade2, EcsComBusiness>> _filterEventOnBusinessUpgrade2 = default;

        //Wallet
        readonly EcsFilterInject<Inc<EcsComWallet>> _filterWallet = default;
        readonly EcsPoolInject<EcsComWallet> _poolWallet = default;
        readonly EcsPoolInject<EcsEventMoneySpent> _poolEventMoneySpent = default;

        //Business
        readonly EcsPoolInject<EcsComBusiness> _poolBusiness = default;
        readonly EcsPoolInject<EcsTagOwnedBusiness> _poolTagOwnedBusiness = default;
        readonly EcsPoolInject<EcsEventOnBusinessUpgrade1> _poolEventEventOnBusinessUpgrade1 = default;
        readonly EcsPoolInject<EcsEventOnBusinessUpgrade2> _poolEventEventOnBusinessUpgrade2 = default;

        //Ловим Ui клик применить Upgrade
        readonly EcsFilterInject<Inc<EcsEventOnUpgrade1Clicked>> _filterEventOnUpgrade1Clicked = default;
        readonly EcsFilterInject<Inc<EcsEventOnUpgrade2Clicked>> _filterEventOnUpgrade2Clicked = default;

        //Event recalculate
        readonly EcsPoolInject<EcsEventEarningNeedRecalculate> _poolEventEarningNeedRecalculate = default;


        public void Run(IEcsSystems systems)
        {
            //Кликнули применить Upgrade 1
            foreach (var entityUpgrade1 in _filterEventOnUpgrade1Clicked.Value)
            {
                Debug.Log("EcsRunSysBusinessUpgrade : EventOnUpgrade1Clicked");
                ref var compBusiness = ref _poolBusiness.Value.Get(entityUpgrade1);
                int upgradePrice = compBusiness.UpgradeFirstPrice;
                foreach (var entWallet in _filterWallet.Value)
                {
                    if(IsCanBuy(upgradePrice, entWallet))
                    {
                        SpentMoney(upgradePrice, entWallet);
                        //Деньги уплочены Апгрейдим 1
                        _poolEventEventOnBusinessUpgrade1.Value.Add(entityUpgrade1);
                    }
                    else Debug.Log("EcsRunSysBusinessUpgrade 1 : not have money!");

                }
            }

            //Кликнули применить Upgrade 2
            foreach (var entityUpgrade2 in _filterEventOnUpgrade2Clicked.Value)
            {
                Debug.Log("EcsRunSysBusinessUpgrade : EventOnUpgrade2Clicked");
                ref var compBusiness = ref _poolBusiness.Value.Get(entityUpgrade2);
                int upgradePrice = compBusiness.UpgradeSecondPrice;
                foreach (var entWallet in _filterWallet.Value)
                {
                    if (IsCanBuy(upgradePrice, entWallet))
                    {
                        SpentMoney(upgradePrice, entWallet);
                        //Деньги уплочены Апгрейдим 2
                        _poolEventEventOnBusinessUpgrade2.Value.Add(entityUpgrade2);
                    }
                    else Debug.Log("EcsRunSysBusinessUpgrade 2 : not have money!");
                }
            }

            //Upgrade 1
            foreach (var entBusiness in _filterEventOnBusinessUpgrade1.Value)
            {
                ref var compBusiness = ref _poolBusiness.Value.Get(entBusiness);
                compBusiness.IsUpgrade1Applyed = true;
                _poolEventEarningNeedRecalculate.Value.Add(entBusiness);
                //Удаляем ивент после обработки
                _poolEventEventOnBusinessUpgrade1.Value.Del(entBusiness);
            }

            //Upgrade 2
            foreach (var entBusiness in _filterEventOnBusinessUpgrade2.Value)
            {
                ref var compBusiness = ref _poolBusiness.Value.Get(entBusiness);
                compBusiness.IsUpgrade2Applyed = true;
                _poolEventEarningNeedRecalculate.Value.Add(entBusiness);
                //Удаляем ивент после обработки
                _poolEventEventOnBusinessUpgrade2.Value.Del(entBusiness);
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
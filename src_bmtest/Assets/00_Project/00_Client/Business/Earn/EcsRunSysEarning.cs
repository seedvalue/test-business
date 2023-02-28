using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.ComponentModel;
using UnityEngine;

namespace Client
{

    /*
        Прогресс дохода
        Бар заполняется от 0% до 100% за время “Задержка дохода” из конфига ниже.
        Как только доходит до 100%, значение текущего дохода зачисляется в Баланс,
        а прогресс начинает копиться заново с 0%.
        Значение бара должно обновляться каждый кадр (для плавности).

    Доход = lvl * базовый_доход * (1 + множитель_от_улучшения_1 + множитель_от_улучшения_2)

     */


    //Система дохода
    sealed class EcsRunSysEarning : IEcsRunSystem
    {

        readonly EcsFilterInject<Inc<EcsTagOwnedBusiness>> _filterTagOwnedBusiness = default;
        readonly EcsFilterInject<Inc<EcsEventEarningNeedRecalculate>> _filterEventEarningNeedRecalculate = default;

        readonly EcsPoolInject<EcsComBusiness> _poolBusiness = default;
        readonly EcsPoolInject<EcsTagOwnedBusiness> _pooTagOwnedBusiness = default;

        readonly EcsPoolInject<EcsEventEarned> _poolEventEarned = default;
        readonly EcsPoolInject<EcsEventEarningNeedRecalculate> _poolEcsEventEarningNeedRecalculate = default;
        readonly EcsPoolInject<EcsEventUiBusinessViewUpdate> _poolventUiBusinessViewUpdate = default;

        public void Run(IEcsSystems systems)
        {
            //Ловим событие Need recalculate
            //Прилетает при поднятии уровня, применении апгрейдов
            foreach (var entity in _filterEventEarningNeedRecalculate.Value)
            {
                Debug.Log("EcsRunSysEarning : EventEarningNeedRecalculate");
                ref var compBusiness = ref _poolBusiness.Value.Get(entity);

                int level = compBusiness.Level;
                int baseEarn = compBusiness.EarnBaseVal;
                float upgradeMultFirst = 0;
                float upgradeMultSecond = 0;
                if (compBusiness.IsUpgrade1Applyed) 
                    upgradeMultFirst = compBusiness.UpgradeFirstEarnМultiplier / 100F;   
                if (compBusiness.IsUpgrade2Applyed) 
                    upgradeMultSecond = compBusiness.UpgradeSecondEarnМultiplier / 100F;
               
                //recalculate
                int levelUpPrice = (compBusiness.Level + 1) * compBusiness.BasePrice;
                float newEarnVal = level * baseEarn * (1F + upgradeMultFirst + upgradeMultSecond);
                compBusiness.CurrentLevelUpPrice = levelUpPrice;
                compBusiness.EarnVal = Mathf.RoundToInt(newEarnVal);
                //UI View
                _poolventUiBusinessViewUpdate.Value.Add(entity);
                //Чистка события после обработки
                _poolEcsEventEarningNeedRecalculate.Value.Del(entity);
            }

            //Real time прогресс подсчет, таймер
            //Ищем бизнесы с тегом OwnedBusiness
            foreach (var entity in _filterTagOwnedBusiness.Value)
            {
                ref var compBusiness = ref _poolBusiness.Value.Get(entity);
                compBusiness.CurrentTimer -= Time.deltaTime;
                compBusiness.BusinessProgressEarning = compBusiness.CurrentTimer / compBusiness.EarnDelay;
                if (compBusiness.CurrentTimer <= 0)
                {
                    //Reset timer
                    compBusiness.CurrentTimer = compBusiness.EarnDelay;
                    //Бросаем ивент что заработана сумма, кошелек обработает
                    _poolEventEarned.Value.Add(entity);
                    ref var compEvent = ref _poolEventEarned.Value.Get(entity);
                    compEvent.EarnedValue = compBusiness.EarnVal;
                }  
            }
        }
    }
}
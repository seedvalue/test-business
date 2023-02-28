using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.ComponentModel;
using UnityEngine;

namespace Client
{

    /*
        �������� ������
        ��� ����������� �� 0% �� 100% �� ����� ��������� ������� �� ������� ����.
        ��� ������ ������� �� 100%, �������� �������� ������ ����������� � ������,
        � �������� �������� �������� ������ � 0%.
        �������� ���� ������ ����������� ������ ���� (��� ���������).

    ����� = lvl * �������_����� * (1 + ���������_��_���������_1 + ���������_��_���������_2)

     */


    //������� ������
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
            //����� ������� Need recalculate
            //��������� ��� �������� ������, ���������� ���������
            foreach (var entity in _filterEventEarningNeedRecalculate.Value)
            {
                Debug.Log("EcsRunSysEarning : EventEarningNeedRecalculate");
                ref var compBusiness = ref _poolBusiness.Value.Get(entity);

                int level = compBusiness.Level;
                int baseEarn = compBusiness.EarnBaseVal;
                float upgradeMultFirst = 0;
                float upgradeMultSecond = 0;
                if (compBusiness.IsUpgrade1Applyed) 
                    upgradeMultFirst = compBusiness.UpgradeFirstEarn�ultiplier / 100F;   
                if (compBusiness.IsUpgrade2Applyed) 
                    upgradeMultSecond = compBusiness.UpgradeSecondEarn�ultiplier / 100F;
               
                //recalculate
                int levelUpPrice = (compBusiness.Level + 1) * compBusiness.BasePrice;
                float newEarnVal = level * baseEarn * (1F + upgradeMultFirst + upgradeMultSecond);
                compBusiness.CurrentLevelUpPrice = levelUpPrice;
                compBusiness.EarnVal = Mathf.RoundToInt(newEarnVal);
                //UI View
                _poolventUiBusinessViewUpdate.Value.Add(entity);
                //������ ������� ����� ���������
                _poolEcsEventEarningNeedRecalculate.Value.Del(entity);
            }

            //Real time �������� �������, ������
            //���� ������� � ����� OwnedBusiness
            foreach (var entity in _filterTagOwnedBusiness.Value)
            {
                ref var compBusiness = ref _poolBusiness.Value.Get(entity);
                compBusiness.CurrentTimer -= Time.deltaTime;
                compBusiness.BusinessProgressEarning = compBusiness.CurrentTimer / compBusiness.EarnDelay;
                if (compBusiness.CurrentTimer <= 0)
                {
                    //Reset timer
                    compBusiness.CurrentTimer = compBusiness.EarnDelay;
                    //������� ����� ��� ���������� �����, ������� ����������
                    _poolEventEarned.Value.Add(entity);
                    ref var compEvent = ref _poolEventEarned.Value.Get(entity);
                    compEvent.EarnedValue = compBusiness.EarnVal;
                }  
            }
        }
    }
}
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    /// <summary>
    /// ������� ��������� ��������� ������. 
    /// ��������� �� ��� ������������,
    /// ���� ���������� ������� �� �������
    /// ������ ���������� ������ ����� ��������� �������� (�� ������ ����)
    /// </summary>
    sealed class EcsRunSysUiItemBusinessButtonsLock : IEcsRunSystem
    {

        readonly EcsFilterInject<Inc<EcsComWallet>> _filterWallet = default;

        readonly EcsFilterInject<Inc<EcsComBusiness>, Exc<EcsTagOwnedBusiness>> _filterNotOwnedBusinesses = default;
        
        readonly EcsFilterInject<Inc<EcsComBusiness>> _filterBusiness = default;
        readonly EcsFilterInject<Inc<EcsEventWalletUpdated>> _filterEventWalletUpdated = default;

        readonly EcsPoolInject<EcsComBusiness> _poolBusiness = default;
        readonly EcsPoolInject<EcsComWallet> _poolWallet = default;

        readonly EcsPoolInject<EcsEventWalletUpdated> _poolEventWalletUpdated = default;


      

        public void Run(IEcsSystems systems)
        {

            //������� �������
            foreach (var entWallet in _filterEventWalletUpdated.Value)
            {
                Debug.Log("EcsRunSysButtonsLock : WalletUpdated, update buttons lock");
                ref var compWallet = ref _poolWallet.Value.Get(entWallet);
                int moneyHave = compWallet.MoneyHave;

                //������ ���� ��������
                foreach (var entBusiness in _filterBusiness.Value)
                {
                    ref var compBusiness = ref _poolBusiness.Value.Get(entBusiness);
                    Button btnLevelUp = compBusiness.UiView.ButtonLevelUp;
                    Button btnUpgrade1 = compBusiness.UiView.ButtonUpgradeFirst;
                    Button btnUpgrade2 = compBusiness.UiView.ButtonUpgradeSecond;
                    btnLevelUp.interactable = IsMoneyHave(compBusiness.CurrentLevelUpPrice, moneyHave);                    
                    if(compBusiness.IsUpgrade1Applyed == false)
                    {
                        //�� ��������� ������� 1 ��������� ������� �� �����
                        btnUpgrade1.interactable = IsMoneyHave(compBusiness.UpgradeFirstPrice, moneyHave);
                    } else btnUpgrade1.interactable = false; //�������, �����

                    if (compBusiness.IsUpgrade2Applyed == false)
                    {
                        //�� ��������� ������� 1 ��������� ������� �� �����
                        btnUpgrade2.interactable = IsMoneyHave(compBusiness.UpgradeSecondPrice, moneyHave);
                    }
                    else btnUpgrade2.interactable = false; //�������, �����
                }
            }
            //������ ������ ��������� ���� ������ �� ������
            //������ �� ��������� ��������
            foreach (var entNotOwned in _filterNotOwnedBusinesses.Value)
            {
                ref var compBusiness = ref _poolBusiness.Value.Get(entNotOwned);
                compBusiness.UiView.ButtonUpgradeFirst.interactable = false;
                compBusiness.UiView.ButtonUpgradeSecond.interactable = false;
            }
        }

        private bool IsMoneyHave(int need, int have)
        {
            if (have >= need) return true;
            return false;
        }
    }
}
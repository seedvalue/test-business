using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Client
{
    sealed class EcsRunSysWallet : IEcsInitSystem, IEcsRunSystem
    {

        readonly EcsFilterInject<Inc<EcsEventEarned>> _filterEventEarned = default;
        readonly EcsFilterInject<Inc<EcsEventMoneySpent>> _filterEventMoneySpent = default;
        readonly EcsFilterInject<Inc<EcsEventWalletRestored>> _filterEventWalletRestored = default;
        // readonly EcsFilterInject<Inc<EcsComWallet>> _filterWallet = default;


        readonly EcsPoolInject<EcsComWallet> _poolWallet = default;
        readonly EcsPoolInject<EcsEventEarned> _poolEventEarned = default;
        readonly EcsPoolInject<EcsEventMoneySpent> _poolEventMoneySpent = default;
        readonly EcsPoolInject<EcsEventWalletUpdated> _poolEventWalletUpdated = default;
        readonly EcsPoolInject<EcsEventWalletRestored> _poolEventWalletRestored = default;


        int _entWallet;
        
        public void Init(IEcsSystems systems)
        {
           
            var ecsWorld = systems.GetWorld();
            //������� �������� ��������
            //������ 1, ������� �������� ��� 
            _entWallet = ecsWorld.NewEntity();
            _poolWallet.Value.Add(_entWallet);
            //������� ������� ����, ��� ������� ��������
            _poolEventWalletUpdated.Value.Add(_entWallet);
        }

        public void Run(IEcsSystems systems)
        {
            //����� ������� �������������� ��������
            foreach (var entWallet in _filterEventWalletRestored.Value)
            {
                Debug.Log("EcsRunSysWallet : Wallet restored");
                ref var compRestored = ref _poolEventWalletRestored.Value.Get(entWallet);
                int restoredMoney = compRestored.RestoredMoney;

                ref var compWallet = ref _poolWallet.Value.Get(_entWallet);
                compWallet.MoneyHave = restoredMoney;
                //������� ������� ����, ��� ������� ��������
                if (_poolEventWalletUpdated.Value.Has(_entWallet) == false)
                    _poolEventWalletUpdated.Value.Add(_entWallet);
                //������� ������������ ������� ��������������
                _poolEventWalletRestored.Value.Del(entWallet);
            }


            //����� ������� ���������
            foreach (var entEarnedEvent in _filterEventEarned.Value)
            {
                Debug.Log("EcsRunSysWallet : EarnedEvent");
                ref var compEarnedEvent = ref _poolEventEarned.Value.Get(entEarnedEvent);
                int earnedCount = compEarnedEvent.EarnedValue;

                ref var compWallet = ref _poolWallet.Value.Get(_entWallet);
                compWallet.MoneyHave += earnedCount;
                //������� ������� ����, ��� ������� ��������
                if (_poolEventWalletUpdated.Value.Has(_entWallet) == false)
                    _poolEventWalletUpdated.Value.Add(_entWallet);
            }

            //����� ������� ������ �����
            foreach (var entEventMoneySpent in _filterEventMoneySpent.Value)
            {
                Debug.Log("EcsRunSysWallet : EventMoneySpent");
                ref var compSpent = ref _poolEventMoneySpent.Value.Get(entEventMoneySpent);
                ref var compWallet = ref _poolWallet.Value.Get(_entWallet);
                if (compWallet.MoneyHave >= compSpent.SpentValue)
                {
                    //������ ����, ������
                    compWallet.MoneyHave -= compSpent.SpentValue;
                    //������� ������� ����, ��� ������� ��������
                    if (_poolEventWalletUpdated.Value.Has(_entWallet) == false)
                        _poolEventWalletUpdated.Value.Add(_entWallet);
                }
                else Debug.LogError("EcsRunSysWallet : MoneySpent : NOT HAVE MONEY");
                _poolEventMoneySpent.Value.Del(_entWallet);
            }
        }
    }
}
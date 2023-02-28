using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Client
{
    sealed class EcsRunSysSaveRestore : IEcsInitSystem, IEcsRunSystem
    {

        //Quit app avter save state
        readonly EcsPoolInject<EcsEventQuitApp> _poolEventQuitApp = default;

        //Save or Clean
        readonly EcsFilterInject<Inc<EcsEventSaveData>> _filterSaveData = default;
        readonly EcsFilterInject<Inc<EcsEventCleanSavedData>> _filterCleanData = default;
        //Pool save or clean
        readonly EcsPoolInject<EcsEventSaveData> _poolEventSaveData = default;
        readonly EcsPoolInject<EcsEventCleanSavedData> _poolEventCleanSavedData = default;

        //Owned business
        readonly EcsFilterInject<Inc<EcsTagOwnedBusiness>> _filterTagOwnedBusiness = default;
        readonly EcsPoolInject<EcsComBusiness> _poolBusiness = default;
        readonly EcsPoolInject<EcsTagOwnedBusiness> _pooTagOwnedBusiness = default;

        //Wallet
        readonly EcsFilterInject<Inc<EcsComWallet>> _filterWallet = default;
        readonly EcsPoolInject<EcsComWallet> _poolWallet = default;
        readonly EcsPoolInject<EcsEventWalletRestored> _poolEventWalletRestored = default;

        //Business restore
        readonly EcsFilterInject<Inc<EcsEventTryRestoreBusiness>> _filterEventTryRestoreBusiness = default;
        readonly EcsPoolInject<EcsEventTryRestoreBusiness> _poolEventTryRestoreBusiness = default;

        //Event recalculate
        readonly EcsPoolInject<EcsEventEarningNeedRecalculate> _poolEventEarningNeedRecalculate = default;

        const string PREF_KEY_WALLET = "Wallet";
        const string PREF_KEY_BUSINESS = "Bussines_";
        public void Init(IEcsSystems systems)
        {
            if (PlayerPrefs.HasKey(PREF_KEY_WALLET))
            {
                int savedMoney = PlayerPrefs.GetInt(PREF_KEY_WALLET);
                //Ищем кошелек
                foreach (var entWallet in _filterWallet.Value)
                {
                    _poolEventWalletRestored.Value.Add(entWallet);
                    ref var compWalletRestored = ref _poolEventWalletRestored.Value.Get(entWallet);
                    //Отправить событие на кошелек
                    compWalletRestored.RestoredMoney = savedMoney;
                }
            }
        }

        public void Run(IEcsSystems systems)
        {
            //Event Save data
            foreach (var entEventSave in _filterSaveData.Value)
            {
                //Найти активные бизнесы
                foreach (var entActiveBusinesses in _filterTagOwnedBusiness.Value)
                {
                    ref var compBusiness = ref _poolBusiness.Value.Get(entActiveBusinesses);
                    string json = JsonUtility.ToJson(compBusiness);
                    SaveBusiness(compBusiness.ID, json);
                }
                //Найти кошелек
                foreach (var entWallet in _filterWallet.Value)
                {
                    ref var compWallet = ref _poolWallet.Value.Get(entWallet);
                    SaveWallet(compWallet.MoneyHave);
                }
                _poolEventQuitApp.Value.Add(entEventSave);
                //Удалить ивент после обработки
                _poolEventSaveData.Value.Del(entEventSave);
            }

            //Event Clean data
            foreach (var entEventClean in _filterCleanData.Value)
            {
                PlayerPrefs.DeleteAll();
                _poolEventQuitApp.Value.Add(entEventClean);
                //Удалить ивент после обработки
                _poolEventCleanSavedData.Value.Del(entEventClean);
            }

            //Event try restore Business
            foreach (var entBusiness in _filterEventTryRestoreBusiness.Value)
            {
                ref var compRestoreEvent = ref _poolEventTryRestoreBusiness.Value.Get(entBusiness);
                int business = compRestoreEvent.EntityBusiness;
                int ID = compRestoreEvent.IDconfig;
                if(IsSavedBusinessHave(ID))
                {
                    //Бизнес с данным ID есть сохраненный
                    ref var compCurrentBusiness = ref _poolBusiness.Value.Get(entBusiness);
                    EcsComBusiness restored = GetSavedBusiness(ID);
                    //Восстанавливаем только прогресс, левел, применены ли апдейты
                    //значение таймера. Остальное засетаплено изначально с конфигов
                    compCurrentBusiness.BusinessProgressEarning = restored.BusinessProgressEarning;
                    compCurrentBusiness.CurrentTimer = restored.CurrentTimer;
                    compCurrentBusiness.Level = restored.Level;
                    compCurrentBusiness.IsUpgrade1Applyed = restored.IsUpgrade1Applyed;
                    compCurrentBusiness.IsUpgrade2Applyed = restored.IsUpgrade2Applyed;
                    //Ставим тег, что бизнес куплен
                    if (compCurrentBusiness.Level>0)
                    {
                        if (_pooTagOwnedBusiness.Value.Has(entBusiness) == false)
                            _pooTagOwnedBusiness.Value.Add(entBusiness);
                    }
                   
                    //И бросить ивент для пересчета формул
                    if (_poolEventEarningNeedRecalculate.Value.Has(entBusiness) == false)
                    _poolEventEarningNeedRecalculate.Value.Add(entBusiness);

                    Debug.Log("EcsRunSysSaveRestore : restored business id=" + ID);
                }
                _poolEventTryRestoreBusiness.Value.Del(entBusiness);
            }
        }

        private void SaveWallet(int moneyHave)
        {
            Debug.Log("SaveWallet : moneyHave = " + moneyHave);
            PlayerPrefs.SetInt(PREF_KEY_WALLET, moneyHave);
        }

        private void SaveBusiness(int id, string json)
        {
            Debug.Log("SaveBusiness : id  = " + id + " json: \n" + json);
            string key = PREF_KEY_BUSINESS + id.ToString();
            PlayerPrefs.SetString(key, json);
        }

        private bool IsSavedBusinessHave(int IdConfig)
        {
            if (PlayerPrefs.HasKey(PREF_KEY_BUSINESS + IdConfig.ToString())) return true;
            return false;
        }

        private EcsComBusiness GetSavedBusiness(int IdConfig)
        {
            string key = PREF_KEY_BUSINESS + IdConfig.ToString();
            EcsComBusiness restored = new EcsComBusiness();
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                restored = JsonUtility.FromJson<EcsComBusiness>(json);
            }
            return restored;
        }
    }
}
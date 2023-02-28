using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Client
{
    sealed class EcsRunSysWndGame : IEcsInitSystem, IEcsRunSystem
    {
        readonly EcsPoolInject<EcsComWndGame> _poolWndGame = default;
        
        //Save or Clean
        readonly EcsPoolInject<EcsEventSaveData> _poolEventSaveData = default;
        readonly EcsPoolInject<EcsEventCleanSavedData> _poolEventCleanSavedData = default;


        //Update Ui wallet
        readonly EcsFilterInject<Inc<EcsEventWalletUpdated, EcsComWallet>> _filterEventUpdateWallet = default;
        readonly EcsPoolInject<EcsComWallet> _pullWallet = default;

        //EXIT Wnd clicks
        readonly EcsFilterInject<Inc<EcsEventOnClickedExit>> _filterEventOnClickedExit = default;
        readonly EcsFilterInject<Inc<EcsEventOnClickedSaveAndExit>> _filterEventOnClickedSaveAndExit = default;
        readonly EcsFilterInject<Inc<EcsEventOnClickedCleanAndExit>> _filterEventOnClickedCleanAndExit = default;
        readonly EcsFilterInject<Inc<EcsEventOnClickedExitBack>> _filterEventOnClickedExitBack = default;

        readonly EcsPoolInject<EcsEventOnClickedExit> _poolEventOnClickedExit = default;
        readonly EcsPoolInject<EcsEventOnClickedSaveAndExit> _poolEventOnClickedExitSave = default;
        readonly EcsPoolInject<EcsEventOnClickedCleanAndExit> _poolEventOnClickedExitClean = default;
        readonly EcsPoolInject<EcsEventOnClickedExitBack> _poolEventOnClickedExitBack = default;

        //Events ItemUi
        readonly EcsFilterInject<Inc<EcsEventUiBusinessViewCreate, EcsComBusiness>> _filterEventCreateBusinessView = default;
        readonly EcsFilterInject<Inc<EcsEventUiBusinessViewUpdate, EcsComBusiness>> _filterEventUiUpdateBusinessView = default;

        //Pull 
        readonly EcsPoolInject<EcsComBusiness> _poolBusiness = default;
        readonly EcsPoolInject<EcsEventUiBusinessViewCreate> _poolEventUiBusinessViewCreate = default;
        readonly EcsPoolInject<EcsEventUiBusinessViewUpdate> _poolventUiBusinessViewUpdate = default;

        //UI Item business Button click
        readonly EcsPoolInject<EcsEventOnLevelUpClicked> _poolEventOnLevelUpClicked = default;
        readonly EcsPoolInject<EcsEventOnUpgrade1Clicked> _poolEventOnUpgrade1Clicked = default;
        readonly EcsPoolInject<EcsEventOnUpgrade2Clicked> _poolEventOnUpgrade2Clicked = default;

        //Owned business
        readonly EcsFilterInject<Inc<EcsTagOwnedBusiness>> _filterTagOwnedBusiness = default;

        private ViewWndGame _viewWndGame;

        int _entWndGame;

        public void Init(IEcsSystems systems)
        {
            var ecsWorld = systems.GetWorld();
            _entWndGame = ecsWorld.NewEntity();
            _poolWndGame.Value.Add(_entWndGame);
            ref var compWndGame = ref _poolWndGame.Value.Get(_entWndGame);
            _viewWndGame = GameObject.FindFirstObjectByType<ViewWndGame>();
            CleanupOldItemsList(_viewWndGame);
            _viewWndGame.PopupExit.SetActive(false);
            compWndGame.View = _viewWndGame;
            SetupExitButtons(_entWndGame);
        }

        public void Run(IEcsSystems systems)
        {
            RunExitButtons(systems);

            //Event update wallet
            foreach (var entWallet in _filterEventUpdateWallet.Value)
            {
                ref var comp = ref _pullWallet.Value.Get(entWallet);
                UpdateWalletView(comp.MoneyHave);
            }

            //Event create business view
            foreach (var entBusiness in _filterEventCreateBusinessView.Value)
            {
                ref var compBusiness = ref _poolBusiness.Value.Get(entBusiness);

                var parent = _viewWndGame.UiContentBusinessList;
                var prefabItem = _viewWndGame.UiPrefabViewBusiness;
                var instanced = GameObject.Instantiate(prefabItem, parent);
                var itemView = instanced.GetComponent<ViewUiOneBusiness>();
                compBusiness.UiView = itemView;

                itemView.TextBusinessName.text = compBusiness.BusinessName;
                itemView.SliderProgressEarning.value = 0F;
                itemView.ButtonLevelUp.onClick.AddListener(delegate { OnClickLevelUp(entBusiness); });

                //Upgrade 1
                itemView.TextUpgradeFirstName.text = compBusiness.UpgradeFirstName;
                itemView.TextUpgradeFirstPriceVal.text = "÷ÂÌ‡: " + compBusiness.UpgradeFirstPrice.ToString() + "$";
                itemView.TextUpgradeFirstEarnÃultiplie.text = "ƒÓıÓ‰: +" + compBusiness.UpgradeFirstEarnÃultiplier.ToString() + "%";
                itemView.ButtonUpgradeFirst.onClick.AddListener(delegate { OnClickUpgradeFirst(entBusiness); });

                //Upgrade 2
                itemView.TextUpgradeSecondName.text = compBusiness.UpgradeSecondName;
                itemView.TextUpgradeSecondPriceVal.text = "÷ÂÌ‡: " + compBusiness.UpgradeSecondPrice.ToString() + "$";
                itemView.TextUpgradeSecondEarnÃultiplie.text = "ƒÓıÓ‰: +" + compBusiness.UpgradeSecondEarnÃultiplier.ToString() + "%";
                itemView.ButtonUpgradeSecond.onClick.AddListener(delegate { OnClickUpgradeSecond(entBusiness); });

                //”‰‡ÎËÚ¸ Ó·‡·ÓÚ‡ÌÌ˚È Event
                _poolEventUiBusinessViewCreate.Value.Del(entBusiness);
            }

            //Event update business view
            foreach (var entBusiness in _filterEventUiUpdateBusinessView.Value)
            {
                ref var compBusiness = ref _poolBusiness.Value.Get(entBusiness);
                var price = compBusiness.CurrentLevelUpPrice;
                compBusiness.UiView.TextLevelUpPrice.text = "÷ÂÌ‡:" + price.ToString() + "$";
                compBusiness.UiView.TextLevelVal.text = compBusiness.Level.ToString();
                compBusiness.UiView.TextEarnVal.text = compBusiness.EarnVal.ToString() + "$";
                // ÛÔÎÂÌÓ
                if (compBusiness.IsUpgrade1Applyed)
                    compBusiness.UiView.TextUpgradeFirstPriceVal.text = " ”œÀ≈ÕŒ";
                if (compBusiness.IsUpgrade2Applyed)
                    compBusiness.UiView.TextUpgradeSecondPriceVal.text = " ”œÀ≈ÕŒ";
                //”‰‡ÎËÚ¸ Ó·‡·ÓÚ‡ÌÌ˚È Event
                _poolventUiBusinessViewUpdate.Value.Del(entBusiness);
            }

            //Update progress bar
            foreach (var entBusiness in _filterTagOwnedBusiness.Value)
            {
                ref var compBusiness = ref _poolBusiness.Value.Get(entBusiness);
                compBusiness.UiView.SliderProgressEarning.value = 1F - compBusiness.BusinessProgressEarning;
            }
        }

        private void UpdateWalletView(int money)
        {
            _viewWndGame.TextWalletMoneyCount.text = money.ToString() + "$";
        }

        private void CleanupOldItemsList(ViewWndGame view)
        {
            //Clean up old items list
            var parent = view.UiContentBusinessList;
            foreach (Transform item in parent)
            {
                GameObject.Destroy(item.gameObject);
            }
        }

        #region Item Business Buttons delegates
        private void OnClickLevelUp(int entity)
        {
            Debug.Log("OnClickLevelUp : entity =" + entity);
            _poolEventOnLevelUpClicked.Value.Add(entity);
        }

        private void OnClickUpgradeFirst(int entity)
        {
            Debug.Log("OnClickUpgradeFirst : entity =" + entity);
            _poolEventOnUpgrade1Clicked.Value.Add(entity);
        }

        private void OnClickUpgradeSecond(int entity)
        {
            Debug.Log("OnClickUpgradeSecond : entity =" + entity);
            _poolEventOnUpgrade2Clicked.Value.Add(entity);
        }
        #endregion

        #region EXIT

        private void RunExitButtons(IEcsSystems systems)
        {
            //Event exit clicked
            foreach (var ent in _filterEventOnClickedExit.Value)
            {
                Debug.Log("EcsRunSysWndGame : Exit");
                //Show popup advanced operations selection
                _viewWndGame.PopupExit.SetActive(true);
                _poolEventOnClickedExit.Value.Del(ent);
                Time.timeScale = 0;
            }

            //Event save and exit clicked
            foreach (var ent in _filterEventOnClickedSaveAndExit.Value)
            {
                Debug.Log("EcsRunSysWndGame : Save and exit");
                _viewWndGame.PopupExit.SetActive(false);
                _poolEventSaveData.Value.Add(ent);
                _poolEventOnClickedExitSave.Value.Del(ent);
            }

            //Event clean and exit clicked
            foreach (var ent in _filterEventOnClickedCleanAndExit.Value)
            {
                Debug.Log("EcsRunSysWndGame : Clean and exit");
                _viewWndGame.PopupExit.SetActive(false);
                _poolEventCleanSavedData.Value.Add(ent);
                _poolEventOnClickedExitClean.Value.Del(ent);
            }

            //Event back clicked
            //Event clean and exit clicked
            foreach (var ent in _filterEventOnClickedExitBack.Value)
            {
                Debug.Log("EcsRunSysWndGame : back to game");
                _viewWndGame.PopupExit.SetActive(false);
                _poolEventOnClickedExitBack.Value.Del(ent);
                Time.timeScale = 1;
            }
        }

        private void SetupExitButtons(int ent)
        {
            _viewWndGame.ButtonExit.onClick.AddListener(delegate { OnClickExit(ent); });
            _viewWndGame.ButtonExitSave.onClick.AddListener(delegate { OnClickExitSave(ent); });
            _viewWndGame.ButtonExitClean.onClick.AddListener(delegate { OnClickExitClean(ent); });
            _viewWndGame.ButtonBack.onClick.AddListener(delegate { OnClickBack(ent); });
        }
        
        private void OnClickExit(int entity)
        {
            Debug.Log("OnClickExit : entity =" + entity);
            _poolEventOnClickedExit.Value.Add(entity);
        }

        private void OnClickBack(int entity)
        {
            Debug.Log("OnClickBack : entity =" + entity);
            _poolEventOnClickedExitBack.Value.Add(entity);
        }

        private void OnClickExitSave(int entity)
        {
            Debug.Log("OnClickExitSave : entity =" + entity);
            _poolEventOnClickedExitSave.Value.Add(entity);
        }

        private void OnClickExitClean(int entity)
        {
            Debug.Log("OnClickExitClean : entity =" + entity);
            _poolEventOnClickedExitClean.Value.Add(entity);
        }

        #endregion
    }
}
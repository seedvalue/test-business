using Leopotam.EcsLite;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using System.ComponentModel;
using UnityEngine;

namespace Client
{
    sealed class EcsStartup : MonoBehaviour
    {
        [SerializeField] GameCfg _gameConfig;

        [Header("Ui:")]
        [SerializeField] EcsUguiEmitter _uguiEmitter;

        EcsWorld _world;
        IEcsSystems _systems;

        void Start()
        {
            Application.targetFrameRate = 60;
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);
            _systems
                .Add(new EcsRunSysAppQuit())
                .Add(new EcsInitSysApp())
                //Earn money
                .Add(new EcsRunSysEarning())
                .Add(new EcsRunSysWallet())
                 //Business
                .Add(new EcsRunSysBusinessLevelUp())
                .Add(new EcsRunSysBusinessUpgrade())
                .Add(new EcsRunSysSaveRestore())

                //UI
                .Add(new EcsRunSysWndGame())
                .Add(new EcsRunSysUiItemBusinessButtonsLock())

#if UNITY_EDITOR
                // add debug systems for custom worlds here, for example:
                // .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ("events"))
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .DelHere<EcsEventOnLevelUpClicked>()
                .DelHere<EcsEventOnUpgrade1Clicked>()
                .DelHere<EcsEventOnUpgrade2Clicked>()
                .DelHere<EcsEventWalletUpdated>()
                .DelHere<EcsEventEarned>()

                .Inject()
                .Inject(_gameConfig)
                .InjectUgui(_uguiEmitter)
                .Init();
        }

        void Update()
        {
            _systems?.Run();
        }

        void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.Destroy();
                _systems = null;
            }

            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }
    }
}
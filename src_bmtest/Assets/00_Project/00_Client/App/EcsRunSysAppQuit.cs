using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class EcsRunSysAppQuit : IEcsRunSystem {
        readonly EcsFilterInject<Inc<EcsEventQuitApp>> _filterEventQuitApp = default;
        readonly EcsPoolInject<EcsEventQuitApp> _poolrEventQuitApp = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filterEventQuitApp.Value)
            {
                Debug.Log("EcsRunSysQuitApp : Application.Quit()");
                Application.Quit();
                _poolrEventQuitApp.Value.Del(entity);
            }
        }
    }
}
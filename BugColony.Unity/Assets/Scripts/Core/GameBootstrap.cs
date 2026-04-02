using UnityEngine;
using VContainer.Unity;
using BugColony.Factory;
using BugColony.Systems;
using BugColony.UI;

namespace BugColony.Core
{
    public class GameBootstrap : IStartable, ITickable
    {
        private readonly ColonyManager _colonyManager;
        private readonly ResourceSpawner _resourceSpawner;
        private readonly LifetimeSystem _lifetimeSystem;
        private readonly UIManager _uiManager;
        private readonly GameBootstrapConfig _bootstrapConfig;

        public GameBootstrap(
            ColonyManager colonyManager,
            ResourceSpawner resourceSpawner,
            LifetimeSystem lifetimeSystem,
            UIManager uiManager,
            GameBootstrapConfig bootstrapConfig)
        {
            _colonyManager = colonyManager;
            _resourceSpawner = resourceSpawner;
            _lifetimeSystem = lifetimeSystem;
            _uiManager = uiManager;
            _bootstrapConfig = bootstrapConfig;
        }

        public void Start()
        {
            Debug.Log("[GameBootstrap] Bug Colony Game started!");

            // Spawn initial colony
            for (int i = 0; i < _bootstrapConfig.InitialWorkerCount; i++)
            {
                Vector3 position = new(
                    Random.Range(_bootstrapConfig.InitialSpawnRangeMin, _bootstrapConfig.InitialSpawnRangeMax),
                    0f,
                    Random.Range(_bootstrapConfig.InitialSpawnRangeMin, _bootstrapConfig.InitialSpawnRangeMax));
                _colonyManager.SpawnBug(BugType.Worker, position);
            }

            for (int i = 0; i < _bootstrapConfig.InitialPredatorCount; i++)
            {
                Vector3 position = new(
                    Random.Range(_bootstrapConfig.InitialSpawnRangeMin, _bootstrapConfig.InitialSpawnRangeMax),
                    0f,
                    Random.Range(_bootstrapConfig.InitialSpawnRangeMin, _bootstrapConfig.InitialSpawnRangeMax));
                _colonyManager.SpawnBug(BugType.Predator, position);
            }

            Debug.Log($"[GameBootstrap] Spawned {_colonyManager.TotalAlive} bugs");
        }

        public void Tick()
        {
            _resourceSpawner.Update(Time.deltaTime);
            _lifetimeSystem.Update(Time.deltaTime);
            _uiManager.Update();
        }
    }
}

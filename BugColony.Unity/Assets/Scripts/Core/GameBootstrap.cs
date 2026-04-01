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

        public GameBootstrap(
            ColonyManager colonyManager,
            ResourceSpawner resourceSpawner,
            LifetimeSystem lifetimeSystem,
            UIManager uiManager)
        {
            _colonyManager = colonyManager;
            _resourceSpawner = resourceSpawner;
            _lifetimeSystem = lifetimeSystem;
            _uiManager = uiManager;
        }

        public void Start()
        {
            Debug.Log("[GameBootstrap] Bug Colony Game started!");

            // Spawn initial colony
            for (int i = 0; i < 5; i++)
            {
                Vector3 position = new(
                    Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
                _colonyManager.SpawnBug(BugType.Worker, position);
            }

            for (int i = 0; i < 2; i++)
            {
                Vector3 position = new(
                    Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
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

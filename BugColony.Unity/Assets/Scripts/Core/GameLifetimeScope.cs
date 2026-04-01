using UnityEngine;
using VContainer;
using VContainer.Unity;
using BugColony.Bugs;
using BugColony.Factory;
using BugColony.Pool;
using BugColony.Systems;
using BugColony.UI;

namespace BugColony.Core
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Bug Prefabs")]
        [SerializeField] private WorkerBug workerBugPrefab;
        [SerializeField] private PredatorBug predatorBugPrefab;

        [Header("Resource Prefabs")]
        [SerializeField] private Resource resourcePrefab;

        [Header("Pool Settings")]
        [SerializeField] private int initialBugPoolSize = 20;
        [SerializeField] private int initialResourcePoolSize = 30;

        protected override void Configure(IContainerBuilder builder)
        {
            // Pool parents
            var bugPoolParent = new GameObject("BugPool").transform;
            var resourcePoolParent = new GameObject("ResourcePool").transform;

            // Pools
            var workerPool = new BugPool(workerBugPrefab, bugPoolParent, initialBugPoolSize);
            var predatorPool = new BugPool(predatorBugPrefab, bugPoolParent, initialBugPoolSize);
            var resourcePool = new ResourcePool(resourcePrefab, resourcePoolParent, initialResourcePoolSize);

            builder.RegisterInstance(resourcePool);

            // Bug Factory
            var bugFactory = new BugFactory();
            bugFactory.RegisterPool(BugType.Worker, workerPool);
            bugFactory.RegisterPool(BugType.Predator, predatorPool);
            builder.RegisterInstance(bugFactory);

            // Resource Factory
            builder.Register<ResourceFactory>(Lifetime.Singleton)
                .WithParameter(resourcePool);

            // Systems
            builder.Register<ColonyManager>(Lifetime.Singleton)
                .WithParameter(bugFactory);

            builder.Register<ResourceSpawner>(Lifetime.Singleton);
            builder.Register<MutationSystem>(Lifetime.Singleton);
            builder.Register<LifetimeSystem>(Lifetime.Singleton);

            // UI
            builder.Register<UIManager>(Lifetime.Singleton);

            // Entry Point
            builder.RegisterEntryPoint<GameBootstrap>();
        }
    }
}

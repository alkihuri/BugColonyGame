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
        [Header("Bug Prefabs")] [SerializeField]
        private WorkerBug workerBugPrefab;

        [SerializeField] private PredatorBug predatorBugPrefab;

        [Header("Resource Prefabs")] [SerializeField]
        private Resource resourcePrefab;

        [Header("Pool Settings")] [SerializeField]
        private int initialBugPoolSize = 20;

        [SerializeField] private int initialResourcePoolSize = 30;

        [Header("UI")] [SerializeField] private UiController uiPrefab;

        [Header("Configs")]
        [SerializeField] private BugsConfig bugsConfig;
        [SerializeField] private ColonyConfig colonyConfig;
        [SerializeField] private ResourceSpawnerConfig resourceSpawnerConfig;
        [SerializeField] private GameBootstrapConfig gameBootstrapConfig;


        protected override void Configure(IContainerBuilder builder)
        {
            // Pool parents
            var bugPoolParent = new GameObject("BugPool").transform;
            var resourcePoolParent = new GameObject("ResourcePool").transform;

            // Create a deferred injector: inject() will be set after the container builds.
            // Pools call it for every new instance they create (prewarm + runtime growth).
            var injector = new DeferredInjector();

            // Worker pool — each new WorkerBug instance gets VContainer injection applied
            var workerPool = new BugPool(
                workerBugPrefab, bugPoolParent, initialBugPoolSize,
                onCreated: instance => injector.Inject(instance));

            // Predator pool — no [Inject] fields currently, callback is a no-op but future-proof
            var predatorPool = new BugPool(
                predatorBugPrefab, bugPoolParent, initialBugPoolSize,
                onCreated: instance => injector.Inject(instance));

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

            // Configs
            builder.RegisterInstance(bugsConfig);
            builder.RegisterInstance(colonyConfig);
            builder.RegisterInstance(resourceSpawnerConfig);
            builder.RegisterInstance(gameBootstrapConfig);

            // Systems
            builder.Register<ColonyManager>(Lifetime.Singleton)
                .WithParameter(bugFactory);

            builder.Register<ResourceSpawner>(Lifetime.Singleton);
            builder.Register<MutationSystem>(Lifetime.Singleton);
            builder.Register<LifetimeSystem>(Lifetime.Singleton);

            // UI 
            builder.RegisterInstance<UiController>(uiPrefab);
            builder.Register<UIManager>(Lifetime.Singleton);


            // Entry Point
            builder.RegisterEntryPoint<GameBootstrap>();

            // After the container is built, wire the injector so pooled instances
            // created during prewarm AND at runtime all receive proper DI.
            builder.RegisterBuildCallback(container =>
            {
                injector.SetResolver(container);

                // Retroactively inject into already-prewarmed instances
                workerPool.InjectAll(container);
                predatorPool.InjectAll(container);
            });
        }

        /// <summary>
        /// Holds a resolver reference that is set after the container builds.
        /// Passed as a callback into ObjectPool so injection runs on every
        /// newly created instance, including those spawned at runtime.
        /// </summary>
        private class DeferredInjector
        {
            private IObjectResolver _resolver;

            public void SetResolver(IObjectResolver resolver) => _resolver = resolver;

            public void Inject(object instance)
            {
                // Resolver may be null during prewarm (container not built yet).
                // InjectAll() handles the prewarm batch retroactively.
                _resolver?.Inject(instance);
            }
        }
    }
}
using UnityEngine;
using VContainer;
using BugColony.Bugs.Behaviors;
using BugColony.Core;
using BugColony.Systems;

namespace BugColony.Bugs
{
    public class WorkerBug : BugBase
    {
        private const int ResourcesRequiredToSplit = 2;

        private int _resourcesEaten;
        private ColonyManager _colonyManager;

        // VContainer injects this after the MonoBehaviour is instantiated from the pool
        [Inject]
        public void Construct(ColonyManager colonyManager)
        {
            _colonyManager = colonyManager;
        }

        protected override void Awake()
        {
            base.Awake();
            SetBehavior(new WorkerBehavior());
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            _resourcesEaten = 0; // reset counter each time this bug is taken from the pool
        }

        public override void Eat(IResource resource)
        {
            if (resource == null || resource.IsConsumed) return;
            base.Eat(resource);

            _resourcesEaten++;
            Debug.Log($"[WorkerBug] {name} has eaten {_resourcesEaten}/{ResourcesRequiredToSplit} resources.");

            if (_resourcesEaten >= ResourcesRequiredToSplit)
            {
                _resourcesEaten = 0;
                TrySplit();
            }
        }

        private void TrySplit()
        {
            if (_colonyManager == null)
            {
                Debug.LogWarning("[WorkerBug] ColonyManager not injected — split skipped.");
                return;
            }

            _colonyManager.SplitWorker(this);
        }
    }
}

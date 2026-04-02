using System.Collections.Generic;
using BugColony.Bugs;
using BugColony.Factory;

namespace BugColony.Systems
{
    public class LifetimeSystem
    {
        private readonly ColonyManager _colonyManager;
        private readonly float _deadBugCleanupDelay;
        private readonly Dictionary<BugBase, float> _deadTimers = new();

        public LifetimeSystem(ColonyManager colonyManager, BugsConfig bugsConfig)
        {
            _colonyManager = colonyManager;
            _deadBugCleanupDelay = bugsConfig.DeadBugCleanupDelay;
            _colonyManager.OnBugDied += OnBugDied;
        }

        private void OnBugDied(BugBase bug)
        {
            _deadTimers[bug] = 0f;
        }

        public void Update(float deltaTime)
        {
            var toCleanup = new List<BugBase>();
            var keys = new List<BugBase>(_deadTimers.Keys);

            foreach (var key in keys)
            {
                float elapsed = _deadTimers[key] + deltaTime;
                _deadTimers[key] = elapsed;
                if (elapsed >= _deadBugCleanupDelay)
                {
                    toCleanup.Add(key);
                }
            }

            foreach (var bug in toCleanup)
            {
                _deadTimers.Remove(bug);
                BugType type = bug is PredatorBug ? BugType.Predator : BugType.Worker;
                _colonyManager.RecycleBug(type, bug);
            }
        }
    }
}

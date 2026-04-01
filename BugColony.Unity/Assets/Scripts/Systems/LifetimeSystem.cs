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

        public LifetimeSystem(ColonyManager colonyManager, float deadBugCleanupDelay = 5f)
        {
            _colonyManager = colonyManager;
            _deadBugCleanupDelay = deadBugCleanupDelay;
            _colonyManager.OnBugDied += OnBugDied;
        }

        private void OnBugDied(BugBase bug)
        {
            _deadTimers[bug] = 0f;
        }

        public void Update(float deltaTime)
        {
            var toCleanup = new List<BugBase>();

            foreach (var kvp in _deadTimers)
            {
                _deadTimers[kvp.Key] = kvp.Value + deltaTime;
                if (_deadTimers[kvp.Key] >= _deadBugCleanupDelay)
                {
                    toCleanup.Add(kvp.Key);
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

using System;
using System.Collections.Generic;
using BugColony.Bugs;
using BugColony.Factory;

namespace BugColony.Systems
{
    public class ColonyManager
    {
        private readonly List<BugBase> _aliveBugs = new();
        private readonly List<BugBase> _deadBugs = new();
        private readonly BugFactory _bugFactory;

        public IReadOnlyList<BugBase> AliveBugs => _aliveBugs;
        public IReadOnlyList<BugBase> DeadBugs => _deadBugs;
        public int TotalAlive => _aliveBugs.Count;
        public int TotalDead => _deadBugs.Count;

        public event Action<BugBase> OnBugSpawned;
        public event Action<BugBase> OnBugDied;

        public ColonyManager(BugFactory bugFactory)
        {
            _bugFactory = bugFactory;
        }

        public BugBase SpawnBug(BugType type, UnityEngine.Vector3 position)
        {
            BugBase bug = _bugFactory.Create(type, position);
            _aliveBugs.Add(bug);
            bug.OnDeath += HandleBugDeath;
            OnBugSpawned?.Invoke(bug);
            return bug;
        }

        public void RecycleBug(BugType type, BugBase bug)
        {
            _aliveBugs.Remove(bug);
            _deadBugs.Remove(bug);
            bug.OnDeath -= HandleBugDeath;
            _bugFactory.Recycle(type, bug);
        }

        private void HandleBugDeath(BugBase bug)
        {
            _aliveBugs.Remove(bug);
            _deadBugs.Add(bug);
            OnBugDied?.Invoke(bug);
        }
    }
}

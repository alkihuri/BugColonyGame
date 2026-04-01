using System;
using System.Collections.Generic;
using UnityEngine;
using BugColony.Bugs;
using BugColony.Factory;

namespace BugColony.Systems
{
    public class ColonyManager
    {
        private const int MutationColonyThreshold = 10;
        private const float MutationChance = 0.1f;
        private const float SplitSpawnRadius = 1.5f;

        private readonly List<BugBase> _aliveBugs = new();
        private readonly List<BugBase> _deadBugs = new();
        private readonly BugFactory _bugFactory;

        public IReadOnlyList<BugBase> AliveBugs => _aliveBugs;
        public IReadOnlyList<BugBase> DeadBugs => _deadBugs;
        public int TotalAlive => _aliveBugs.Count;
        public int TotalDead => _deadBugs.Count;

        public event Action<BugBase> OnBugSpawned;
        public event Action<BugBase> OnBugDied;
        public event Action<BugBase, BugBase> OnBugSplit; // (parent, offspring)

        public ColonyManager(BugFactory bugFactory)
        {
            _bugFactory = bugFactory;
        }

        public BugBase SpawnBug(BugType type, Vector3 position)
        {
            BugBase bug = _bugFactory.Create(type, position);
            RegisterBug(bug);
            OnBugSpawned?.Invoke(bug);
            return bug;
        }

        /// <summary>
        /// Called when a WorkerBug has eaten enough resources to split.
        /// Spawns 2 offspring near the parent position.
        /// If colony size > 10, each offspring has a 10% chance to mutate into a PredatorBug.
        /// The parent is then despawned back into the pool.
        /// </summary>
        public void SplitWorker(WorkerBug parent)
        {
            if (parent == null || !parent.IsAlive) return;

            Vector3 origin = parent.transform.position;
            bool colonyLarge = TotalAlive > MutationColonyThreshold;

            Debug.Log($"[ColonyManager] Worker {parent.name} splits! Colony size: {TotalAlive}, mutation eligible: {colonyLarge}");

            for (int i = 0; i < 2; i++)
            {
                // Scatter offspring in a small radius around the parent
                Vector3 offset = new Vector3(
                    UnityEngine.Random.Range(-SplitSpawnRadius, SplitSpawnRadius),
                    0f,
                    UnityEngine.Random.Range(-SplitSpawnRadius, SplitSpawnRadius));

                BugBase offspring;

                if (colonyLarge && UnityEngine.Random.value < MutationChance)
                {
                    // Mutate into a predator
                    offspring = SpawnBug(BugType.Predator, origin + offset);
                    Debug.Log($"[ColonyManager] Offspring {offspring.name} MUTATED into a Predator!");
                }
                else
                {
                    offspring = SpawnBug(BugType.Worker, origin + offset);
                }

                OnBugSplit?.Invoke(parent, offspring);
            }

            // Despawn the parent back into the pool
            DespawnBug(BugType.Worker, parent);
        }

        public void DespawnBug(BugType type, BugBase bug)
        {
            _aliveBugs.Remove(bug);
            _deadBugs.Remove(bug);
            bug.OnDeath -= HandleBugDeath;
            _bugFactory.Recycle(type, bug);
        }

        // Keep old name for backward compatibility
        public void RecycleBug(BugType type, BugBase bug) => DespawnBug(type, bug);

        private void RegisterBug(BugBase bug)
        {
            _aliveBugs.Add(bug);
            bug.OnDeath += HandleBugDeath;
        }

        private void HandleBugDeath(BugBase bug)
        {
            _aliveBugs.Remove(bug);
            _deadBugs.Add(bug);
            OnBugDied?.Invoke(bug);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using BugColony.Bugs;
using BugColony.Factory;
using Random = UnityEngine.Random;

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
        public event Action<BugBase, BugBase> OnBugSplit;

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
        /// Worker ate 2 resources → splits into 2 workers.
        /// If colony > 10 bugs each offspring has 10% chance to mutate into a predator.
        /// Parent is despawned.
        /// </summary>
        public void SplitWorker(WorkerBug parent)
        {
            if (parent == null || !parent.IsAlive) return;

            Vector3 origin = parent.transform.position;
            bool colonyLarge = TotalAlive > MutationColonyThreshold;

            Debug.Log($"[ColonyManager] Worker {parent.name} splits! Colony: {TotalAlive}, mutation eligible: {colonyLarge}");

            for (int i = 0; i < 2; i++)
            {
                Vector3 offset = RandomOffset(SplitSpawnRadius);
                BugBase offspring;

                if (colonyLarge && Random.value < MutationChance)
                {
                    offspring = SpawnBug(BugType.Predator, origin + offset);
                    Debug.Log($"[ColonyManager] Offspring {offspring.name} MUTATED into a Predator!");
                }
                else
                {
                    offspring = SpawnBug(BugType.Worker, origin + offset);
                }

                OnBugSplit?.Invoke(parent, offspring);
            }

            DespawnBug(BugType.Worker, parent);
        }

        /// <summary>
        /// Predator ate/killed 3 targets → splits into 2 predators with fresh timers.
        /// Parent is despawned.
        /// </summary>
        public void SplitPredator(PredatorBug parent)
        {
            if (parent == null || !parent.IsAlive) return;

            Vector3 origin = parent.transform.position;

            Debug.Log($"[ColonyManager] Predator {parent.name} splits!");

            for (int i = 0; i < 2; i++)
            {
                Vector3 offset = RandomOffset(SplitSpawnRadius);
                BugBase offspring = SpawnBug(BugType.Predator, origin + offset);
                OnBugSplit?.Invoke(parent, offspring);
            }

            DespawnBug(BugType.Predator, parent);
        }

        public void DespawnBug(BugType type, BugBase bug)
        {
            _aliveBugs.Remove(bug);
            _deadBugs.Remove(bug);
            bug.OnDeath -= HandleBugDeath;
            _bugFactory.Recycle(type, bug);
        }

        // Backward-compatible alias
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

            // Colony rule: if no bugs remain, spawn a rescue worker
            if (_aliveBugs.Count == 0)
            {
                Debug.Log("[ColonyManager] Colony wiped out — spawning rescue worker.");
                SpawnBug(BugType.Worker, Vector3.zero);
            }
        }

        private static Vector3 RandomOffset(float radius) =>
            new(Random.Range(-radius, radius), 0f, Random.Range(-radius, radius));
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using BugColony.Bugs;
using BugColony.Pool;

namespace BugColony.Factory
{
    public enum BugType
    {
        Worker,
        Predator
    }

    public class BugFactory
    {
        private readonly Dictionary<BugType, BugPool> _pools = new();

        public void RegisterPool(BugType type, BugPool pool)
        {
            _pools[type] = pool;
        }

        public BugBase Create(BugType type, Vector3 position)
        {
            if (!_pools.TryGetValue(type, out var pool))
            {
                throw new InvalidOperationException(
                    $"No pool registered for bug type: {type}");
            }

            BugBase bug = pool.Get();
            bug.transform.position = position;
            return bug;
        }

        public void Recycle(BugType type, BugBase bug)
        {
            if (_pools.TryGetValue(type, out var pool))
            {
                pool.Return(bug);
            }
        }
    }
}

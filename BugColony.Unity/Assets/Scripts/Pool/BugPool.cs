using System;
using UnityEngine;
using BugColony.Bugs;

namespace BugColony.Pool
{
    public class BugPool : ObjectPool<BugBase>
    {
        public BugPool(BugBase prefab, Transform parent, int initialSize = 10, Action<BugBase> onCreated = null)
            : base(prefab, parent, initialSize, onCreated)
        {
        }
    }
}

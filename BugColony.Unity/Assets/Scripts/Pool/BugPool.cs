using UnityEngine;
using BugColony.Bugs;

namespace BugColony.Pool
{
    public class BugPool : ObjectPool<BugBase>
    {
        public BugPool(BugBase prefab, Transform parent, int initialSize = 10)
            : base(prefab, parent, initialSize)
        {
        }
    }
}

using UnityEngine;

namespace BugColony.Pool
{
    public class ResourcePool : ObjectPool<Resource>
    {
        public ResourcePool(Resource prefab, Transform parent, int initialSize = 20)
            : base(prefab, parent, initialSize)
        {
        }
    }
}

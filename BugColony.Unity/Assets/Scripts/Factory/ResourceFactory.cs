using UnityEngine;
using BugColony.Pool;

namespace BugColony.Factory
{
    public class ResourceFactory
    {
        private readonly ResourcePool _pool;

        public ResourceFactory(ResourcePool pool)
        {
            _pool = pool;
        }

        public Resource Create(Vector3 position)
        {
            Resource resource = _pool.Get();
            resource.transform.position = position;
            return resource;
        }

        public void Recycle(Resource resource)
        {
            _pool.Return(resource);
        }
    }
}

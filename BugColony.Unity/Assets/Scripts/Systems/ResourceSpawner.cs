using UnityEngine;
using BugColony.Factory;
using VContainer;

namespace BugColony.Systems
{
    public class ResourceSpawner
    {
        private readonly ResourceFactory _resourceFactory;
        private readonly float _spawnInterval = 2f;
        private readonly Vector3 _spawnAreaMin = new(-20f, 0f, -20f);
        private readonly Vector3 _spawnAreaMax = new(20f, 0f, 20f);
        private float _timer;

        [Inject]
        public ResourceSpawner(ResourceFactory resourceFactory)
        {
            _resourceFactory = resourceFactory;
        }


        public void Update(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer >= _spawnInterval)
            {
                _timer = 0f;
                SpawnResource();
            }
        }

        private void SpawnResource()
        {
            Vector3 position = new(
                Random.Range(_spawnAreaMin.x, _spawnAreaMax.x),
                0f,
                Random.Range(_spawnAreaMin.z, _spawnAreaMax.z)
            );
            _resourceFactory.Create(position);
        }
    }
}
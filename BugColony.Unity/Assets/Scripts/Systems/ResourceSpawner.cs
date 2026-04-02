using UnityEngine;
using BugColony.Factory;
using VContainer;

namespace BugColony.Systems
{
    public class ResourceSpawner
    {
        private readonly ResourceFactory _resourceFactory;
        private readonly float _spawnInterval;
        private readonly Vector3 _spawnAreaMin;
        private readonly Vector3 _spawnAreaMax;
        private float _timer;

        [Inject]
        public ResourceSpawner(ResourceFactory resourceFactory, ResourceSpawnerConfig config)
        {
            _resourceFactory = resourceFactory;
            _spawnInterval = config.SpawnInterval;
            _spawnAreaMin = config.SpawnAreaMin;
            _spawnAreaMax = config.SpawnAreaMax;
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
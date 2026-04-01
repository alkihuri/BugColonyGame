using UnityEngine;
using BugColony.Factory;

namespace BugColony.Systems
{
    public class ResourceSpawner
    {
        private readonly ResourceFactory _resourceFactory;
        private readonly float _spawnInterval;
        private readonly Vector3 _spawnAreaMin;
        private readonly Vector3 _spawnAreaMax;
        private float _timer;

        public ResourceSpawner(
            ResourceFactory resourceFactory,
            float spawnInterval = 2f,
            Vector3 spawnAreaMin = default,
            Vector3 spawnAreaMax = default)
        {
            _resourceFactory = resourceFactory;
            _spawnInterval = spawnInterval;
            _spawnAreaMin = spawnAreaMin == default ? new Vector3(-20f, 0f, -20f) : spawnAreaMin;
            _spawnAreaMax = spawnAreaMax == default ? new Vector3(20f, 0f, 20f) : spawnAreaMax;
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

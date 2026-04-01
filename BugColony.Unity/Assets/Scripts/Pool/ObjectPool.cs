using System.Collections.Generic;
using UnityEngine;
using BugColony.Core;

namespace BugColony.Pool
{
    public class ObjectPool<T> where T : MonoBehaviour, ISpawnable
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Queue<T> _available = new();
        private readonly HashSet<T> _inUse = new();

        public int CountAvailable => _available.Count;
        public int CountInUse => _inUse.Count;
        public int CountTotal => _available.Count + _inUse.Count;

        public ObjectPool(T prefab, Transform parent, int initialSize = 10)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                Prewarm();
            }
        }

        private T CreateInstance()
        {
            T instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void Prewarm()
        {
            T instance = CreateInstance();
            _available.Enqueue(instance);
        }

        public T Get()
        {
            T instance = _available.Count > 0 ? _available.Dequeue() : CreateInstance();
            _inUse.Add(instance);
            instance.OnSpawn();
            return instance;
        }

        public void Return(T instance)
        {
            if (!_inUse.Remove(instance)) return;
            instance.OnDespawn();
            _available.Enqueue(instance);
        }

        public void ReturnAll()
        {
            var active = new List<T>(_inUse);
            foreach (var instance in active)
            {
                Return(instance);
            }
        }
    }
}

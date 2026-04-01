using UnityEngine;
using BugColony.Core;

namespace BugColony
{
    public class Resource : MonoBehaviour, IResource
    {
        [SerializeField] private float nutrientValue = 20f;

        public float NutrientValue => nutrientValue;
        public bool IsConsumed { get; private set; }

        public void Consume()
        {
            if (IsConsumed) return;
            IsConsumed = true;
            gameObject.SetActive(false);
        }

        public void OnSpawn()
        {
            IsConsumed = false;
            gameObject.SetActive(true);
        }

        public void OnDespawn()
        {
            IsConsumed = true;
            gameObject.SetActive(false);
        }
    }
}

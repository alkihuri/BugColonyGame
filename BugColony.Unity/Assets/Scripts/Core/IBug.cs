using UnityEngine;

namespace BugColony.Core
{
    public interface IBug : ISpawnable
    {
        float Health { get; }
        float Energy { get; }
        float Speed { get; }
        bool IsAlive { get; }

        void Move(Vector3 direction);
        void Eat(IResource resource);
        void TakeDamage(float damage);
        void Die();
        void SetBehavior(IBugBehavior behavior);
        void SetState(IBugState state);
    }
}

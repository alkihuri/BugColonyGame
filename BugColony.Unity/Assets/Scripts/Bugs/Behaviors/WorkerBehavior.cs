using UnityEngine;
using BugColony.Core;
using BugColony.Bugs.States;

namespace BugColony.Bugs.Behaviors
{
    /// <summary>
    /// WorkerBug behavior:
    /// 1. OverlapSphere (5m) — if any PredatorBug nearby → FleeState (run away)
    /// 2. SphereCast forward (visionRange) — if resource spotted → ChaseState toward it
    /// 3. Otherwise — let MoveState / IdleState cycle run naturally
    /// </summary>
    public class WorkerBehavior : BugBehaviorBase
    {
        private readonly float _predatorDetectionRadius;
        private readonly float _visionRange;
        private readonly float _visionRadius;
        private readonly float _eatRange;

        public WorkerBehavior(
            float predatorDetectionRadius = 5f,
            float visionRange = 8f,
            float visionRadius = 1.5f,
            float eatRange = 0.8f)
        {
            _predatorDetectionRadius = predatorDetectionRadius;
            _visionRange = visionRange;
            _visionRadius = visionRadius;
            _eatRange = eatRange;
        }

        public override void Execute(IBug bug)
        {
            if (bug is not BugBase bugBase) return;

            var currentState = bugBase.StateMachine.CurrentState;
            if (currentState is DeadState) return;

            // --- Priority 1: Flee from nearby predators ---
            if (currentState is not FleeState)
            {
                if (HasNearbyPredator(bugBase))
                {
                    bug.SetState(new FleeState(_predatorDetectionRadius));
                    return;
                }
            }

            // Don't interrupt flee, eat, or chase with lower-priority logic
            if (currentState is FleeState or EatState or ChaseState) return;

            // --- Priority 2: SphereCast forward to spot resources ---
            if (TryFindResourceAhead(bugBase, out GameObject resourceObj))
            {
                bug.SetState(new ChaseState(resourceObj, _eatRange));
            }
        }

        // Pre-allocated buffers — avoid GC allocs each frame
        private static readonly Collider[] ThreatBuffer = new Collider[16];
        private static readonly RaycastHit[] VisionBuffer = new RaycastHit[32];

        // OverlapSphereNonAlloc to check if any predator is within detection radius
        private bool HasNearbyPredator(BugBase self)
        {
            int count = Physics.OverlapSphereNonAlloc(self.transform.position, _predatorDetectionRadius, ThreatBuffer);
            for (int i = 0; i < count; i++)
            {
                var col = ThreatBuffer[i];
                if (col.gameObject == self.gameObject) continue;
                var predator = col.GetComponent<PredatorBug>();
                if (predator != null && predator.IsAlive)
                    return true;
            }
            return false;
        }

        // SphereCastNonAlloc forward to spot a resource in vision cone
        private bool TryFindResourceAhead(BugBase self, out GameObject resourceObj)
        {
            resourceObj = null;
            Vector3 forward = self.transform.forward;
            if (forward == Vector3.zero) forward = Vector3.forward;

            int count = Physics.SphereCastNonAlloc(
                self.transform.position,
                _visionRadius,
                forward,
                VisionBuffer,
                _visionRange);

            float closestDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                var hit = VisionBuffer[i];
                if (hit.collider.gameObject == self.gameObject) continue;
                var res = hit.collider.GetComponent<IResource>();
                if (res != null && !res.IsConsumed)
                {
                    float d = hit.distance;
                    if (d < closestDist)
                    {
                        closestDist = d;
                        resourceObj = hit.collider.gameObject;
                    }
                }
            }

            return resourceObj != null;
        }
    }
}

using UnityEngine;
using BugColony.Core;
using BugColony.Bugs.States;

namespace BugColony.Bugs.Behaviors
{
    /// <summary>
    /// PredatorBug behavior:
    /// 1. SphereCast forward (visionRange) — if WorkerBug or Resource spotted → ChaseState
    /// 2. OverlapSphere (attackRange) — if prey in melee range → AttackState / EatState
    /// 3. Otherwise — let MoveState / IdleState cycle run naturally
    /// </summary>
    public class PredatorBehavior : BugBehaviorBase
    {
        private readonly float _visionRange;
        private readonly float _visionRadius;
        private readonly float _attackRange;
        private readonly float _attackDamage;

        public PredatorBehavior(
            float visionRange = 15f,
            float visionRadius = 2f,
            float attackRange = 1.2f,
            float attackDamage = 10f)
        {
            _visionRange = visionRange;
            _visionRadius = visionRadius;
            _attackRange = attackRange;
            _attackDamage = attackDamage;
        }

        public override void Execute(IBug bug)
        {
            if (bug is not BugBase bugBase) return;

            var currentState = bugBase.StateMachine.CurrentState;
            if (currentState is AttackState or DeadState or EatState) return;

            // --- Priority 1: Already in attack range? Engage immediately ---
            if (currentState is not ChaseState)
            {
                if (TryFindPreyInMeleeRange(bugBase, out GameObject meleeTarget))
                {
                    EngageTarget(bug, meleeTarget);
                    return;
                }
            }

            // Don't interrupt an active chase
            if (currentState is ChaseState) return;

            // --- Priority 2: SphereCast forward to spot prey ---
            if (TryFindPreyAhead(bugBase, out GameObject chaseable))
            {
                bug.SetState(new ChaseState(chaseable, _attackRange, _attackDamage));
            }
        }

        // Pre-allocated buffers — avoid GC allocs each frame
        private static readonly Collider[] MeleeBuffer = new Collider[16];
        private static readonly RaycastHit[] VisionBuffer = new RaycastHit[32];

        // OverlapSphereNonAlloc tight radius — immediate melee check
        private bool TryFindPreyInMeleeRange(BugBase self, out GameObject target)
        {
            target = null;
            int count = Physics.OverlapSphereNonAlloc(self.transform.position, _attackRange, MeleeBuffer);
            for (int i = 0; i < count; i++)
            {
                var col = MeleeBuffer[i];
                if (col.gameObject == self.gameObject) continue;

                // Prefer WorkerBug
                var worker = col.GetComponent<WorkerBug>();
                if (worker != null && worker.IsAlive)
                {
                    target = col.gameObject;
                    return true;
                }

                // Also eat resources
                var res = col.GetComponent<IResource>();
                if (res != null && !res.IsConsumed)
                {
                    target = col.gameObject;
                    return true;
                }
            }
            return false;
        }

        // SphereCastNonAlloc forward — vision cone detection for workers and resources
        private bool TryFindPreyAhead(BugBase self, out GameObject target)
        {
            target = null;
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

                // Prioritize WorkerBug
                var worker = hit.collider.GetComponent<WorkerBug>();
                if (worker != null && worker.IsAlive)
                {
                    if (hit.distance < closestDist)
                    {
                        closestDist = hit.distance;
                        target = hit.collider.gameObject;
                    }
                    continue;
                }

                // Also interested in resources
                var res = hit.collider.GetComponent<IResource>();
                if (res != null && !res.IsConsumed && hit.distance < closestDist)
                {
                    closestDist = hit.distance;
                    target = hit.collider.gameObject;
                }
            }

            return target != null;
        }

        private void EngageTarget(IBug bug, GameObject target)
        {
            var targetBug = target.GetComponent<IBug>();
            if (targetBug != null)
            {
                bug.SetState(new AttackState(targetBug, _attackDamage));
                return;
            }

            var res = target.GetComponent<IResource>();
            if (res != null)
                bug.SetState(new EatState(res));
        }
    }
}

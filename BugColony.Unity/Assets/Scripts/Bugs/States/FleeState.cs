using UnityEngine;
using BugColony.Core;

namespace BugColony.Bugs.States
{
    /// <summary>
    /// Worker flees away from the nearest PredatorBug within _threatDetectionRadius.
    /// Rechecks every frame via OverlapSphere and updates flee direction.
    /// Rotates the bug away from threat so SphereCast forward stays accurate.
    /// Transitions back to IdleState once no predator is found nearby.
    /// </summary>
    public class FleeState : BugStateBase
    {
        private readonly float _threatDetectionRadius;
        private readonly float _maxFleeDuration;
        private readonly float _rotationSpeed;
        private float _fleeTimer;
        private Vector3 _fleeDirection;

        public FleeState(float threatDetectionRadius = 5f, float maxFleeDuration = 3f, float rotationSpeed = 10f)
        {
            _threatDetectionRadius = threatDetectionRadius;
            _maxFleeDuration = maxFleeDuration;
            _rotationSpeed = rotationSpeed;
        }

        public override void Enter(IBug bug)
        {
            _fleeTimer = 0f;
            UpdateFleeDirection(bug);
        }

        public override void Execute(IBug bug)
        {
            if (bug is not BugBase bugBase) return;

            _fleeTimer += Time.deltaTime;

            Transform closestThreat = FindClosestPredator(bugBase);

            if (closestThreat != null)
            {
                // Flee directly away from the predator
                _fleeDirection = (bugBase.transform.position - closestThreat.position).normalized;
                _fleeDirection.y = 0f;
                _fleeTimer = 0f; // keep fleeing while threatened
            }

            if (closestThreat == null || _fleeTimer >= _maxFleeDuration)
            {
                bug.SetState(new IdleState());
                return;
            }

            // Rotate away from threat so forward direction stays accurate
            if (_fleeDirection != Vector3.zero)
            {
                bugBase.transform.rotation = Quaternion.Slerp(
                    bugBase.transform.rotation,
                    Quaternion.LookRotation(_fleeDirection),
                    _rotationSpeed * Time.deltaTime);
            }

            bug.Move(_fleeDirection);
        }

        public override void Exit(IBug bug) { }

        private void UpdateFleeDirection(IBug bug)
        {
            if (bug is not BugBase bugBase) return;
            Transform threat = FindClosestPredator(bugBase);
            if (threat != null)
            {
                _fleeDirection = (bugBase.transform.position - threat.position).normalized;
                _fleeDirection.y = 0f;
            }
            else
            {
                _fleeDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
            }
        }

        private Transform FindClosestPredator(BugBase self)
        {
            Collider[] hits = Physics.OverlapSphere(self.transform.position, _threatDetectionRadius);
            Transform closest = null;
            float minDist = float.MaxValue;

            foreach (var col in hits)
            {
                if (col.gameObject == self.gameObject) continue;
                var predator = col.GetComponent<PredatorBug>();
                if (predator != null && predator.IsAlive)
                {
                    float dist = Vector3.Distance(self.transform.position, col.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = col.transform;
                    }
                }
            }

            return closest;
        }
    }
}

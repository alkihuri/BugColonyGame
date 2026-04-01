using UnityEngine;
using BugColony.Core;
namespace BugColony.Bugs.States
{
    /// <summary>
    /// Predator chases a target (WorkerBug or Resource).
    /// Rotates toward target each frame so SphereCast forward stays accurate.
    /// When close enough -> transitions to AttackState or EatState.
    /// If target lost -> back to IdleState.
    /// </summary>
    public class ChaseState : BugStateBase
    {
        private readonly GameObject _targetObject;
        private readonly float _attackRange;
        private readonly float _attackDamage;
        private readonly float _rotationSpeed;

        private IBug _targetBug;
        private IResource _targetResource;

        public ChaseState(GameObject target, float attackRange = 1.2f, float attackDamage = 10f, float rotationSpeed = 10f)
        {
            _targetObject = target;
            _attackRange = attackRange;
            _attackDamage = attackDamage;
            _rotationSpeed = rotationSpeed;
        }

        public override void Enter(IBug bug)
        {
            if (_targetObject == null) return;
            _targetBug = _targetObject.GetComponent<IBug>();
            _targetResource = _targetObject.GetComponent<IResource>();
        }

        public override void Execute(IBug bug)
        {
            if (bug is not BugBase bugBase) return;

            if (_targetObject == null || !_targetObject.activeInHierarchy)
            {
                bug.SetState(new IdleState());
                return;
            }

            if (_targetBug != null && !_targetBug.IsAlive)
            {
                bug.SetState(new IdleState());
                return;
            }

            if (_targetResource != null && _targetResource.IsConsumed)
            {
                bug.SetState(new IdleState());
                return;
            }

            Vector3 toTarget = _targetObject.transform.position - bugBase.transform.position;
            float distance = toTarget.magnitude;

            if (distance <= _attackRange)
            {
                if (_targetBug != null)
                    bug.SetState(new AttackState(_targetBug, _attackDamage));
                else if (_targetResource != null)
                    bug.SetState(new EatState(_targetResource));
                return;
            }

            Vector3 direction = new Vector3(toTarget.x, 0f, toTarget.z).normalized;

            // Rotate toward target so SphereCast forward stays meaningful
            if (direction != Vector3.zero)
            {
                bugBase.transform.rotation = Quaternion.Slerp(
                    bugBase.transform.rotation,
                    Quaternion.LookRotation(direction),
                    _rotationSpeed * Time.deltaTime);
            }

            bug.Move(direction);
        }

        public override void Exit(IBug bug) { }
    }
}

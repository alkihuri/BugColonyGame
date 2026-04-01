using UnityEngine;
using BugColony.Core;

namespace BugColony.Bugs.States
{
    public class AttackState : BugStateBase
    {
        private readonly IBug _target;
        private float _attackCooldown;
        private readonly float _attackInterval;
        private readonly float _attackDamage;

        public AttackState(IBug target, float attackDamage = 10f, float attackInterval = 1f)
        {
            _target = target;
            _attackDamage = attackDamage;
            _attackInterval = attackInterval;
        }

        public override void Enter(IBug bug)
        {
            _attackCooldown = 0f;
        }

        public override void Execute(IBug bug)
        {
            if (_target == null || !_target.IsAlive)
            {
                bug.SetState(new IdleState());
                return;
            }

            _attackCooldown += Time.deltaTime;
            if (_attackCooldown >= _attackInterval)
            {
                bool wasAlive = _target.IsAlive;
                _target.TakeDamage(_attackDamage);
                _attackCooldown = 0f;

                // If this attack killed the target, notify the attacker (predator kill counter)
                if (wasAlive && !_target.IsAlive && bug is PredatorBug predator)
                {
                    predator.OnKill();
                }
            }
        }

        public override void Exit(IBug bug) { }
    }
}

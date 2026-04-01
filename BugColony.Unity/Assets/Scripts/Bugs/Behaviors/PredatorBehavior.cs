using UnityEngine;
using BugColony.Core;
using BugColony.Bugs.States;

namespace BugColony.Bugs.Behaviors
{
    public class PredatorBehavior : BugBehaviorBase
    {
        private readonly float _huntRadius;
        private readonly float _attackDamage;

        public PredatorBehavior(float huntRadius = 15f, float attackDamage = 10f)
        {
            _huntRadius = huntRadius;
            _attackDamage = attackDamage;
        }

        public override void Execute(IBug bug)
        {
            if (bug is not BugBase bugBase) return;
            if (bugBase.StateMachine.CurrentState is AttackState or DeadState) return;

            var colliders = Physics.OverlapSphere(
                bugBase.transform.position, _huntRadius);

            foreach (var col in colliders)
            {
                if (col.gameObject == bugBase.gameObject) continue;

                var target = col.GetComponent<IBug>();
                if (target != null && target.IsAlive)
                {
                    bug.SetState(new AttackState(target, _attackDamage));
                    return;
                }
            }
        }
    }
}

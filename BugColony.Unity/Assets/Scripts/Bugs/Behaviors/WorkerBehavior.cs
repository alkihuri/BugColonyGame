using UnityEngine;
using BugColony.Core;
using BugColony.Bugs.States;

namespace BugColony.Bugs.Behaviors
{
    public class WorkerBehavior : BugBehaviorBase
    {
        private readonly float _searchRadius;
        private readonly LayerMask _resourceLayer;

        public WorkerBehavior(float searchRadius = 0.4f)
        {
            _searchRadius = searchRadius;
        }

        public override void Execute(IBug bug)
        {
            if (bug is not BugBase bugBase) return;
            if (bugBase.StateMachine.CurrentState is EatState or DeadState) return;

            var colliders = Physics.OverlapSphere(
                bugBase.transform.position, _searchRadius);

            foreach (var col in colliders)
            {
                var resource = col.GetComponent<IResource>();
                if (resource != null && !resource.IsConsumed)
                {
                    bug.SetState(new EatState(resource));
                    return;
                }
            }
        }
    }
}

using UnityEngine;
using BugColony.Core;

namespace BugColony.Bugs.States
{
    public class EatState : BugStateBase
    {
        private readonly IResource _resource;
        private float _eatTimer;
        private readonly float _eatDuration;

        public EatState(IResource resource, float eatDuration = 1.5f)
        {
            _resource = resource;
            _eatDuration = eatDuration;
        }

        public override void Enter(IBug bug)
        {
            _eatTimer = 0f;
        }

        public override void Execute(IBug bug)
        {
            _eatTimer += Time.deltaTime;
            if (_eatTimer >= _eatDuration)
            {
                bug.Eat(_resource);
                bug.SetState(new IdleState());
            }
        }

        public override void Exit(IBug bug) { }
    }
}

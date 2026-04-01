using UnityEngine;
using BugColony.Core;

namespace BugColony.Bugs.States
{
    public class IdleState : BugStateBase
    {
        private float _idleTimer;
        private readonly float _idleDuration;

        public IdleState(float idleDuration = 0f)
        {
            _idleDuration = idleDuration;
        }

        public override void Enter(IBug bug)
        {
            _idleTimer = 0f;
        }

        public override void Execute(IBug bug)
        {
            _idleTimer += Time.deltaTime;
            if (_idleTimer >= _idleDuration)
            {
                bug.SetState(new MoveState());
            }
        }

        public override void Exit(IBug bug) { }
    }
}

using UnityEngine;
using BugColony.Core;

namespace BugColony.Bugs.States
{
    public class MoveState : BugStateBase
    {
        private Vector3 _targetDirection;
        private float _moveTimer;
        private readonly float _moveDuration;

        public MoveState(float moveDuration = 3f)
        {
            _moveDuration = moveDuration;
        }

        public override void Enter(IBug bug)
        {
            _moveTimer = 0f;
            _targetDirection = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            ).normalized;
        }

        public override void Execute(IBug bug)
        {
            _moveTimer += Time.deltaTime;
            bug.Move(_targetDirection);

            if (_moveTimer >= _moveDuration)
            {
                bug.SetState(new IdleState());
            }
        }

        public override void Exit(IBug bug) { }
    }
}

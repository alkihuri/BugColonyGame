using UnityEngine;
using BugColony.Core;

namespace BugColony.Bugs.States
{
    /// <summary>
    /// Random wandering state. The bug picks a random direction, rotates to face it,
    /// and walks for _moveDuration seconds before returning to IdleState.
    /// Rotating the bug is critical so that SphereCast forward in behaviors works correctly.
    /// </summary>
    public class MoveState : BugStateBase
    {
        private Vector3 _targetDirection;
        private float _moveTimer;
        private readonly float _moveDuration;
        private readonly float _rotationSpeed;

        public MoveState(float moveDuration = 3f, float rotationSpeed = 8f)
        {
            _moveDuration = moveDuration;
            _rotationSpeed = rotationSpeed;
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
            if (bug is not BugBase bugBase) return;

            _moveTimer += Time.deltaTime;

            // Smoothly rotate toward movement direction so forward SphereCast works
            if (_targetDirection != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(_targetDirection);
                bugBase.transform.rotation = Quaternion.Slerp(
                    bugBase.transform.rotation,
                    targetRot,
                    _rotationSpeed * Time.deltaTime);
            }

            bug.Move(_targetDirection);

            if (_moveTimer >= _moveDuration)
            {
                bug.SetState(new IdleState());

                Tools.FieldMeasurer.AddPoint(bugBase.transform.position);
            }
        }

        public override void Exit(IBug bug) { }
    }
}

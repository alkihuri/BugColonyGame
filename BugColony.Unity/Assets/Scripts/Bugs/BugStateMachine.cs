using BugColony.Core;

namespace BugColony.Bugs
{
    public class BugStateMachine
    {
        private IBugState _currentState;

        public IBugState CurrentState => _currentState;

        public void ChangeState(IBug bug, IBugState newState)
        {
            _currentState?.Exit(bug);
            _currentState = newState;
            _currentState?.Enter(bug);
        }

        public void Update(IBug bug)
        {
            _currentState?.Execute(bug);
        }
    }
}

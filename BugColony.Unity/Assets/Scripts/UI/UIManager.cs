using BugColony.Systems;

namespace BugColony.UI
{
    public class UIManager
    {
        private readonly ColonyManager _colonyManager;
        private readonly UiController _ui;

        public UIManager(ColonyManager colonyManager, UiController ui)
        {
            _colonyManager = colonyManager;
            _ui = ui;
        }

        public void Update()
        {
            if (_ui == null) return;

            if (_ui.aliveCountText != null)
                _ui.aliveCountText.text = $"Alive Bugs: {_colonyManager.TotalAlive}";

            

            if (_ui.deadWorkerCountText != null)
                _ui.deadWorkerCountText.text = $"Dead Workers: {_colonyManager.DeadWorkers}";

            if (_ui.deadPredatorCountText != null)
                _ui.deadPredatorCountText.text = $"Dead Predators: {_colonyManager.DeadPredators}";
        }
    }
}
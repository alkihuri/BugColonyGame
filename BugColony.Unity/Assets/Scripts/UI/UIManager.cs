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
            
            
            _colonyManager.DeadWorkers.OnValueChanged += UpdateDeadWorkersUi;
            _colonyManager.DeadPredators.OnValueChanged += UpdateDeadPredatorsUi;
            _colonyManager.TotalDead.OnValueChanged += UpdateTotalDeadUi;
            _colonyManager.TotalAlive.OnValueChanged += UpdateTotalAliveUi;
        }

        private void UpdateTotalAliveUi(int obj)
        {
            _ui.GetFreeOrNewField("TotalAliveText").text = $"Total Alive: {obj}";
        }

        private void UpdateTotalDeadUi(int obj)
        {
            _ui.GetFreeOrNewField("TotalDeadText").text = $"Total Dead: {obj}";
        }

        private void UpdateDeadWorkersUi(int obj)
        {
            _ui.GetFreeOrNewField("DeadWorkersText").text = $"Dead Workers: {obj}";
        }

        private void UpdateDeadPredatorsUi(int obj)
        { 
            _ui.GetFreeOrNewField("DeadPredators").text = $"Dead Predators: {obj}";
        }
        
        

        
        
        
        public void Update()
        { 
            
        }
    }
}
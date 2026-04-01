using UnityEngine;
using VContainer;
using BugColony.Bugs.Behaviors;
using BugColony.Core;
using BugColony.Systems;

namespace BugColony.Bugs
{
    public class PredatorBug : BugBase
    {
        private const int EatsRequiredToSplit = 3;
        private const float Lifetime = 10f;

        private int _eatCount;      // counts both resource eats and bug kills
        private float _lifeTimer;
        private ColonyManager _colonyManager;

        [Inject]
        public void Construct(ColonyManager colonyManager)
        {
            _colonyManager = colonyManager;
        }

        protected override void Awake()
        {
            base.Awake();
            SetBehavior(new PredatorBehavior());
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            _eatCount = 0;
            _lifeTimer = 0f;
        }

        protected override void Update()
        {
            if (!IsAlive) return;

            // Tick lifetime — predator dies after 10 seconds regardless of energy
            _lifeTimer += Time.deltaTime;
            if (_lifeTimer >= Lifetime)
            {
                Debug.Log($"[PredatorBug] {name} lifetime expired.");
                Die();
                return;
            }

            // Run state machine and behavior but skip the energy-starvation death in BugBase.
            // Predators are driven purely by their lifetime timer.
            StateMachine.Update(this);
            CurrentBehavior?.Execute(this);
        }

        // Called by Eat state (resource pickup)
        public override void Eat(IResource resource)
        {
            if (resource == null || resource.IsConsumed) return;
            base.Eat(resource);
            RegisterEat();
        }

        // Called by AttackState when a kill lands
        public void OnKill()
        {
            RegisterEat();
        }

        private void RegisterEat()
        {
            _eatCount++;
            Debug.Log($"[PredatorBug] {name} eat count: {_eatCount}/{EatsRequiredToSplit}");

            if (_eatCount >= EatsRequiredToSplit)
            {
                _eatCount = 0;
                TrySplit();
            }
        }

        private void TrySplit()
        {
            if (_colonyManager == null)
            {
                Debug.LogWarning("[PredatorBug] ColonyManager not injected — split skipped.");
                return;
            }
            _colonyManager.SplitPredator(this);
        }
    }
}

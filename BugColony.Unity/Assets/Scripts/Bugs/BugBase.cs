using System;
using UnityEngine;
using BugColony.Core;
using BugColony.Bugs.States;

namespace BugColony.Bugs
{
    public abstract class BugBase : MonoBehaviour, IBug
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float energyDecayRate = 1f;

        public float Health { get; protected set; }
        public float Energy { get; protected set; }
        public float Speed => speed;
        public bool IsAlive => Health > 0f;

        public float MaxHealth => maxHealth;
        public float MaxEnergy => maxEnergy;

        private BugStateMachine _stateMachine;
        private IBugBehavior _behavior;

        public BugStateMachine StateMachine => _stateMachine;
        public IBugBehavior CurrentBehavior => _behavior;

        public event Action<BugBase> OnDeath;

        protected virtual void Awake()
        {
            _stateMachine = new BugStateMachine();
        }

        protected virtual void Update()
        {
            if (!IsAlive) return;

            Energy -= energyDecayRate * Time.deltaTime;
            if (Energy <= 0f)
            {
                Energy = 0f;
                Die();
                return;
            }

            _stateMachine.Update(this);
            _behavior?.Execute(this);
        }

        public void Move(Vector3 direction)
        {
            if (!IsAlive) return;
            transform.position += direction.normalized * Speed * Time.deltaTime;
        }

        public virtual void Eat(IResource resource)
        {
            if (resource == null || resource.IsConsumed) return;
            Energy = Mathf.Min(Energy + resource.NutrientValue, maxEnergy);
            resource.Consume();
        }

        public virtual bool CanSplit()
        {
            return IsAlive && Energy >= maxEnergy * 0.8f;
        }

        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;
            Health = Mathf.Max(Health - damage, 0f);
            if (Health <= 0f)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            if (_stateMachine.CurrentState is DeadState) return;
            Health = 0f;
            SetState(new DeadState());
            OnDeath?.Invoke(this);
        }

        public void SetBehavior(IBugBehavior behavior)
        {
            _behavior = behavior;
            _behavior?.Initialize(this);
        }

        public void SetState(IBugState state)
        {
            _stateMachine.ChangeState(this, state);
        }

        public virtual void OnSpawn()
        {
            Health = maxHealth;
            Energy = maxEnergy;
            gameObject.SetActive(true);
            SetState(new IdleState());
        }

        public virtual void OnDespawn()
        {
            OnDeath = null;
            gameObject.SetActive(false);
        }
    }
}

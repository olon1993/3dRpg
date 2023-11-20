using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable 
    {
        [SerializeField] private bool _showDebugLog = false;

        [Range(0, 1)]
        [SerializeField] float regenerationOnLevelUp = 0.7f;
        [SerializeField] UnityEvent<float> takeDamage;
        [SerializeField] UnityEvent onDie;

        LazyValue<float> health;
        private float maxHealth = 100f;

        private bool _isDead = false;

        private Animator _animator;
        private ActionScheduler _actionScheduler;
        private BaseStats _baseStats;

        // Unity Methods
        void Awake()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _baseStats = GetComponent<BaseStats>();

            health = new LazyValue<float>(GetInitialHealth);
        }

        void Start()
        {
            health.ForceInit();

            if (_showDebugLog)
            {
                Debug.Log(gameObject.name + " health: " + health.value);
            }
        }

        private void OnEnable()
        {
            if (_baseStats != null)
            {
                _baseStats.OnLevelUp += UpdateHealthOnLevelUp;
            }
        }

        private void OnDisable()
        {
            if (_baseStats != null)
            {
                _baseStats.OnLevelUp -= UpdateHealthOnLevelUp;
            }
        }

        // Public Methods
        public void UpdateHealthOnLevelUp()
        {
            float regenHealthPoints = _baseStats.GetStat(Stat.Health) * regenerationOnLevelUp;
            maxHealth = _baseStats.GetStat(Stat.Health);
            health.value = Mathf.Max(health.value, regenHealthPoints);
        }

        public void TakeDamage(GameObject instagator, float damage)
        {
            if (_showDebugLog)
            {
                Debug.Log(gameObject.name + " took " + damage + " damage");
            }

            health.value = Mathf.Max(health.value - damage, 0);
            if (health.value == 0)
            {
                onDie.Invoke();
                AwardExperience(instagator);
                Die();
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        public void Heal(float healthToRestore)
        {
            health.value = Mathf.Min(GetMaxHealth(), health.value + healthToRestore);
        }

        public void Die()
        {
            if (_isDead)
            {
                return;
            }

            _isDead = true;
            _animator.SetTrigger("die");
            _actionScheduler.CancelCurrentAction();
        }

        public bool IsDead
        {
            get { return _isDead; }
        }

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return (health.value / maxHealth);
        }

        public float GetHealth()
        {
            return health.value;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        // Private Methods
        private void AwardExperience(GameObject instagator)
        {
            Experience experience = instagator.GetComponent<Experience>();
            if (experience == null)
            {
                return;
            }

            experience.GainExperience(_baseStats.GetStat(Stat.ExperienceReward));
        }

        private float GetInitialHealth()
        {
            maxHealth = _baseStats.GetStat(Stat.Health);
            return maxHealth;
        }

        // IJsonSavable
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(health.value);
        }

        public void RestoreFromJToken(JToken state)
        {
            health.value = state.ToObject<float>();
            if (health.value == 0)
            {
                Die();
            }
        }
    }

    //[System.Serializable]
    //public class TakeDamageEvent : UnityEvent<float>
    //{

    //}
}

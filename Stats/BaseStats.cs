using GameDevTV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{

    public class BaseStats : MonoBehaviour
    {
        [SerializeField] private bool _showDebugLog = false;

        [Range(1, 99)]
        [SerializeField] private int _startingLevel = 1;
        [SerializeField] private CharacterClass _characterClass;
        [SerializeField] private Progression _progression;
        [SerializeField] private GameObject _levelUpParticleEffect = null;
        [SerializeField] private bool _shouldUserModifiers = false;

        public event Action OnLevelUp;

        private Experience experience;
        private LazyValue<int> currentLevel;

        // Unity Methods
        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.OnExperiencedGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.OnExperiencedGained -= UpdateLevel;
            }
        }

        // Public Methods
        public int GetLevel()
        {
            return currentLevel.value;
        }

        public float GetStat(Stat stat)
        {
            if (_showDebugLog)
            {
                Debug.Log(gameObject.name + " " + stat + " at level " + _startingLevel + ": " + _progression.GetStat(stat, _characterClass, GetLevel()));
            }

            if (stat == Stat.Damage)
            {
                Debug.Log("Damage= (" + GetBaseStat(stat) + " + " + GetStatModifier(stat) + ") * " + GetStatMultiplier(stat) + " = "
                    + (GetBaseStat(stat) + GetStatModifier(stat)) * GetStatMultiplier(stat));
            }

            return (GetBaseStat(stat) + GetStatModifier(stat)) * GetStatMultiplier(stat);
        }

        // Private Methods
        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();

            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                OnLevelUp();
                LevelUpEffect();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(_levelUpParticleEffect, transform);
        }

        private float GetBaseStat(Stat stat)
        {
            if (_showDebugLog)
            {
                Debug.Log(gameObject.name + " " + stat + " base: " + _progression.GetStat(stat, _characterClass, GetLevel()));
            }

            return _progression.GetStat(stat, _characterClass, GetLevel());
        }

        private float GetStatModifier(Stat stat)
        {
            if(_shouldUserModifiers == false)
            {
                return 0;
            }

            float statModifierSum = 0;
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetStatModifiers(stat))
                {
                    statModifierSum += modifier;
                }
            }

            if (_showDebugLog)
            {
                Debug.Log(gameObject.name + " " + stat + " Modifier: " + statModifierSum);
            }

            return statModifierSum;
        }

        private float GetStatMultiplier(Stat stat)
        {
            if(_shouldUserModifiers == false)
            {
                return 1;
            }

            float statMultiplierSum = 100;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float multiplier in provider.GetStatMultipliers(stat))
                {
                    statMultiplierSum += multiplier;
                }
            }
            statMultiplierSum /= 100;

            if (_showDebugLog)
            {
                Debug.Log(gameObject.name + " " + stat + " Multiplier: " + statMultiplierSum);
            }

            return statMultiplierSum;
        }

        private int CalculateLevel()
        {
            experience = GetComponent<Experience>();
            if(experience == null)
            {
                return _startingLevel;
            }

            float currentXP = experience.GetExperience();
            int penultimateLevel = _progression.GetLevels(Stat.ExperienceToLevelUp, _characterClass);

            for (int level = 1; level <= penultimateLevel; level++)
            {
                float xpToLevelUp = _progression.GetStat(Stat.ExperienceToLevelUp, _characterClass, level);
                if (xpToLevelUp > currentXP)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }
    }
}

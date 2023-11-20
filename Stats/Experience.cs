using Newtonsoft.Json.Linq;
using RPG.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private bool _showDebugLog = false;

        [SerializeField] float Xp;

        public event Action OnExperiencedGained;

        public float GetExperience()
        {
            return Xp;
        }

        public void GainExperience(float xp)
        {
            Xp += xp;
            OnExperiencedGained();

            if (_showDebugLog)
            {
                Debug.Log("XP: " + Xp);
            }
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(Xp);
        }

        public void RestoreFromJToken(JToken state)
        {
            Xp = state.ToObject<float>();
        }
    }
}

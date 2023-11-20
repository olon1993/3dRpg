using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{

    public class SavingWrapper : MonoBehaviour
    {
        const string DEFAULT_SAVE_FILE = "save";

        [SerializeField] private float _fadeInTime = 1f;

        IEnumerator Start()
        {
            yield return GetComponent<JsonSavingSystem>().LoadLastScene(DEFAULT_SAVE_FILE);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(_fadeInTime);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Load()
        {
            GetComponent<JsonSavingSystem>().Load(DEFAULT_SAVE_FILE);
        }

        public void Save()
        {
            GetComponent<JsonSavingSystem>().Save(DEFAULT_SAVE_FILE);
        }

        public void Delete()
        {
            GetComponent<JsonSavingSystem>().Delete(DEFAULT_SAVE_FILE);
        }
    }
}

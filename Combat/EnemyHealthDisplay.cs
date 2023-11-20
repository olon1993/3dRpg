using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{

    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI textMesh;
        Fighter fighter;
        Health health;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            health = fighter.GetTarget();
            if(health == null)
            {
                textMesh.text = "N/A";
            }
            else
            {
                textMesh.text = health.GetHealth().ToString() + " / " + health.GetMaxHealth().ToString();
            }
        }
    }
}

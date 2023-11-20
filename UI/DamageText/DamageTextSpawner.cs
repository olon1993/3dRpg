using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{

    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText DamageTextPrefab;

        public void Spawn(float damageAmount)
        {
            DamageText instance = Instantiate<DamageText>(DamageTextPrefab, transform);
            instance.SetDamageText(damageAmount);
        }
    }
}

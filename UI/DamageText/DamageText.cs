using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text = null;

        public void DestroyText()
        {
            Destroy(gameObject);
        }

        public void SetDamageText(float damageAmount)
        {
            text.SetText(damageAmount.ToString());
        }
    }
}

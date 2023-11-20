using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{

    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health health = null;
        [SerializeField] RectTransform healthBar = null;
        [SerializeField] Canvas rootCanvas = null;

        // Update is called once per frame
        void Update()
        {
            if (Mathf.Approximately(health.GetFraction(), 0) ||
                Mathf.Approximately(health.GetFraction(), 1))
            {
                rootCanvas.enabled = false;
            }
            else
            {
                rootCanvas.enabled = true;
                healthBar.localScale = new Vector3(health.GetFraction(), 1, 1);
            }
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{

    public class Weapon : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onHit;

        public void OnHit()
        {
            _onHit.Invoke();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{

    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController playerController)
        {
            Fighter fighter = playerController.GetComponent<Fighter>();

            if (fighter.CanAttack(gameObject) == false)
            {
                return false;
            }

            if (Input.GetMouseButton(0))
            {
                fighter.Attack(gameObject);
            }

            return true;
        }
    }

}

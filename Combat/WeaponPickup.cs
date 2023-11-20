using RPG.Attributes;
using RPG.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{

    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private bool _showDebugLog = false;

        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToRestore = 0;
        [SerializeField] float respawnTime = 5f;
        private Collider collider;

        private void Awake()
        {
            collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_showDebugLog)
            {
                Debug.Log("Enter");
            }

            if (other.gameObject.CompareTag("Player"))
            {
                Fighter fighter = other.gameObject.GetComponent<Fighter>();
                if (fighter == null)
                {
                    Debug.LogError(other.gameObject.name + "has collided with weapon pickup + " + gameObject.name + " but it has no fighter component.");
                    return;
                }

                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject)
        {
            if (healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }

            if (weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }

            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            PickupSetActive(false);
            yield return new WaitForSeconds(seconds);
            PickupSetActive(true);
        }

        private void PickupSetActive(bool isActive)
        {
            collider.enabled = isActive;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(isActive);
            }
        }

        public bool HandleRaycast(PlayerController controller)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(controller.gameObject);
            }

            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}

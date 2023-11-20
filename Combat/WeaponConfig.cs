using RPG.Attributes;
using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weaopn", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] Projectile projectile = null;

        [SerializeField] private float _damage = 5f;
        [SerializeField] private float _damageMultiplierPercentage = 25;
        [SerializeField] private float _range = 2f;
        [SerializeField] private bool _isRightHanded = true;

        const string WEAPON_NAME = "Weapon";

        public Weapon Spawn(Transform leftHand, Transform rightHand, Animator animator)
        {
            DestroyOldWeapon(leftHand, rightHand);

            if (equippedPrefab == null)
            {
                return null;
            }

            if(animatorOverride == null)
            {
                // if this weapon does not have an override controller (meaning we want to use the default attack animation)
                // and the animator currently has an override controller assigned, then we remove the override controller
                // by reassigning the base animator controller as the runtime animator controller
                if (animator.runtimeAnimatorController is AnimatorOverrideController)
                {
                    animator.runtimeAnimatorController = (animator.runtimeAnimatorController as AnimatorOverrideController).runtimeAnimatorController;
                }
            }
            else
            {
                animator.runtimeAnimatorController = animatorOverride;
            }

            Weapon weapon = Instantiate(equippedPrefab, GetHandTransform(leftHand, rightHand));
            weapon.gameObject.name = WEAPON_NAME;
            return weapon;
        }

        private void DestroyOldWeapon(Transform leftHand, Transform rightHand)
        {
            Transform previousWeapon = rightHand.Find(WEAPON_NAME);
            if (previousWeapon == null)
            {
                previousWeapon = leftHand.Find(WEAPON_NAME);
            }

            if(previousWeapon == null)
            {
                return;
            }

            previousWeapon.name = "DESTROYING";
            Destroy(previousWeapon.gameObject);
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }


        public void LaunchProjectile(Transform leftHand, Transform rightHand, Health target, GameObject instagator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(leftHand, rightHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instagator, calculatedDamage);
        }

        private Transform GetHandTransform(Transform leftHand, Transform rightHand)
        {
            if (_isRightHanded)
            {
                return rightHand;
            }
            else
            {
                return leftHand;
            }
        }

        public float Range
        {
            get { return _range; }
        }

        public float Damage
        {
            get { return _damage; }
        }

        public float DamageMultiplierPercentage
        {
            get { return _damageMultiplierPercentage; }
        }
    }
}

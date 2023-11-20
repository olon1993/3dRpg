using RPG.Attributes;
using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{

    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifetime = 10f;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] GameObject[] destroyOnHit = null;

        [SerializeField] UnityEvent onHit;

        private Health _target = null;
        private GameObject _instagator = null;
        private float _damage = 0;

        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, maxLifetime);
        }

        // Update is called once per frame
        void Update()
        {
            if (_target == null)
            {
                return;
            }

            if (isHoming && _target.IsDead == false)
            {
                transform.LookAt(GetAimLocation());
            }

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instagator, float damage)
        {
            _target = target;
            _instagator = instagator;
            _damage = damage;
            transform.LookAt(GetAimLocation());
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCollider = _target.GetComponent<CapsuleCollider>();
            if (targetCollider == null)
            {
                return _target.transform.position;
            }
            return _target.transform.position + Vector3.up * targetCollider.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == _target.gameObject)
            {
                if (_target.IsDead)
                {
                    return;
                }

                onHit.Invoke();

                if (hitEffect != null)
                {
                    Instantiate(hitEffect, GetAimLocation(), transform.rotation);
                }

                Health targetHealth = other.GetComponent<Health>();
                targetHealth.TakeDamage(_instagator, _damage);
                speed = 0;

                foreach (GameObject gameObject in destroyOnHit)
                {
                    Destroy(gameObject);
                }

                Destroy(gameObject, lifeAfterImpact);
            }
        }
    }
}

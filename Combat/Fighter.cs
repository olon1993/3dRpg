using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, IModifierProvider, IJsonSaveable
    {
        private bool _showDebugLog = false;

        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] private WeaponConfig _defaultWeapon = null;
        [SerializeField] private WeaponConfig _currentWeaponConfig;
        private LazyValue<Weapon> _currentWeapon;

        private Health _target;
        private ActionScheduler _actionScheduler;
        private Mover _mover;
        private BaseStats _baseStats;
        private Animator _animator;

        private float _timeSinceLastAttack = Mathf.Infinity;

        // Start is called before the first frame update
        void Awake()
        {
            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _baseStats = GetComponent<BaseStats>();

            _currentWeaponConfig = _defaultWeapon;
            _currentWeapon = new LazyValue<Weapon>(GetInitialWeapon);
        }

        void Start()
        {
            _currentWeapon.ForceInit();
        }

        // Update is called once per frame
        void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;

            if (_target == null)
            {
                return;
            }

            if (_target.IsDead)
            {
                return;
            }

            if (!GetIsInRange(_target.transform))
            {
                _mover.MoveTo(_target.transform.position, 1f);
            }
            else
            {
                _mover.Cancel();
                AttckBehavior();
            }
        }

        // Triggered by animation event
        public void Hit()
        {
            if (_target == null)
            {
                return;
            }

            if (_currentWeapon.value != null)
            {
                _currentWeapon.value.OnHit();
            }

            _target.TakeDamage(gameObject, _baseStats.GetStat(Stat.Damage));
        }

        // Triggered by animation event
        public void Shoot()
        {
            if (_target != null)
            {
                if (_currentWeaponConfig.HasProjectile())
                {
                    _currentWeaponConfig.LaunchProjectile(leftHandTransform, rightHandTransform, _target, gameObject, _baseStats.GetStat(Stat.Damage));
                }
            }
        }

        public void Attack(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);
            _target = combatTarget.gameObject.GetComponent<Health>();

            if (_showDebugLog)
            {
                Debug.Log(name + " is attacking");
            }
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null)
            {
                return false;
            }

            if (_mover.CanMoveTo(combatTarget.transform.position) == false && 
                GetIsInRange(combatTarget.transform) == false)
            {
                return false;
            }

            Health potentialTarget = combatTarget.GetComponent<Health>();
            if (potentialTarget == null)
            {
                return false;
            }

            if (potentialTarget.IsDead)
            {
                return false;
            }

            return true;
        }

        public void Cancel()
        {
            CancelAttack();
            _mover.Cancel();
            _target = null;
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            _currentWeaponConfig = weapon;
            _currentWeapon.value = AttachWeapon(weapon);
        }

        public Health GetTarget()
        {
            return _target;
        }

        private void AttckBehavior()
        {
            transform.LookAt(_target.transform);

            if (_timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();
                _timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            _animator.ResetTrigger("cancelAttack");
            _animator.SetTrigger("attack");
        }

        private void CancelAttack()
        {
            _animator.ResetTrigger("attack");
            _animator.SetTrigger("cancelAttack");
        }

        private Weapon GetInitialWeapon()
        {
            return AttachWeapon(_defaultWeapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(leftHandTransform, rightHandTransform, animator);
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(targetTransform.position, transform.position) < _currentWeaponConfig.Range;
        }

        // IModifierProvider
        public IEnumerable<float> GetStatModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.Damage;
            }
        }

        public IEnumerable<float> GetStatMultipliers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.DamageMultiplierPercentage;
            }
        }

        // IJsonSaveable
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_currentWeaponConfig.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            string weaponName = state.ToString();
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }

}

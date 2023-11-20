using GameDevTV.Utils;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;
using UnityEngine;

namespace RPG.Control
{
    public class AiController : MonoBehaviour
    {
        [SerializeField] private bool _showDebugLog = false;

        [Range(0, 1)]
        [SerializeField] float patrolSpeedMultiplier = 0.25f;
        [SerializeField] float chaseDistance = 5f;
        private GameObject _player;
        private ActionScheduler _actionScheduler;

        private LazyValue<Vector3> _guardPosition;
        [SerializeField] float suspicionTime = 5f;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        [SerializeField] float agroTime = 10f;
        private float _timeSinceAggrevated = Mathf.Infinity;
        [SerializeField] float mobAgroDistance = 5f;

        [SerializeField] PatrolPath patrolPath;
        [SerializeField] int initialWaypointIndex = 0;
        [SerializeField] float waypointDistanceErrorMargin = 1f;
        [SerializeField] float waypointDwellingTime = 5f;
        private float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private int _currentWaypointIndex = 0;

        private Mover _mover;
        private Fighter _fighter;
        private Health _health;

        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();

            _player = GameObject.FindGameObjectWithTag("Player");
            _guardPosition = new LazyValue<Vector3>(GetInitialGuardPosition);
        }

        void Start()
        {
            _currentWaypointIndex = initialWaypointIndex;
            _guardPosition.ForceInit();
        }

        // Update is called once per frame
        void Update()
        {
            if (_health.IsDead)
            {
                return;
            }

            if (IsAggrevated() && _fighter.CanAttack(_player))
            {
                AttackBehavior();
            }
            else if (_timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }

            UpdateTimers();
        }

        public void Aggrevate()
        {
            _timeSinceAggrevated = 0;
        }

        private Vector3 GetInitialGuardPosition()
        {
            return transform.position;
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWaypoint += Time.deltaTime;
            _timeSinceAggrevated += Time.deltaTime;
        }

        private void AttackBehavior()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, mobAgroDistance, Vector3.up, 0);
            foreach(RaycastHit hit in hits)
            {
                hit.collider.GetComponent<AiController>()?.Aggrevate();
            }
        }

        private void SuspicionBehavior()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void PatrolBehavior()
        {
            if (_showDebugLog)
            {
                Debug.Log(name + " is executing PatrolBehavior");
            }

            Vector3 nextPosition = _guardPosition.value;

            if(patrolPath != null)
            {
                if (IsAtWaypoint())
                {
                    _timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }

                nextPosition = GetNextWaypoint();
            }
            else if (_showDebugLog)
            {
                Debug.Log("PatrolPath is null on " + name);
            }
        
            if (_timeSinceArrivedAtWaypoint > waypointDwellingTime)
            {
                _mover.StartMoveAction(nextPosition, patrolSpeedMultiplier);
            }
        }

        private bool IsAggrevated()
        {
            if(_timeSinceAggrevated < agroTime)
            {
                return true;
            }

            return Vector3.Distance(transform.position, _player.transform.position) < chaseDistance;
        }

        private bool IsAtWaypoint()
        {
            Debug.Log(Vector3.Distance(transform.position, GetNextWaypoint()));
            if(Vector3.Distance(transform.position, GetNextWaypoint()) < waypointDistanceErrorMargin)
            {
                return true;
            }
            return false;
        }

        private Vector3 GetNextWaypoint()
        {
            return patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}

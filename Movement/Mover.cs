using RPG.Combat;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, IJsonSaveable //ISaveable, 
    {
        [SerializeField] private bool _showDebugLog = false;

        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] private float _maxNavPathLength = 40f;
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private Health _health;

        // Start is called before the first frame update
        void Awake()
        {
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            _navMeshAgent.enabled = _health.IsDead == false;

            UpdateAnimator();
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path) == false)
            {
                return false;
            }

            if (path.status != NavMeshPathStatus.PathComplete)
            {
                return false;
            }

            if (GetPathLength(path) > _maxNavPathLength)
            {
                return false;
            }

            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float pathLength = 0;

            if (path.corners.Length < 2)
            {
                return pathLength;
            }

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            if (_showDebugLog)
            {
                Debug.Log("Path Length: " + pathLength);
            }

            return pathLength;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = _navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;

            _animator.SetFloat("forwardSpeed", speed);
        }

        public void StartMoveAction(Vector3 destination, float speedMultiplier)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedMultiplier);
        }

        public void MoveTo(Vector3 destination, float speedMultiplier)
        {
            _navMeshAgent.SetDestination(destination);
            _navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedMultiplier);
            _navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> data = state;
            data["position"] = transform.position.ToToken();
            data["rotation"] = transform.eulerAngles.ToToken();
            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            IDictionary<string, JToken> data = (JObject)state;
            _navMeshAgent.Warp(data["position"].ToObject<Vector3>());
            transform.eulerAngles = data["rotation"].ToObject<Vector3>();
        }
    }

}

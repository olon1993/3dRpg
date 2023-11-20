using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private bool _showDebugLog = false;

        [SerializeField] float sphereCastRadius = 1f;

        [System.Serializable]
        public struct CursorMapping
        {
            public CursorType CursorType;
            public Texture2D Texture;
            public Vector2 Hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;

        [SerializeField] private float _maxnavMeshProjectionDistance = 1f;

        Ray ray;
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;

        // Start is called before the first frame update
        void Awake()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if (InteractWithUI())
            {
                SetCursor(CursorType.UI);
                return;
            }

            if (_health.IsDead)
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent())
            {
                return;
            }

            if (InteractWithMovement())
            {
                return;
            }

            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();

            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }

            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), sphereCastRadius);
            float[] distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        private bool InteractWithMovement()
        {
            Vector3 target;
            bool hasHit = RaycastNavmesh(out target);

            if (hasHit) 
            {
                if (_showDebugLog)
                {
                    Debug.Log("InteractWithMovement hasHit on " + name);
                }

                if (_mover.CanMoveTo(target) == false)
                {
                    if (_showDebugLog)
                    {
                        Debug.Log("CanMoveTo returned false on " + name);
                    }

                    return false;
                }

                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoveAction(target, 1f);
                }

                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        private bool RaycastNavmesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (hasHit == false)
            {
                if (_showDebugLog)
                {
                    Debug.Log("RaycastNavmesh has hit returned false on " + name);
                }

                return false;
            }

            // Find nearest navmesh point
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(hit.point, out navMeshHit, _maxnavMeshProjectionDistance, NavMesh.AllAreas) == false)
            {
                if (_showDebugLog)
                {
                    Debug.Log("NavMesh.SamplePosition returned false on " + name);
                }

                return false;
            }


            target = navMeshHit.position;

            return true;
        }

        private void SetCursor(CursorType cursorType)
        {
            CursorMapping cursorMapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(cursorMapping.Texture, cursorMapping.Hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType cursorType)
        {
            foreach(CursorMapping cursorMapping in cursorMappings)
            {
                if(cursorMapping.CursorType == cursorType)
                {
                    return cursorMapping;
                }
            }

            return cursorMappings[0];
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

    }
}

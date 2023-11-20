using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        private float waypointGizmoRadius = 0.5f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(GetNextIndex(i)));
            }
        }

        public int GetNextIndex(int currentIndex)
        {
            return (currentIndex + 1) % transform.childCount;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class GeneratorMovement : MonoBehaviour
{
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0;
    private float moveSpeed = 2f;
    private bool isMoving = true;

    public void SetWaypoints(List<Transform> points)
    {
        waypoints = points;
        if (waypoints.Count > 0)
            transform.position = waypoints[0].position;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    void Update()
    {
        if (!isMoving || waypoints.Count == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Count)
                currentWaypointIndex = 0;
        }
    }
}
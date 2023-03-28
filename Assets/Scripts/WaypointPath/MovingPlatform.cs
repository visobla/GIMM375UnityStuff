using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private WaypointPath _waypointPath;

    [SerializeField]
    private float _speed;

    private int _targetWaypointIndex;

    private Transform _previousWaypoint;
    private Transform _targetWaypoint;

    private float _timeToWaypoint;
    private float _elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        // Set the fixed timestep for this script to 0.01 seconds
        Time.fixedDeltaTime = 0.01f;
        TargetNextWaypoint();
    }

    //Changed the FixedUpdate Settings in Edit>Project Settings> Time from 0.02 to 0.01
    void FixedUpdate()
    {
        _elapsedTime += Time.deltaTime;

        //Follows the percentage and when it is equal to or greater than 1 moves to next waypoint
        float elapsedPercentage = _elapsedTime / _timeToWaypoint;
        //Adds smoothing so it is slower at beginning and end.
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        //Moves the platform from waypoint to waypoint
        transform.position = Vector3.Lerp(_previousWaypoint.position, _targetWaypoint.position, elapsedPercentage);
        //Rotates the plaform to follow waypoints
        transform.rotation = Quaternion.Lerp(_previousWaypoint.rotation, _targetWaypoint.rotation, elapsedPercentage);

        if (elapsedPercentage >= 1)
        {
            TargetNextWaypoint();
        }
    }

    private void TargetNextWaypoint()
    {
        _previousWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);
        _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
        _targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);

        _elapsedTime = 0;

        float distanceToWaypoint = Vector3.Distance(_previousWaypoint.position, _targetWaypoint.position);
        _timeToWaypoint = distanceToWaypoint / _speed;
    }

    //Attaches Player to platform 
    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    //Removes player from platform when they exit
    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }
}

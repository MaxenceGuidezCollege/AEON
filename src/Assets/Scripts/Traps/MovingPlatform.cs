using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public WaypointPath waypointPath;
    public float speed;
    
    private Transform _previousWaypoint, _targetWaypoint;
    private int _targetWaypointIndex;
    private float _timeToWaypoint, _elapsedTime, _elapsedPercentage, _currentSpeed;

    void Start()
    {
        _currentSpeed = speed;
        TargetNextWaypoint();
    }

    void Update()
    {
        bool isTimeStopped = TimeController.instance.GetStopState();
        
        if(_elapsedPercentage >= 1) TargetNextWaypoint();

        if (isTimeStopped)
        {
            _currentSpeed = 0.000000001f;
        }
        else
        {
            _currentSpeed = speed;
            _elapsedTime += Time.deltaTime;
            _elapsedPercentage = _elapsedTime / _timeToWaypoint;
        }

        Vector3 prevPos = _previousWaypoint.position;
        Vector3 targetPos = _targetWaypoint.position;
        transform.position = Vector3.Lerp(prevPos, targetPos, _elapsedPercentage);
        
        float distanceToWaypoint = Vector3.Distance(prevPos, targetPos);
        _timeToWaypoint = distanceToWaypoint / _currentSpeed;
    }

    private void TargetNextWaypoint() {
        _previousWaypoint = waypointPath.GetWaypoint(_targetWaypointIndex);
        _targetWaypointIndex = waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
        _targetWaypoint = waypointPath.GetWaypoint(_targetWaypointIndex);

        _elapsedTime = 0;
    }
}

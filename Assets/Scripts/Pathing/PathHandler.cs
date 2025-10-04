using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathHandler : MonoBehaviour
{
    public Waypoint CurrentWaypoint
    {
        get => _currentWaypoint; 
        private set
        {
            // Remove listener from previous waypoint event, if any
            if(_currentWaypoint != null && _currentWaypoint.Event != null)
            _currentWaypoint.Event.OnEventEnd.RemoveListener(HandleWaypointEventEnd);

            // Add listener to new waypoint event, if any
            if(value != null)
            {
                if(value.Event != null)
                    value.Event.OnEventEnd.AddListener(HandleWaypointEventEnd);

                // Log error if no event assigned
                else
                    Debug.LogError($"{value.name} has no event assigned.");
            }

            _currentWaypoint = value;
        }
    }
    public bool IsBusy { get; private set; }
    public bool IsFinished { get; private set; }

    private PlayerMovement _playerMovement;
    private Waypoint[] _waypoints;
    private Waypoint _currentWaypoint;

    private void Awake() => Initialize();
    private void Initialize()
    {
        // Get references to the required components
        _playerMovement = GetComponent<PlayerMovement>();

        // Try get all Waypoints placed in the scene
        _waypoints = FindObjectsByType<Waypoint>(FindObjectsSortMode.None);
        if(_waypoints.Length == 0)
            Debug.LogError($"No active Waypoints found in scene.");

        // Sort them by their Order property
        if(IsOrderCorrect(_waypoints))
            _waypoints = _waypoints.OrderBy(t => t.Order).ToArray();

        // If cannot sort, log error
        else
        {
            string message = "";

            foreach(Waypoint waypoint in _waypoints)
                message += $"Order: {waypoint.Order} - Waypoint: {waypoint.name} - Type: {waypoint.Event.GetType()}\n";

            Debug.LogError($"PathPoints have repeating order!\n{message}");
        }

        // Set first waypoint as current
        if (_waypoints.Length > 0)
            CurrentWaypoint = _waypoints[0];
    }
    private bool IsOrderCorrect(Waypoint[] pathPoints)
    {
        List<int> orders = new();
        foreach(Waypoint point in pathPoints)
        {
            if(orders.Contains(point.Order))
                return false;
            orders.Add(point.Order);
        }

        return true;
    }

    private void Start() => StartListening();
    private void StartListening()
    {
        // Listen to _playerMovement events
        if(_playerMovement != null)
        {
            _playerMovement.OnStartedMoving += ContinuePath;
            _playerMovement.OnArrived += HandleArrival;
        }
    }
    private void OnDisable() => StopListening();
    private void StopListening()
    {
        // Prevent memory leaks
        if(_playerMovement != null)
        {
            _playerMovement.OnStartedMoving -= ContinuePath;
            _playerMovement.OnArrived -= HandleArrival;
        }

        if(_currentWaypoint != null && _currentWaypoint.Event != null)
            _currentWaypoint.Event.OnEventEnd.RemoveListener(HandleWaypointEventEnd);
    }

    private void ContinuePath()
    {
        if(!IsFinished)
        {
            if (CurrentWaypoint != null)
            {
                Debug.Log($"Started moving to {CurrentWaypoint.name}...");
                // UNDONE: Implement starting movement logic
                IsBusy = true;
            }
        }
        else
            Debug.Log("Path has already ended.");
    }
    private void HandleArrival()
    {
        if(CurrentWaypoint.Event != null)
            CurrentWaypoint.Event.StartEvent();
        else
            Debug.LogError($"{CurrentWaypoint.name} has no event assigned.");
    }

    private void HandleWaypointEventEnd()
    {
        // If arrived at last waypoint, end path
        if(CurrentWaypoint == _waypoints.Last())
            EndPath();
        else
            AssignNextWaypoint();

        IsBusy = false;
    }
    private void EndPath()
    {
        IsFinished = true;
        CurrentWaypoint = null;
        Debug.Log("Arrived at the last waypoint of the path.");
        Debug.Log("Thanks for testing!");
    }
    private void AssignNextWaypoint()
    {
        // Assign next waypoint
        int targetIndex = System.Array.IndexOf(_waypoints , CurrentWaypoint) + 1;
        if(targetIndex < _waypoints.Length)
            CurrentWaypoint = _waypoints[targetIndex];
        Debug.Log("Move anywhere to keep moving...");
    }

    public void Interact()
    {
        if(CurrentWaypoint == null)
            Debug.Log("No current waypoint to interact with.");
        else if (CurrentWaypoint.Event != null)
        {
            Debug.Log($"Ending current {CurrentWaypoint.Event.GetType()}...");
            CurrentWaypoint.Event.EndEvent();
        }
    }
}
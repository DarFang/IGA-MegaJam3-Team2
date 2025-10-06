using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Responsible for managing the sequence of Waypoints and their associated events.
/// </summary>
public class PathHandler : MonoBehaviour
{
    #region Properties

    /// <summary>
    /// The current waypoint targeted by the PathHandler
    /// </summary>>
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
                    Debug.LogError($"{value.name} has no event assigned.", this);
            }

            _currentWaypoint = value;
        }
    }
    
    /// <summary>
    /// Is the PathHandler currently busy with an active task?
    /// </summary>
    /// <returns>true if the PathHandler has an active task</returns>
    public bool IsBusy { get; private set; }
    
    /// <summary>
    /// Has the PathHandler reached the end of the path?
    /// </summary>
    /// <returns>true if the last waypoint in the path has been completed</returns>
    public bool HasFinished { get; private set; }

    #endregion

    #region Fields

    private PlayerMovement _playerMovement;
    private Waypoint[] _waypoints;
    private Waypoint _currentWaypoint;

    #endregion

    #region Private Methods

    private void Awake() => Initialize();

    private void Initialize()
    {
        // Get references to the required components
        _playerMovement = GetComponent<PlayerMovement>();

        // Try get all Waypoints placed in the scene
        _waypoints = FindObjectsByType<Waypoint>(FindObjectsSortMode.None);
        if(_waypoints.Length == 0)
            Debug.LogError($"No active Waypoints found in scene.", this);

        // Sort them by their Order property
        if(IsOrderCorrect(_waypoints))
            _waypoints = _waypoints.OrderBy(t => t.Order).ToArray();

        // If cannot sort, log error
        else
        {
            string message = "";

            foreach(Waypoint waypoint in _waypoints)
                message += $"Order: {waypoint.Order} - Waypoint: {waypoint.name} - Type: {waypoint.Event.GetType()}\n";

            Debug.LogError($"PathPoints have repeating order!\n{message}", this);
        }

        // Set first waypoint as current
        if (_waypoints.Length > 0)
            CurrentWaypoint = _waypoints[0];
    }

    /// <summary>
    /// Checks if the Waypoints array has repeating Order values.
    /// </summary>
    /// <param name="pathPoints">The Waypoints array to check.</param>
    /// <returns>true if order values don't repeat.</returns>
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

    /// <summary>
    /// Start listening to _playerMovement events.
    /// </summary>
    private void StartListening()
    {
        if (_playerMovement != null)
        {
            _playerMovement.OnStartedMoving += StartMoveEvent;
            _playerMovement.OnArrived += HandleArrival;
        }
        if (CurrentWaypoint != null && CurrentWaypoint.AutoStart)
        {
            _playerMovement.GoTo(CurrentWaypoint);
        }
    }

    private void OnDisable() => StopListening();

    /// <summary>
    /// Stop listening to _playerMovement and CurrentWaypoint events to prevent memory leaks.
    /// </summary>
    private void StopListening()
    {
        if(_playerMovement != null)
        {
            _playerMovement.OnStartedMoving -= StartMoveEvent;
            _playerMovement.OnArrived -= HandleArrival;
        }

        if(_currentWaypoint != null && _currentWaypoint.Event != null)
            _currentWaypoint.Event.OnEventEnd.RemoveListener(HandleWaypointEventEnd);
    }

    private void StartMoveEvent()
    {
        if(!HasFinished)
        {
            if (CurrentWaypoint != null)
            {
                Debug.Log($"Started moving to {CurrentWaypoint.name}...");
                // UNDONE: Implement starting movement event logic
                IsBusy = true;
            }
        }
        else
            Debug.Log("Path has already ended.");
    }

    private void HandleArrival()
    {
        if (CurrentWaypoint.Event != null)
            CurrentWaypoint.Event.StartEvent();
            if (CurrentWaypoint.Event.SkippEvent)
                HandleWaypointEventEnd();
        else if (CurrentWaypoint.Event is not DialogueEvent)
            Debug.LogError($"{CurrentWaypoint.name} has no event assigned.", CurrentWaypoint);
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

    /// <summary>
    /// Marks the end of the path and performs finalization logic.
    /// </summary>
    /// <remarks>This method sets the <see cref="HasFinished"/> property to <see langword="true"/>  and clears
    /// the <see cref="CurrentWaypoint"/> to indicate that the path traversal is complete. It also logs messages for
    /// debugging purposes.</remarks>
    private void EndPath()
    {
        HasFinished = true;
        CurrentWaypoint = null;
        Debug.Log("Arrived at the last waypoint of the path.");
        Debug.Log("Thanks for testing!");
    }

    /// <summary>
    /// Assign the next waypoint, if any.
    /// </summary>
    private void AssignNextWaypoint()
    {
        int targetIndex = System.Array.IndexOf(_waypoints, CurrentWaypoint) + 1;
        if (targetIndex < _waypoints.Length)
            CurrentWaypoint = _waypoints[targetIndex];
        if (CurrentWaypoint != null && CurrentWaypoint.AutoStart)
        {
        _playerMovement.GoTo(CurrentWaypoint);
            return;
        }
        Debug.Log("Move anywhere to keep moving...");

    }

    #endregion

    #region Public Methods

    /// <summary>
    /// End the current waypoint's event, if any
    /// </summary>
    public void EndCurrentEvent()
    {
        if(CurrentWaypoint == null)
            Debug.Log("No current waypoint to interact with.");
        else if (CurrentWaypoint.Event != null)
        {
            Debug.Log($"Ending current {CurrentWaypoint.Event.GetType()}...");
            CurrentWaypoint.Event.EndEvent();
        }
    }

    #endregion
}
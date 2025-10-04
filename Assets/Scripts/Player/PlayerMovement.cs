using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public event Action OnStartedMoving;
    public event Action OnArrived;

    private PathHandler _pathHandler;
    private InputSystem_Actions _inputActions;
    private NavMeshAgent _agent;
    private bool _isMoving;

    private void Awake()
    {
        // Get instances of required components
        if(!TryGetComponent(out _agent))
            throw new System.NullReferenceException($"No NavMeshAgent component found on {this}.");
        if(!TryGetComponent(out _pathHandler))
            throw new System.NullReferenceException($"No PathHandler component found on {this}.");
        _inputActions = new();

        // Enable the input actions
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        //Prevent memory leaks
        _inputActions?.Disable();
        _inputActions?.Dispose();
    }

    private void Start() => Debug.Log("Move anywhere to start moving through the path (Input System Bindings)");

    private void Update()
    {
        if(_inputActions != null)
            HandleInput();

        if(_isMoving && HasArrived())
        {
            _isMoving = false;
            OnArrived?.Invoke();
        }
    }

    private void HandleInput()
    {
        if(_inputActions.Player.Move.WasPressedThisFrame())
        {
            if(!_isMoving && !_pathHandler.IsBusy && !_pathHandler.IsFinished)
            {
                GoTo(_pathHandler.CurrentWaypoint);
                OnStartedMoving?.Invoke();
            }
            else
            {
                if (_isMoving)
                    Debug.Log("Already moving to a waypoint...");
                else if (_pathHandler.IsBusy)
                    Debug.Log("Path is currently busy...");
            }
        }

        if(_inputActions.Player.Interact.WasPressedThisFrame())
        {
            if(!_isMoving)
            {
                if(_pathHandler.IsBusy)
                    _pathHandler.Interact();
            }
            else
                Debug.Log("Cannot interact while moving...");
        }
    }

    private void GoTo(Waypoint waypoint)
    {
        if(waypoint != null)
        {
            if(_agent.SetDestination(waypoint.transform.position))
                _isMoving = true;
            else
                Debug.LogError($"Failed to set destination for NavMeshAgent on {this}.");
        }
        else
            Debug.LogWarning("There is no waypoint to go to");
    }

    public bool HasArrived() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
}

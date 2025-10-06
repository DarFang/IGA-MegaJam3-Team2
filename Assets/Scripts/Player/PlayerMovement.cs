using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    #region Events
    public event Action OnStartedMoving;
    public event Action OnArrived;
    #endregion

    #region Private Fields

    private PathHandler _pathHandler;
    private InputSystem_Actions _inputActions;
    private NavMeshAgent _agent;
    private bool _isMoving;

    #endregion

    #region Private Methods

    private void Awake() => Initialize();

    /// <summary>
    /// Get required components and enable the input actions
    /// </summary>
    private void Initialize()
    {
        if(!TryGetComponent(out _agent))
            Debug.LogError($"No NavMeshAgent component found on {this}.", this);
        if(!TryGetComponent(out _pathHandler))
            Debug.LogError($"No PathHandler component found on {this}.", this);
        _inputActions = new();

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
        if (GameManager.Instance != null && (GameManager.Instance.IsInCutScene || GameManager.Instance.IsInBattle))
            return;
        if (_inputActions != null)
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
            if (!_isMoving && !_pathHandler.IsBusy && !_pathHandler.HasFinished)
            {
                GoTo(_pathHandler.CurrentWaypoint);
                OnStartedMoving?.Invoke();
                GameManager.Instance.TurnOffInventory();
                GameManager.Instance.TriggerPlayerAction(false);
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
                    _pathHandler.EndCurrentEvent();
            }
            else
                Debug.Log("Cannot interact while moving...");
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Move the player
    /// </summary>
    /// <param name="waypoint">Target waypoint to move to</param>
    public void GoTo(Waypoint waypoint)
    {
        if(waypoint != null)
        {
            if(_agent.SetDestination(waypoint.transform.position))
                _isMoving = true;
            else
                Debug.LogError($"Failed to set destination for NavMeshAgent on {this}.", this);
        }
        else
            Debug.LogWarning("There is no waypoint to go to");
    }
    
    /// <summary>
    /// Move the player
    /// </summary>
    /// <param name="position">The target destination</param>
    public void GoTo(Vector3 position)
    {
        if(_agent.SetDestination(position))
            _isMoving = true;
        else
            Debug.LogError($"Failed to set destination for NavMeshAgent on {this}." , this);
    }
    
    /// <summary>
    /// Move the player
    /// </summary>
    /// <param name="target">target destination transform</param>
    public void GoTo(Transform target)
    {
        if(_agent.SetDestination(target.position))
            _isMoving = true;
        else
            Debug.LogError($"Failed to set destination for NavMeshAgent on {this}." , this);
    }

    /// <returns>
    /// <see langword="true"></see> if the player has arrived to their destination or has none
    /// </returns>
    public bool HasArrived() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;

    #endregion
}

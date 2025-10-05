using System;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    #region Private Fields

    private Animator _animator;
    private PlayerMovement _playerMovement;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponentInParent<PlayerMovement>();
    }
    private void Start() => StartListening();
    private void OnDisable() => StopListening();

    #endregion

    #region Private Methods

    /// <summary>
    /// Subscribe to <see cref="PlayerMovement"/>'s instance events
    /// </summary>
    private void StartListening()
    {
        _playerMovement.OnStartedMoving += SetMoveTrigger;
        _playerMovement.OnArrived += SetIdleTrigger;
    }

    /// <summary>
    /// Prevent memory leaks by unsubscribing from <see cref="PlayerMovement"/>'s instance events
    /// </summary>
    private void StopListening()
    {
        _playerMovement.OnStartedMoving -= SetMoveTrigger;
        _playerMovement.OnArrived -= SetIdleTrigger;
    }

    private void SetMoveTrigger() => _animator.SetTrigger("Move");
    private void SetIdleTrigger() => _animator.SetTrigger("Idle");

    #endregion
}

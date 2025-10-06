using System;
using UnityEngine;

public class PlayerAnimationController : AnimationController
{
    #region Exposed Fields

    [SerializeField] private Player _player;
    [SerializeField] private bool _testing;

    #endregion

    #region Private Fields

    private PlayerMovement _playerMovement;

    #endregion

    #region Protected Fields

    protected override Animator Animator { get; set; }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        _playerMovement = GetComponentInParent<PlayerMovement>();
        if (_player == null)
            Debug.LogError("Player not assigned in inspector", this);
    }
    private void Start() => StartListening();
    private void OnDisable() => StopListening();

    #endregion

    #region Event Handling Methods

    /// <summary>
    /// Subscribe to <see cref="PlayerMovement"/>'s instance events
    /// </summary>
    private void StartListening()
    {
        if (_playerMovement != null)
        {
            _playerMovement.OnStartedMoving += SetMoveTrigger;
            _playerMovement.OnArrived += SetIdleTrigger;
        }

        if (_player != null)
        {
            _player.OnActionPerformed += PlayActionPerformed;
            _player.OnDamageTaken += PlayDamageTaken;
            _player.OnDeath += PlayDead;
        }
    }

    /// <summary>
    /// Prevent memory leaks by unsubscribing from <see cref="PlayerMovement"/>'s instance events
    /// </summary>
    private void StopListening()
    {
        if(_playerMovement != null)
        {
            _playerMovement.OnStartedMoving -= SetMoveTrigger;
            _playerMovement.OnArrived -= SetIdleTrigger;
        }

        if (_player != null)
            _player.OnActionPerformed -= PlayActionPerformed;
    }

    #endregion

    #region Animator Methods

    private void SetMoveTrigger() => Animator.SetTrigger("Move");
    private void SetIdleTrigger() => Animator.SetTrigger("Idle");

    #endregion
}
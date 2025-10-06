using System;
using UnityEngine;

public class PlayerAnimationController : AnimationController
{
    #region Exposed Fields

    [SerializeField] private PlayerUI _playerUI;
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

        if (_playerUI != null)
            _playerUI.OnActionSelected += PlayerUI_OnActionSelected;
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

        if (_playerUI != null)
            _playerUI.OnActionSelected -= PlayerUI_OnActionSelected;
    }

    #endregion

    #region Animator Methods

    private void SetMoveTrigger() => Animator.SetTrigger("Move");
    private void SetIdleTrigger() => Animator.SetTrigger("Idle");
    private void PlayerUI_OnActionSelected(PlayerAction action)
    {
        switch(action)
        {
            case PlayerAction.Attack:
                Animator.SetTrigger("Attack");
                break;
            case PlayerAction.Defense:
                Animator.SetTrigger("Defend");
                break;
            case PlayerAction.Mana:
                Animator.SetTrigger("Mana");
                break;
            case PlayerAction.Heal:
                Animator.SetTrigger("Heal");
                break;
        }
    }

    #endregion
}
using System;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    #region Exposed Fields

    [SerializeField] private PlayerUI _playerUI;
    [SerializeField]private bool _testing;

    #endregion

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

    private void SetMoveTrigger() => _animator.SetTrigger("Move");
    private void SetIdleTrigger() => _animator.SetTrigger("Idle");
    private void PlayerUI_OnActionSelected(PlayerAction action)
    {
        switch(action)
        {
            case PlayerAction.Attack:
                _animator.SetTrigger("Attack");
                break;
            case PlayerAction.Defense:
                _animator.SetTrigger("Defend");
                break;
            case PlayerAction.Mana:
                _animator.SetTrigger("Mana");
                break;
            case PlayerAction.Heal:
                _animator.SetTrigger("Heal");
                break;
        }
    }

    #endregion
}
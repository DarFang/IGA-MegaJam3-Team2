using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
// ==================== PLAYER ====================
public class Player : Entity {
    [SerializeField] private PlayerUI playerUI;

    private UniTaskCompletionSource<PlayerAction> _currentActionSource;
    private CancellationTokenSource _actionCancellationTokenSource;
    private bool _isWaitingForInput = false;

    protected void Start() {

        if (playerUI != null) {
            playerUI.OnActionSelected += HandleActionSelected;
        }
    }

    private void HandleActionSelected(PlayerAction action) {
        if (!_isWaitingForInput) return;

        Debug.Log($"[Player] Action received: {action}");
        _currentActionSource?.TrySetResult(action);
    }

    public override async UniTask DoActionAsync(BattleContext context, CancellationToken cancellationToken = default) {
        if (IsDead) return;

        if (playerUI != null) {
            // �������� UI ��� �����
            _isWaitingForInput = true;
            playerUI.SetButtonsInteractable(true);

            // ��������� ������ ��� ���������� 䳿
            _currentActionSource = new UniTaskCompletionSource<PlayerAction>();
            _actionCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try {
                // ������ �� ���� 䳿 � ���������
                var playerAction = await _currentActionSource.Task
                    .Timeout(TimeSpan.FromSeconds(30)) // ������ �� "���������"
                    .AttachExternalCancellation(_actionCancellationTokenSource.Token);

                ExecutePlayerAction(playerAction, context);

            } catch (TimeoutException) {
                Debug.LogWarning("[Player] Action selection timeout, using random action");
                await base.DoActionAsync(context, cancellationToken);
            } catch (OperationCanceledException) {
                Debug.Log("[Player] Action selection was cancelled");
                await base.DoActionAsync(context, cancellationToken);
            } finally {
                // ��������� ����� ����������
                _isWaitingForInput = false;
                playerUI.SetButtonsInteractable(false);
                _currentActionSource = null;
                _actionCancellationTokenSource?.Dispose();
                _actionCancellationTokenSource = null;
            }
        } else {
            Debug.LogWarning("[Player] No PlayerUI assigned, using random action");
            await base.DoActionAsync(context, cancellationToken);
        }
    }

    private void ExecutePlayerAction(PlayerAction action, BattleContext context) {
        switch (action) {
            case PlayerAction.Attack:
                if (context.Opponent != null && !context.Opponent.IsDead) {
                    Attack(context.Opponent);
                }
                break;

            case PlayerAction.Defense:
                float defenseBoost = Stats.Attack.CurrentValue * defencePercentage;
                ApplyDefenseBuff(defenseBoost);
                break;

            case PlayerAction.Mana:
                if (context.Opponent != null && !context.Opponent.IsDead) {
                    CombatManager.Instance.ManaManager.GainMana(this);
                }
                break;

            case PlayerAction.Heal:
                float healAmount = Stats.Health.MaxValue * healPercentage;
                Heal(healAmount);
                break;
        }
    }

    public void CancelActionSelection() {
        _isWaitingForInput = false;
        _actionCancellationTokenSource?.Cancel();
        _currentActionSource?.TrySetCanceled();

        if (playerUI != null) {
            playerUI.SetButtonsInteractable(false);
        }
    }

    protected void OnDestroy() {
        CancelActionSelection();

        if (playerUI != null) {
            playerUI.OnActionSelected -= HandleActionSelected;
        }
    }
}

public enum PlayerAction {
    Attack,
    Defense,
    Mana,
    Heal
}
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Events;

public class BattleFieldManager : MonoBehaviour {
    [SerializeField] private TurnManager turnManager;
    private Entity player;
    private Entity enemy;
    private Entity currentEntity;
    private bool battleActive;

    public UnityEvent<bool> OnBattleEnd;
    public event Action<Entity> OnTurnStarted;
    public event Action<string> OnActionLogged; // Новий event для логів

    public async void StartBattle(Entity playerEntity, Entity enemyEntity, UpgradedAbilities upgradedAbilities)
    {
        string battleStartMessage = $"<color=#808080>{playerEntity.EntityName}</color> engages in battle with <color=#808080>{enemyEntity.EntityName}</color>!";
        OnActionLogged?.Invoke(battleStartMessage);
        if (playerEntity == null || enemyEntity == null)
        {
            Debug.LogError("[BattleField] Cannot start battle with null entities");
            return;
        }

        player = playerEntity;
        enemy = enemyEntity;

        player.SetUpgradedAbilities(upgradedAbilities);
        enemy.SetUpgradedAbilities(upgradedAbilities);

        battleActive = true;

        player.OnDeath += HandleEntityDeath;
        enemy.OnDeath += HandleEntityDeath;

        // Підписуємось на дії сутностей
        player.OnActionPerformed += HandleActionPerformed;
        enemy.OnActionPerformed += HandleActionPerformed;

        turnManager = new TurnManager();

        Debug.Log($"[BattleField] Battle started: {player.name} vs {enemy.name}");

        await RunBattleLoopAsync();
    }

    private void HandleActionPerformed(BattleAction action)
    {
        string logMessage = action.GetLogMessage();
        Debug.Log($"[BattleLog] {logMessage}");
        OnActionLogged?.Invoke(logMessage);
        if (action.ActionType != BattleActionType.WillGain)
        {
            string manaLogMessage = action.GetManaLogMessage();
            Debug.Log($"[BattleLog] {manaLogMessage}");
            OnActionLogged?.Invoke(manaLogMessage);
        }
    }

    private async UniTask RunBattleLoopAsync() {
        while (battleActive && !player.IsDead && !enemy.IsDead) {
            await ExecuteTurnAsync();
        }
        EndBattle();
    }

    private async UniTask ExecuteTurnAsync() {
        currentEntity = DetermineNextEntity();
        if (currentEntity == null || currentEntity.IsDead) {
            return;
        }

        turnManager.ProceedTurn();
        OnTurnStarted?.Invoke(currentEntity);

        Debug.Log($"[Turn {turnManager.CurrentTurn}] {currentEntity.name}'s turn");

        BattleContext context = new BattleContext(
            currentEntity == player ? enemy : player,
            turnManager,
            this
        );

        await currentEntity.DoActionAsync(context);
        await UniTask.Delay(1000);

        turnManager.EndTurn();
    }

    private Entity DetermineNextEntity() {
        bool isOddTurn = turnManager.CurrentTurn % 2 == 1;
        bool playerIsFaster = player.Stats.Speed.CurrentValue >= enemy.Stats.Speed.CurrentValue;

        if (isOddTurn) {
            return playerIsFaster ? player : enemy;
        } else {
            return playerIsFaster ? enemy : player;
        }
    }

    private void HandleEntityDeath(Entity deadEntity) {
        Debug.Log($"[BattleField] {deadEntity.name} has been defeated!");
        battleActive = false;
    }

    private void EndBattle() {
        Entity winner = player.IsDead ? enemy : player;
        Debug.Log($"[BattleField] Battle ended. Winner: {winner.name}");
        if (winner == enemy)
        {
            GameManager.Instance.GameOver();
            return;
        }
        Destroy(enemy.gameObject);
        OnBattleEnd?.Invoke(winner == player);

        // Відписуємось від подій
        if (player != null) {
            player.OnDeath -= HandleEntityDeath;
            player.OnActionPerformed -= HandleActionPerformed;
        }
        if (enemy != null) {
            enemy.OnDeath -= HandleEntityDeath;
            enemy.OnActionPerformed -= HandleActionPerformed;
        }
    }
}


public class TurnManager {
    private int currentTurn;

    public int CurrentTurn => currentTurn;

    public event Action<int> OnTurnStart;
    public event Action OnTurnEnd;

    public void ProceedTurn() {
        currentTurn++;

        OnTurnStart?.Invoke(currentTurn);
    }

    public void EndTurn() {
        OnTurnEnd?.Invoke();
    }
}
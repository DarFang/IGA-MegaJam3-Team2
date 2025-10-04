using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class BattleFieldManager : MonoBehaviour {
    [SerializeField] private TurnManager turnManager;

    private Entity player;
    private Entity enemy;
    private Entity currentEntity;
    private bool battleActive;

    public event Action<Entity> OnBattleEnd;
    public event Action<Entity> OnTurnStarted;

    // Варіант 1: З використанням UniTask
    public async void StartBattle(Entity playerEntity, Entity enemyEntity) {
        if (playerEntity == null || enemyEntity == null) {
            Debug.LogError("[BattleField] Cannot start battle with null entities");
            return;
        }

        player = playerEntity;
        enemy = enemyEntity;
        battleActive = true;

        player.OnDeath += HandleEntityDeath;
        enemy.OnDeath += HandleEntityDeath;

        turnManager = new TurnManager();

        Debug.Log($"[BattleField] Battle started: {player.name} vs {enemy.name}");

        await RunBattleLoopAsync();
    }

    private async UniTask RunBattleLoopAsync() {
        while (battleActive && !player.IsDead && !enemy.IsDead) {
            await ExecuteTurnAsync();
        }

        if (!battleActive) return;
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

        // Тут чекаємо поки entity виконає дію
        await currentEntity.DoActionAsync(context);

        // Невелика затримка після дії для анімацій
        await UniTask.Delay(300);

        turnManager.EndTurn();
    }

    private Entity DetermineNextEntity() {
        // Простий алгоритм: швидший йде першим у непарні ходи
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
        if (!battleActive) return;

        battleActive = false;

        Entity winner = player.IsDead ? enemy : player;

        Debug.Log($"[BattleField] Battle ended. Winner: {winner.name}");

        OnBattleEnd?.Invoke(winner);

        if (player != null) player.OnDeath -= HandleEntityDeath;
        if (enemy != null) enemy.OnDeath -= HandleEntityDeath;
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
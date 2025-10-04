using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class Entity : MonoBehaviour {
    [SerializeField] private EntityView entityView;
    [SerializeField] private CharacterData characterData;
    [SerializeField] private bool isPlayer = false;

    public Stats Stats { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsPlayer => isPlayer;

    public event Action<Entity> OnDeath;
    public event Action<float> OnDamageTaken;
    public event Action<float> OnHealed;

    private void Awake() {
        Initialize();
    }

    private void Initialize() {
        ResetStats();
        UpdateView();
    }

    public void ResetStats() {
        if (characterData == null) {
            Debug.LogError($"[Entity] Character Data not set for: {name}");
            return;
        }

        Stats = new Stats(characterData.StatsData);
        Stats.Health.OnValueChanged += HandleHealthChanged;
        Stats.Health.OnDeath += HandleDeath;
        Stats.Health.OnDamageTaken += damage => OnDamageTaken?.Invoke(damage);
        IsDead = false;

        HandleHealthChanged(Stats.Health.CurrentValue);
        HandleDefenceUpdate(Stats.Defense.CurrentValue);
    }

    private void HandleHealthChanged(float delta) {
        entityView.UpdateHealth(Stats.Health.CurrentValue, Stats.Health.MaxValue);
    }

    private void HandleDefenceUpdate(float delta) {
        entityView.UpdateDefense(Stats.Defense.CurrentValue);
    }

    private void HandleDeath() {
        if (IsDead) return;

        IsDead = true;
        Debug.Log($"[Entity] {name} has died");
        OnDeath?.Invoke(this);
    }

    private void UpdateView() {
        entityView.UpdateName(characterData.name);
    }

    // ==================== COMBAT ACTIONS ====================

    public void TakeDamage(float rawDamage, Entity attacker = null) {
        if (IsDead) return;

        float finalDamage = CalculateDamage(rawDamage, Stats.Defense.CurrentValue);
        Stats.Health.TakeDamage(finalDamage);

        Debug.Log($"[Combat] {name} took {finalDamage:F1} damage");
    }

    public void Attack(Entity target) {
        if (IsDead || target == null || target.IsDead) return;

        float damage = Stats.Attack.CurrentValue;
        target.TakeDamage(damage, this);

        Debug.Log($"[Combat] {name} attacks {target.name} for {damage:F1} damage");
    }

    public void Heal(float amount) {
        if (IsDead) return;

        Stats.Health.Heal(amount);
        OnHealed?.Invoke(amount);

        Debug.Log($"[Combat] {name} healed for {amount:F1} HP");
    }

    public void ApplyDefenseBuff(float amount) {
        if (IsDead) return;

        Stats.Defense.Add(amount);
        Debug.Log($"[Combat] {name} gained {amount:F1} defense");
    }

    // ==================== BATTLE LOGIC ====================

    public virtual async UniTask DoActionAsync(BattleContext context, CancellationToken cancellationToken = default) {
        if (IsDead) return;

        // За замовчуванням - випадкова дія з невеликою затримкою
        await UniTask.Delay(500, cancellationToken: cancellationToken);
        PerformRandomAction(context);
    }

    protected void PerformRandomAction(BattleContext context) {
        if (context.Opponent == null || context.Opponent.IsDead) return;

        int action = UnityEngine.Random.Range(0, 3);

        switch (action) {
            case 0:
                Attack(context.Opponent);
                break;
            case 1:
                float healAmount = Stats.Health.MaxValue * 0.2f;
                Heal(healAmount);
                break;
            case 2:
                float defenseBoost = Stats.Attack.CurrentValue * 0.5f;
                ApplyDefenseBuff(defenseBoost);
                break;
        }
    }

    private float CalculateDamage(float rawDamage, float defense) {
        float damageReduction = 100f / (100f + defense);
        float finalDamage = rawDamage * damageReduction;
        return Mathf.Max(1f, finalDamage);
    }

    public float GetHealthPercentage() => Stats.Health.GetPercentage();
}

public class BattleContext {
    public Entity Opponent { get; set; }
    public TurnManager TurnManager { get; set; }
    public BattleFieldManager BattleField { get; set; }

    public BattleContext(Entity opponent, TurnManager turnManager, BattleFieldManager battleField) {
        Opponent = opponent;
        TurnManager = turnManager;
        BattleField = battleField;
    }
}
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class Entity : MonoBehaviour {
    [SerializeField] private EntityVisualStatus entityView;
    [SerializeField] private CharacterData characterData;
    public Stats Stats { get; private set; }
    public bool IsDead { get; private set; }

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
        Stats.Defense.OnValueChanged += HandleDefenceUpdate;

        Stats.Health.OnValueChanged += HandleHealthChanged;
        Stats.Health.OnDeath += HandleDeath;
        Stats.Health.OnDamageTaken += HandleDamageTaken;

        IsDead = false;
       

        HandleHealthChanged(Stats.Health.CurrentValue);
        HandleDefenceUpdate(Stats.Defense.CurrentValue);
    }

    private void HandleDamageTaken(float damage) {
        OnDamageTaken?.Invoke(damage);
    }

    private void HandleHealthChanged(float delta) {
        string healthText = $"{Stats.Health.CurrentValue} / {Stats.Health.MaxValue}";
        float percentage = Stats.Health.CurrentValue / Stats.Health.MaxValue;
        entityView.UpdateHealth(percentage, healthText);
    }

    private void HandleDefenceUpdate(float delta) {
        string defenceText = Stats.Defense.CurrentValue.ToString();
        entityView.UpdateDefense(defenceText);
    }

    private void HandleDeath() {
        if (IsDead) return;

        IsDead = true;
        Debug.Log($"[Entity] {name} has died");
        OnDeath?.Invoke(this);
    }

    private void UpdateView() {
        entityView.UpdateName(characterData.Name);
    }

    // ==================== COMBAT ACTIONS ====================

    public void TakeDamage(float rawDamage, Entity attacker = null) {
        if (IsDead) return;

        float finalDamage = CalculateDamage(rawDamage, Stats.Defense.CurrentValue);
        Stats.Health.TakeDamage(finalDamage);

        Debug.Log($"[Combat] {name} took {finalDamage:F1} damage");

        // Анімація шкоди вже викликається через OnDamageTaken подію
    }

    public void Attack(Entity target) {
        if (IsDead || target == null || target.IsDead) return;

        float damage = Stats.Attack.CurrentValue;
        target.TakeDamage(damage, this);

        // Показуємо анімацію атаки для атакуючого
        entityView?.ShowDealDamage(damage);

        Debug.Log($"[Combat] {name} attacks {target.name} for {damage:F1} damage");
    }

    public void Heal(float amount) {
        if (IsDead) return;

        Stats.Health.Heal(amount);
        OnHealed?.Invoke(amount);

        // Показуємо анімацію лікування
        entityView?.ShowHeal(amount);

        Debug.Log($"[Combat] {name} healed for {amount:F1} HP");
    }

    public void ApplyDefenseBuff(float amount) {
        if (IsDead) return;

        Stats.Defense.Add(amount);

        // Показуємо анімацію баффа захисту
        entityView?.ShowDefenseBuff(amount);

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
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour {
    [SerializeField] private EntityVisualStatus entityView;
    [SerializeField] private CharacterData characterData;
    public Stats Stats { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<Entity> OnDeath;
    public event Action<float> OnDamageTaken;
    public event Action<float> OnHealed;

    [Range(0, 1f)]
    [SerializeField] protected float healPercentage = 0.2f;
    [Range(0, 1f)]
    [SerializeField] protected float defencePercentage = 0.3f;

    private CancellationTokenSource _currentActionCts;

    private void Awake() {
        Initialize();
    }

    private void Initialize(CharacterData characterData = null) {
        if (characterData != null) {
            this.characterData = characterData;
        }
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
    }

    public void Attack(Entity target) {
        if (IsDead || target == null || target.IsDead) return;

        float damage = Stats.Attack.CurrentValue;
        float resultDamage = damage = Mathf.Round(damage * 100f) / 100f;

        target.TakeDamage(resultDamage, this);

        entityView?.ShowDealDamage(resultDamage);

        Debug.Log($"[Combat] {name} attacks {target.name} for {resultDamage:F1} damage");
    }

    public void Heal(float amount) {
        if (IsDead) return;

        float resultHeal = amount = Mathf.Round(amount * 100f) / 100f;

        Stats.Health.Heal(resultHeal);
        OnHealed?.Invoke(resultHeal);

        entityView?.ShowHeal(resultHeal);

        Debug.Log($"[Combat] {name} healed for {resultHeal:F1} HP");
    }

    public void ApplyDefenseBuff(float amount) {
        if (IsDead) return;

        float resultDefence = Mathf.Round(amount * 100f) / 100f;
        Stats.Defense.Add(resultDefence);

        entityView?.ShowDefenseBuff(resultDefence);

        Debug.Log($"[Combat] {name} gained {resultDefence:F1} defense");
    }

    // ==================== BATTLE LOGIC ====================

    public virtual async UniTask DoActionAsync(BattleContext context, CancellationToken cancellationToken = default) {
        if (IsDead) return;

        _currentActionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        try {
            await UniTask.Delay(500, cancellationToken: _currentActionCts.Token);

            if (_currentActionCts.Token.IsCancellationRequested) return;

            PerformRandomAction(context);
        } catch (OperationCanceledException) {
            Debug.Log($"[Entity] Action cancelled for: {name}");
        } finally {
            _currentActionCts?.Dispose();
            _currentActionCts = null;
        }
    }

    public void CancelCurrentAction() {
        if (_currentActionCts != null) {
            _currentActionCts.Cancel();
            _currentActionCts.Dispose();
            _currentActionCts = null;

            Debug.Log($"[Entity] Previous action cancelled for: {name}");
        }
    }

    protected void PerformRandomAction(BattleContext context) {
        if (context.Opponent == null || context.Opponent.IsDead) return;

        int action = UnityEngine.Random.Range(0, 3);

        switch (action) {
            case 0:
                Attack(context.Opponent);
                break;
            case 1:
                float healAmount = Stats.Health.MaxValue * healPercentage;
                Heal(healAmount);
                break;
            case 2:
                float defenseBoost = Stats.Attack.CurrentValue * defencePercentage;
                ApplyDefenseBuff(defenseBoost);
                break;
        }
    }

    private float CalculateDamage(float rawDamage, float defense) {
        float damageReduction = 100f / (100f + defense);
        float finalDamage = rawDamage * damageReduction;

        finalDamage = Mathf.Round(finalDamage * 100f) / 100f;
        return Mathf.Max(1f, finalDamage);
    }

    public float GetHealthPercentage() => Stats.Health.GetPercentage();

    private void OnDestroy() {
        CancelCurrentAction();
    }
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
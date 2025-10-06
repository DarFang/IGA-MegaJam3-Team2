using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class Entity : MonoBehaviour {
    [SerializeField] private EntityVisualStatus entityView;
    [SerializeField] private CharacterData characterData;
    public Stats Stats { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<Entity> OnDeath;
    public event Action<float> OnDamageTaken;
    public event Action<float> OnHealed;
    public event Action<BattleAction> OnActionPerformed;

    [Range(0, 1f)]
    [SerializeField] protected float healPercentage = 0.6f;
    [Range(0, 1f)]
    [SerializeField] protected float defencePercentage = 0.3f;

    private CancellationTokenSource _currentActionCts;

    private UpgradedAbilities UpgradedAbilities;

    [SerializeField] private SoundList actionSoundList;

    private void OnEnable() {
        Initialize();
    }

    private void Initialize(CharacterData characterData = null) {
        if (characterData != null) {
            this.characterData = characterData;
        }
        ResetStats();
        UpdateView();
    }

    public void SetUpgradedAbilities(UpgradedAbilities upgradedAbilities) {
        UpgradedAbilities = upgradedAbilities;
    }

    public void ResetStats() {
        if (characterData == null) {
            Debug.LogError($"[Entity] Character Data not set for: {name}");
            return;
        }

        gameObject.name = $"{characterData.Name} (Enemy)";

        Stats = new Stats(characterData.StatsData);
        Stats.Defense.OnValueChanged += HandleDefenceUpdate;

        Stats.Health.OnValueChanged += HandleHealthChanged;
        Stats.Health.OnDeath += HandleDeath;
        Stats.Health.OnDamageTaken += HandleDamageTaken;

        Stats.Mana.OnValueChanged += HandleManaUpdate;

        IsDead = false;

        HandleManaUpdate(Stats.Mana.CurrentValue);
        HandleHealthChanged(Stats.Health.CurrentValue);
        HandleDefenceUpdate(Stats.Defense.CurrentValue);
    }

    private void HandleDamageTaken(float damage) {
        OnDamageTaken?.Invoke(damage);
    }

    private void HandleManaUpdate(float delta) {

        float maxValue = Mathf.Round(Stats.Mana.MaxValue * 100f) / 100f;
        float currentValue = Mathf.Round(Stats.Mana.CurrentValue * 100f) / 100f;

        string resultText = $"{currentValue} / {maxValue}";
        float percentage = currentValue / maxValue;

        entityView.UpdateMana(percentage, resultText);
        //Debug.Log($"[Entity] Mana updated for {name}: {currentValue}/{maxValue}");
    }

    private void HandleHealthChanged(float delta) {
        float maxValue = Mathf.Round(Stats.Health.MaxValue * 100f) / 100f;
        float currentValue = Mathf.Round(Stats.Health.CurrentValue * 100f) / 100f;

        string resultText = $"{currentValue} / {maxValue}";
        float percentage = currentValue / maxValue;

        entityView.UpdateHealth(percentage, resultText);
    }

    private void HandleDefenceUpdate(float delta) {
        float maxValue = Mathf.Round(Stats.Defense.MaxValue * 100f) / 100f;
        float currentValue = Mathf.Round(Stats.Defense.CurrentValue * 100f) / 100f;

        string resultText = $"{currentValue} / {maxValue}";
        float percentage = currentValue / maxValue;

        entityView.UpdateDefense(percentage, resultText);
    }

    private void HandleDeath() {
        if (IsDead) return;

        IsDead = true;
        SoundManager.Instance.CreateSound().Play(actionSoundList.GetSound("Death"));
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

        PlaySound("TakeDamage");
        //Debug.Log($"[Combat] {name} took {finalDamage:F1} damage");
    }

    public void Attack(Entity target) {
        if (IsDead || target == null || target.IsDead) return;

        if (Stats.Mana.CurrentValue < UpgradedAbilities.AttackManaCost) {
            Debug.LogWarning($"[Combat] {name} does not have enough mana to attack.");
            return;
        }

        float manaConsumed = UpgradedAbilities.AttackManaCost;
        CombatManager.Instance?.ManaManager.ConsumeMana(this, (int) manaConsumed);

        float damage = Stats.Attack.CurrentValue;
        float upgradedDamage = damage;
        if (UpgradedAbilities != null) {
            upgradedDamage = damage * (UpgradedAbilities.AttackUpgraded ? 1.5f : 1f);
        }

        float resultDamage = upgradedDamage = Mathf.Round(upgradedDamage * 100f) / 100f;
        target.TakeDamage(resultDamage, this);
        entityView?.ShowDealDamage(resultDamage);
        PlaySound("Attack");

        // Логування дії
        OnActionPerformed?.Invoke(new BattleAction {
            ActionType = BattleActionType.Attack,
            Performer = this,
            Target = target,
            Value = resultDamage,
            ManaConsumed = manaConsumed
        });

        //Debug.Log($"[Combat] {name} attacks {target.name} for {resultDamage:F1} damage");
    }

    private void PlaySound(string soundName) {
        if (actionSoundList == null) return;

        Sound sound = actionSoundList.GetSound(soundName);
        if (sound != null)
            SoundManager.Instance?.CreateSound().AutoDuckMusic().Play(sound);
    }

    public void Heal(float amount) {
        if (IsDead) return;
        if (Stats.Mana.CurrentValue < UpgradedAbilities.HealManaCost) {
            Debug.LogWarning($"[Combat] {name} does not have enough mana to heal.");
            return;
        }

        float manaConsumed = UpgradedAbilities.HealManaCost;
        CombatManager.Instance?.ManaManager.ConsumeMana(this, (int)manaConsumed);

        float upgradedHeal = amount;
        if (UpgradedAbilities != null) {
            upgradedHeal = amount * (UpgradedAbilities.HealUpgraded ? 1.5f : 1f);
        }

        float resultHeal = upgradedHeal = Mathf.Round(upgradedHeal * 100f) / 100f;
        Stats.Health.Heal(resultHeal);
        OnHealed?.Invoke(resultHeal);
        entityView?.ShowHeal(resultHeal);
        PlaySound("Heal");

        // Логування дії
        OnActionPerformed?.Invoke(new BattleAction {
            ActionType = BattleActionType.Heal,
            Performer = this,
            Target = this,
            Value = resultHeal,
            ManaConsumed = manaConsumed
        });

        //Debug.Log($"[Combat] {name} healed for {resultHeal:F1} HP");
    }

    public void ApplyDefenseBuff(float amount) {
        if (IsDead) return;
        if (Stats.Mana.CurrentValue < UpgradedAbilities.DefenseManaCost) {
            Debug.LogWarning($"[Combat] {name} does not have enough mana to defend.");
            return;
        }

        float manaConsumed = UpgradedAbilities.DefenseManaCost;
        CombatManager.Instance?.ManaManager.ConsumeMana(this, (int)manaConsumed);

        float modifiedAmount = amount;
        if (UpgradedAbilities != null) {
            modifiedAmount = amount * (UpgradedAbilities.DefenseUpgraded ? 1.5f : 1f);
        }
        float resultDefence = Mathf.Round(modifiedAmount * 100f) / 100f;

        Stats.Defense.Add(resultDefence);
        entityView?.ShowDefenseBuff(resultDefence);

        // Логування дії
        OnActionPerformed?.Invoke(new BattleAction {
            ActionType = BattleActionType.Defense,
            Performer = this,
            Target = this,
            Value = resultDefence,
            ManaConsumed = manaConsumed
        });

        Debug.Log($"[Combat] {name} gained {resultDefence:F1} defense");
    }

    public void ApplyManaBuff(float amount) {
        if (IsDead) return;

        Stats.Mana.Add(amount);
        entityView?.ShowManaBuff(amount);

        OnActionPerformed?.Invoke(new BattleAction {
            ActionType = BattleActionType.ManaGain,
            Performer = this,
            Target = this,
            Value = amount,
            ManaConsumed = 0
        });

        Debug.Log($"[Combat] {name} gained {amount:F1} mana");
    }

    public void ConsumeMana(float amount)
    {
        if (IsDead) return;
        Debug.Log($"[Combat] {name} consuming {amount} mana");
        amount = math.abs(amount);
        Stats.Mana.ConsumeMana(amount);

    }
    public float amountManaGained()
    {
        return Stats.Mana.RegenerationRate * 10f * (UpgradedAbilities.ManaUpgraded ? 1.5f : 1f);
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

        if (Stats.Mana.CurrentValue < 10)
        {
            CombatManager.Instance?.ManaManager.GainMana(this);
            return;
        }

        int action = UnityEngine.Random.Range(0, 3);

        switch (action)
        {
            case 0:
                Attack(context.Opponent);
                break;
            case 1:
                float healAmount = Stats.Attack.CurrentValue * healPercentage;
                Heal(healAmount);
                break;
            case 2:
                float defenseBoost = Stats.Attack.CurrentValue * defencePercentage;
                ApplyDefenseBuff(defenseBoost);
                break;
            case 3:
                CombatManager.Instance?.ManaManager.GainMana(this);
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

    public string GetName() {
        if (characterData != null) return characterData.Name;
        return gameObject.name;
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

public class BattleAction {
    public BattleActionType ActionType { get; set; }
    public Entity Performer { get; set; }
    public Entity Target { get; set; }
    public float Value { get; set; }
    public float ManaConsumed { get; set; }

    public string GetLogMessage() {
        string performerName = Performer?.GetName() ?? "Unknown";
        string targetName = Target?.GetName() ?? "";
        // string manaInfo = ManaConsumed > 0 ? $" (Consumed {ManaConsumed} mana)" : "";
        string manaInfo = "";

        switch (ActionType)
        {
            case BattleActionType.Attack:
                return $"{performerName} dealt {Value:F1} damage to {targetName}{manaInfo}";

            case BattleActionType.Heal:
                return $"{performerName} healed {Value:F1} HP{manaInfo}";

            case BattleActionType.Defense:
                return $"{performerName} gained {Value:F1} defense{manaInfo}";

            case BattleActionType.ManaGain:
                return $"{performerName} gained {Value:F1} mana";

            default:
                return $"{performerName} performed unknown action";
        }
    }
}

public enum BattleActionType {
    Attack,
    Heal,
    Defense,
    ManaGain
}
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting;
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
        Debug.Log($"[Entity] Mana updated for {name}: {Stats.Mana.CurrentValue}/{Stats.Mana.MaxValue}");
        string manaText = $"{Stats.Mana.CurrentValue} / {Stats.Mana.MaxValue}";
        float percentage = Stats.Mana.CurrentValue / Stats.Mana.MaxValue;
        entityView.UpdateMana(percentage, manaText);
    }

    private void HandleHealthChanged(float delta) {
        float maxHealth = Mathf.Round(Stats.Health.MaxValue * 100f) / 100f;
        float currentValue = Mathf.Round(Stats.Health.CurrentValue * 100f) / 100f;


        string healthText = $"{currentValue} / {maxHealth}";
        float percentage = Stats.Health.CurrentValue / Stats.Health.MaxValue;
        entityView.UpdateHealth(percentage, healthText);
    }

    private void HandleDefenceUpdate(float delta) {
        float defenceValue = Mathf.Round(Stats.Defense.CurrentValue * 100f) / 100f;
        string defenceText = defenceValue.ToString();
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

        PlaySound("TakeDamage");
        Debug.Log($"[Combat] {name} took {finalDamage:F1} damage");
    }

    public void Attack(Entity target) {
        if (IsDead || target == null || target.IsDead) return;

        if (Stats.Mana.CurrentValue < UpgradedAbilities.AttackManaCost) {
            Debug.LogWarning($"[Combat] {name} does not have enough mana to attack.");
            return;
        }
        CombatManager.Instance?.ManaManager.ConsumeMana(this, UpgradedAbilities.AttackManaCost);
        float damage = Stats.Attack.CurrentValue;
        float upgradedDamage = damage;
        if (UpgradedAbilities != null)
        {
            upgradedDamage = damage * (UpgradedAbilities.AttackUpgraded ? 1.5f : 1f);
        }

        float resultDamage = upgradedDamage = Mathf.Round(upgradedDamage * 100f) / 100f;

        target.TakeDamage(resultDamage, this);

        entityView?.ShowDealDamage(resultDamage);

        PlaySound("Attack");
        Debug.Log($"[Combat] {name} attacks {target.name} for {resultDamage:F1} damage");
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
            Debug.LogWarning($"[Combat] {name} does not have enough mana to attack.");
            return;
        }
        CombatManager.Instance?.ManaManager.ConsumeMana(this, UpgradedAbilities.HealManaCost);

        float upgradedHeal = amount;
        if (UpgradedAbilities != null) {
            upgradedHeal = amount * (UpgradedAbilities.HealUpgraded ? 1.5f : 1f);
        }

        float resultHeal = upgradedHeal = Mathf.Round(upgradedHeal * 100f) / 100f;

        Stats.Health.Heal(resultHeal);
        OnHealed?.Invoke(resultHeal);

        entityView?.ShowHeal(resultHeal);

        PlaySound("Heal");

        Debug.Log($"[Combat] {name} healed for {resultHeal:F1} HP");
    }

    public void ApplyDefenseBuff(float amount) {
        if (IsDead) return;
        if (Stats.Mana.CurrentValue < UpgradedAbilities.DefenseManaCost) {
            Debug.LogWarning($"[Combat] {name} does not have enough mana to attack.");
            return;
        }
        CombatManager.Instance?.ManaManager.ConsumeMana(this, UpgradedAbilities.DefenseManaCost);
        float modifiedAmount = amount;
        if (UpgradedAbilities != null) {
            modifiedAmount = amount * (UpgradedAbilities.DefenseUpgraded ? 1.5f : 1f);
        }
        float resultDefence = Mathf.Round(modifiedAmount * 100f) / 100f;

        Stats.Defense.Add(resultDefence);


        entityView?.ShowDefenseBuff(resultDefence);

        Debug.Log($"[Combat] {name} gained {resultDefence:F1} defense");
    }

    public void ApplyManaBuff(float amount) {
        if (IsDead) return;

        float modifiedAmount = amount;
        if (UpgradedAbilities != null) {
            modifiedAmount = amount * (UpgradedAbilities.ManaUpgraded ? 1.5f : 1f);
        }

        if (modifiedAmount < 0) {
            Stats.Mana.ConsumeMana(-modifiedAmount);
        } else {
            Stats.Mana.Add(modifiedAmount);
        }
        // �������� �������� ����� �������
        entityView?.ShowManaBuff(modifiedAmount);

        Debug.Log($"[Combat] {name} gained {amount:F1} mana");
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
            CombatManager.Instance?.ManaManager.GainMana(this, 10);
            return;
        }

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

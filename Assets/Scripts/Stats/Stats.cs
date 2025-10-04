using Language.Lua;
using System;
using UnityEngine;

[System.Serializable]
public class Stats {
    public Health Health { get; private set; }
    public Stat Attack { get; private set; }
    public Stat Defense { get; private set; }
    public Mana Mana { get; private set; }
    public Stat Speed { get; private set; }

    public Stats(StatsData statsData) {
        if (statsData == null) {
            Debug.LogError($"[Stats] StatsData is null");
            return;
        }

        // Кожна характеристика створюється зі своїми спеціалізованими даними
        Health = new Health(statsData.HealthData);
        Attack = new Stat(statsData.AttackData);
        Defense = new Stat(statsData.DefenseData);
        Mana = new Mana(statsData.ManaData);
        Speed = new Stat(statsData.SpeedData);
    }
}
public class Stat {
    public float BaseValue { get; protected set; }
    public float MaxValue { get; protected set; }
    public float MinValue { get; protected set; }
    public float CurrentValue { get; protected set; }

    public event Action<float> OnValueChanged;

    public Stat(StatData data) {
        if (data == null) {
            Debug.LogError("[Stat] StatData is null");
            return;
        }

        BaseValue = data.BaseValue;
        MaxValue = data.MaxValue;
        MinValue = data.MinValue;
        CurrentValue = Mathf.Clamp(data.BaseValue, MinValue, MaxValue);
    }

    public Stat(float currentValue, float maxValue = -1, float minValue = 0) {
        if (maxValue == -1) MaxValue = currentValue;
        MinValue = minValue;
        CurrentValue = currentValue < 0 ? maxValue : Mathf.Clamp(currentValue, minValue, maxValue);
    }

    public virtual void Add(float value) {
        if (value <= 0) return;
        SetValue(Mathf.Clamp(CurrentValue + value, MinValue, MaxValue));
    }

    public virtual void Subtract(float value) {
        if (value <= 0) return;
        SetValue(Mathf.Clamp(CurrentValue - value, MinValue, MaxValue));
    }

    protected virtual void SetValue(float newValue) {
        if (Mathf.Approximately(CurrentValue, newValue)) return;

        float delta = newValue - CurrentValue;
        CurrentValue = newValue;
        OnValueChanged?.Invoke(delta);
    }

    public void Reset() => SetValue(MaxValue);
    public float GetPercentage() => MaxValue > 0 ? CurrentValue / MaxValue : 0f;
}

// ==================== HEALTH ====================
public class Health : Stat {
    public bool HasRegeneration { get; private set; }
    public float RegenerationRate { get; private set; }

    public event Action<float> OnDamageTaken;
    public event Action<float> OnHealed;
    public event Action OnDeath;

    // Конструктор з HealthData
    public Health(RegenerableStatData healthData) : base(healthData) {
        if (healthData != null) {
            HasRegeneration = healthData.HasRegeneration;
            RegenerationRate = healthData.Regeneration;
        }
    }

    // Старий конструктор для сумісності
    public Health(float currentValue, float maxValue = -1) : base(currentValue, maxValue) {
        HasRegeneration = false;
        RegenerationRate = 0f;
    }

    public void Regenerate() {
        if (HasRegeneration && CurrentValue < MaxValue) {
            Heal(RegenerationRate * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage) {
        if (damage <= 0) return;

        Subtract(damage);
        OnDamageTaken?.Invoke(damage);

        if (CurrentValue <= MinValue) {
            OnDeath?.Invoke();
        }
    }

    public void Heal(float healAmount) {
        if (healAmount <= 0) return;

        float actualHeal = Mathf.Min(healAmount, MaxValue - CurrentValue);
        Add(actualHeal);
        OnHealed?.Invoke(actualHeal);
    }
}

public class Mana : Stat {
    public float RegenerationRate { get; private set; }

    public Mana(RegenerableStatData manaData) : base(manaData) {
        if (manaData != null) {
            RegenerationRate = manaData.Regeneration;
        }
    }

    public void Regenerate() {
        if (CurrentValue < MaxValue) {
            Add(RegenerationRate * Time.deltaTime);
        }
    }
}

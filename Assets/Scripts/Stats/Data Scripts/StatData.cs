using System;
using UnityEngine;

[Serializable]
public class StatData {
    private const float ROUND_FACTOR = 100f; // 10^2

    [SerializeField] private float baseValue = 100;
    [SerializeField] private float maxValue = 100;
    [SerializeField] private float minValue = 0f;

    public float BaseValue {
        get {
            return Mathf.Round(baseValue * ROUND_FACTOR) / ROUND_FACTOR;
        }
        set {
            float clampedValue = Mathf.Clamp(value, MinValue, MaxValue);
            baseValue = Mathf.Round(clampedValue * ROUND_FACTOR) / ROUND_FACTOR;
        }
    }

    public float MaxValue {
        get => Mathf.Round(maxValue * ROUND_FACTOR) / ROUND_FACTOR;
        set => maxValue = Mathf.Round(value * ROUND_FACTOR) / ROUND_FACTOR;
    }

    public float MinValue {
        get => Mathf.Round(minValue * ROUND_FACTOR) / ROUND_FACTOR;
        set => minValue = Mathf.Round(value * ROUND_FACTOR) / ROUND_FACTOR;
    }
}
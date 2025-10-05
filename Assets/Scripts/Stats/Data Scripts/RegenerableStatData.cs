using System;
using UnityEngine;

[Serializable]
public class RegenerableStatData : StatData {
    [SerializeField] private float regeneration = 1f;
    [SerializeField] private bool hasRegeneration = false;

    // Властивість Regeneration з доданим округленням до двох знаків після коми
    public float Regeneration {
        get {
            // Використовуємо Math.Round для округлення до двох знаків
            return (float)Math.Round(regeneration, 2);
        }
        set {
            // Округлюємо вхідне значення перед його присвоєнням
            regeneration = (float)Math.Round(value, 2);
        }
    }

    public bool HasRegeneration => hasRegeneration;
}
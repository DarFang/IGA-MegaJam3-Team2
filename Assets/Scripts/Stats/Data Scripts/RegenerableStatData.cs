using System;
using UnityEngine;

[Serializable]
public class RegenerableStatData : StatData {
    [SerializeField] private float regeneration = 1f;
    [SerializeField] private bool hasRegeneration = false;

    public float Regeneration => regeneration;
    public bool HasRegeneration => hasRegeneration;
}
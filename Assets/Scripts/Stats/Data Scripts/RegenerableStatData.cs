using System;
using UnityEngine;

[Serializable]
public class RegenerableStatData : StatData {
    [SerializeField] private float regeneration = 1f;
    [SerializeField] private bool hasRegeneration = false;

    // ���������� Regeneration � ������� ����������� �� ���� ����� ���� ����
    public float Regeneration {
        get {
            // ������������� Math.Round ��� ���������� �� ���� �����
            return (float)Math.Round(regeneration, 2);
        }
        set {
            // ���������� ������ �������� ����� ���� ����������
            regeneration = (float)Math.Round(value, 2);
        }
    }

    public bool HasRegeneration => hasRegeneration;
}
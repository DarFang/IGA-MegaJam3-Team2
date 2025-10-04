using System;
using UnityEngine;

[Serializable]
public class StatData {

    [SerializeField] private float baseValue = 100;
    [SerializeField] private float maxValue = 100;
    [SerializeField] private float minValue = 0f;

    public float BaseValue {
        get => baseValue;
        set {
            // !!! �������� ����� ��������� !!!
            // �������� ���� �������� 'value' �� MinValue �� MaxValue
            baseValue = Mathf.Clamp(value, minValue, maxValue);
        }
    }

    // ����� ������������ ����������� ��������
    public float MaxValue {
        get => maxValue;
        set => maxValue = value;
    }
    public float MinValue {
        get => minValue;
        set => minValue = value;
    }
}

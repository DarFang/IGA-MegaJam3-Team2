using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Inventory/Consumable Item")]
public class ConsumableItem : InventoryItem {
    [Header("Consumable Properties")]
    [SerializeField] private float healthRestore;
    [SerializeField] private float manaRestore;
    [SerializeField] private float effectDuration;

    public float HealthRestore => healthRestore;
    public float ManaRestore => manaRestore;
    public float EffectDuration => effectDuration;

    public void Consume() {
        Debug.Log($"Consumed {ItemName}");
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Inventory/Equipment Item")]
public class EquipmentItem : InventoryItem {
    [Header("Equipment Properties")]
    [SerializeField] private EquipmentSlot equipSlot;
    [SerializeField] private float attackBonus;
    [SerializeField] private float defenseBonus;
    [SerializeField] private float speedBonus;

    public EquipmentSlot EquipSlot => equipSlot;
    public float AttackBonus => attackBonus;
    public float DefenseBonus => defenseBonus;
    public float SpeedBonus => speedBonus;
}

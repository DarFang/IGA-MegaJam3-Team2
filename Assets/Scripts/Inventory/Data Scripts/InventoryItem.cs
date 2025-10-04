using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject {
    [Header("Basic Info")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [SerializeField][TextArea(3, 10)] private string itemLore;
    [SerializeField] private ItemType itemType;
    [SerializeField] private Rarity rarity;

    [Header("Stack Settings")]
    [SerializeField] private bool isStackable = true;
    [SerializeField] private int maxStackSize = 99;

    [Header("Optional Ability")]
    [SerializeField] private bool hasAbility = false;
    [SerializeField] private ItemAbility ability;

    // Properties
    public string ItemName => itemName;
    public Sprite ItemIcon => itemIcon;
    public string ItemLore => itemLore;
    public ItemType Type => itemType;
    public Rarity Rarity => rarity;
    public bool IsStackable => isStackable;
    public int MaxStackSize => maxStackSize;
    public bool HasAbility => hasAbility;
    public ItemAbility Ability => ability;
}

[System.Serializable]
public class ItemAbility {
    [SerializeField] private string abilityName;
    [SerializeField] private Sprite abilityIcon;
    [SerializeField][TextArea(3, 6)] private string abilityDescription;
    [SerializeField] private float cooldown;
    [SerializeField] private float manaCost;

    public string AbilityName => abilityName;
    public Sprite AbilityIcon => abilityIcon;
    public string AbilityDescription => abilityDescription;
    public float Cooldown => cooldown;
    public float ManaCost => manaCost;

    public bool HasAbility => !string.IsNullOrEmpty(abilityName);
}

public enum EquipmentSlot {
    Weapon,
    Helmet,
    Chest,
    Legs,
    Boots,
    Accessory
}


public enum ItemType {
    Consumable,
    Equipment,
    QuestItem,
    Material,
    Special
}

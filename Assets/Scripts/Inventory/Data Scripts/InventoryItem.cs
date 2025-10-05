using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject {
    [Header("Basic Info")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [TextArea(3, 5)]
    [SerializeField] private string description;

    [Header("Unknown State")]
    [SerializeField] private bool startsUnknown = false;
    [SerializeField] private string unknownName = "Unknown Item";
    [SerializeField] private Sprite unknownIcon;
    [TextArea(3, 5)]
    [SerializeField] private string unknownDescription = "You don't know what this item is. Use an identification scroll or visit a merchant to identify it.";

    [Header("Stack Settings")]
    [SerializeField] private bool isStackable = true;
    [SerializeField] private int maxStackSize = 99;

    [Header("Properties")]
    [SerializeField] private Rarity rarity = Rarity.Common;
    [SerializeField] private ItemType itemType = ItemType.Material;

    // Equipment specific (використовується тільки для екіпірування)
    [SerializeField] private EquipmentSlot equipmentSlot = EquipmentSlot.Weapon;
    [SerializeField] private ItemAbility primaryAbility;

    // Runtime identification state
    private HashSet<int> identifiedInstances = new HashSet<int>();

    public string ItemName => itemName;
    public Sprite ItemIcon => itemIcon;
    public string Description => description;
    public string UnknownName => unknownName;
    public Sprite UnknownIcon => unknownIcon != null ? unknownIcon : itemIcon;
    public string UnknownDescription => unknownDescription;
    public bool IsStackable => isStackable;
    public int MaxStackSize => maxStackSize;
    public Rarity Rarity => rarity;
    public bool StartsUnknown => startsUnknown;
    public ItemType ItemType => itemType;
    public EquipmentSlot EquipmentSlot => equipmentSlot;
    public ItemAbility PrimaryAbility => primaryAbility;

    public string GetDisplayName(bool isIdentified) {
        return isIdentified ? itemName : unknownName;
    }

    public Sprite GetDisplayIcon(bool isIdentified) {
        return isIdentified ? itemIcon : UnknownIcon;
    }

    public string GetDisplayDescription(bool isIdentified) {
        return isIdentified ? description : unknownDescription;
    }

    public Rarity GetDisplayRarity(bool isIdentified) {
        return isIdentified ? rarity : Rarity.Common;
    }

    // Методи для управління ідентифікацією
    public void IdentifyInstance(int instanceId) {
        identifiedInstances.Add(instanceId);
    }

    public bool IsInstanceIdentified(int instanceId) {
        return identifiedInstances.Contains(instanceId);
    }
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

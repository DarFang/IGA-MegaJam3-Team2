using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Inventory/Inventory System")]
public class InventoryData : ScriptableObject {
    public int inventorySize = 20;
    public List<InventoryItemCollection> itemCollections;
}

[System.Serializable]
public class InventoryItemCollection {

    public InventoryItem item;
    public int quantity = 1;
}
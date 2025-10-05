using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Inventory/Inventory System")]

public class InventoryData : ScriptableObject {
    public int inventorySize = 20;
    public List<InventoryItem> inventoryItems;
}
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Inventory/Inventory System")]
public class Inventory : ScriptableObject {
    [SerializeField] private int inventorySize = 20;
    [SerializeField] private List<InventorySlot> slots = new();

    public int InventorySize => inventorySize;
    public List<InventorySlot> Slots => slots;

    public Action<InventorySlot> OnSlotUpdated { get; internal set; }
    public Action OnInventoryChanged { get; internal set; }

    public bool AddItem(InventoryItem item, int amount = 1) {
        // Спочатку шукаємо існуючі стеки
        if (item.IsStackable) {
            for (int i = 0; i < slots.Count; i++) {
                if (!slots[i].IsEmpty && slots[i].Item == item) {
                    amount = slots[i].AddToStack(amount);
                    if (amount == 0) return true;
                }
            }
        }

        // Потім шукаємо пусті слоти
        for (int i = 0; i < slots.Count; i++) {
            if (slots[i].IsEmpty) {
                amount = slots[i].AddToStack(amount);
                if (amount == 0) return true;
            }
        }

        return amount == 0;
    }

    public bool RemoveItem(InventoryItem item, int amount = 1) {
        for (int i = 0; i < slots.Count; i++) {
            if (!slots[i].IsEmpty && slots[i].Item == item) {
                slots[i].RemoveFromStack(amount);
                return true;
            }
        }
        return false;
    }

    public int GetItemCount(InventoryItem item) {
        int count = 0;
        for (int i = 0; i < slots.Count; i++) {
            if (!slots[i].IsEmpty && slots[i].Item == item) {
                count += slots[i].Quantity;
            }
        }
        return count;
    }

    public bool HasItem(InventoryItem item) {
        return GetItemCount(item) > 0;
    }

    public void ClearInventory() {
        for (int i = 0; i < slots.Count; i++) {
            slots[i].Clear();
        }
    }

    public InventorySlot GetSlot(int index) {
        if (index >= 0 && index < slots.Count) {
            return slots[index];
        }
        return null;
    }
}


[System.Serializable]
public class InventorySlot {
    [SerializeField] private InventoryItem item;
    [SerializeField] private int quantity;

    public InventoryItem Item => item;
    public int Quantity => quantity;
    public bool IsEmpty => item == null || quantity <= 0;
    public bool IsFull => !IsEmpty && quantity >= item.MaxStackSize;

    public InventorySlot() {
        item = null;
        quantity = 0;
    }

    public InventorySlot(InventoryItem item, int quantity = 1) {
        this.item = item;
        this.quantity = quantity;
    }

    public int AddToStack(int amount) {
        if (IsEmpty || amount <= 0) return 0;

        int spaceAvailable = item.MaxStackSize - quantity;
        int toAdd = Mathf.Min(spaceAvailable, amount);
        quantity += toAdd;
        return toAdd;
    }

    public bool RemoveFromStack(int amount) {
        if (IsEmpty || amount <= 0) return false;

        quantity -= amount;
        if (quantity <= 0) {
            Clear();
        }
        return true;
    }

    public void SetItem(InventoryItem newItem, int newQuantity) {
        item = newItem;
        quantity = Mathf.Clamp(newQuantity, 0, newItem?.MaxStackSize ?? 0);

        if (quantity <= 0) {
            item = null;
        }
    }

    public void Clear() {
        item = null;
        quantity = 0;
    }

    public bool CanAcceptItem(InventoryItem itemToCheck, int amount = 1) {
        if (IsEmpty) return true;
        if (item != itemToCheck) return false;
        if (!item.IsStackable) return false;
        return quantity + amount <= item.MaxStackSize;
    }
}
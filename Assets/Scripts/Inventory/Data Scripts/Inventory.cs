using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour {
    [SerializeField] private int maxCapacity = 100;

    private List<InventorySlot> items = new List<InventorySlot>();

    public IReadOnlyList<InventorySlot> Items => items;
    public int Count => items.Count;
    public int MaxCapacity => maxCapacity;

    public event Action<InventorySlot> OnItemAdded;
    public event Action<InventorySlot> OnItemRemoved;
    public event Action<InventorySlot> OnItemUpdated;
    public event Action<InventorySlot> OnItemIdentified;
    public event Action OnInventoryChanged;

    [SerializeField] InventoryData inventoryData;

    private void Awake() {
        items.Clear();
        InitInventoryData(inventoryData);
    }

    public void InitInventoryData(InventoryData inventoryData) {
        if (inventoryData == null) return;
        foreach (var item in inventoryData.inventoryItems) {
            AddItem(item);
        }
    }

    public bool AddItem(InventoryItem item, int amount = 1, bool? isIdentified = null) {
        if (item == null || amount <= 0) return false;

        if (!item.IsStackable && items.Count >= maxCapacity) {
            return false;
        }

        int remainingAmount = amount;
        bool shouldBeIdentified = isIdentified ?? !item.StartsUnknown;

        // Якщо stackable - знайти існуючий стек з таким самим станом ідентифікації
        if (item.IsStackable) {
            InventorySlot existingSlot = items.FirstOrDefault(s =>
                s.Item == item && s.IsIdentified == shouldBeIdentified);

            if (existingSlot != null) {
                int spaceInStack = item.MaxStackSize - existingSlot.Quantity;
                int toAdd = Mathf.Min(spaceInStack, remainingAmount);

                if (toAdd > 0) {
                    existingSlot.AddToStack(toAdd);
                    remainingAmount -= toAdd;
                    OnItemUpdated?.Invoke(existingSlot);
                }
            }
        }

        // Створити нові слоти
        while (remainingAmount > 0) {
            if (!item.IsStackable && items.Count >= maxCapacity) {
                break;
            }

            int toAdd = Mathf.Min(remainingAmount, item.MaxStackSize);
            InventorySlot newSlot = new InventorySlot(item, toAdd, shouldBeIdentified);
            items.Add(newSlot);
            remainingAmount -= toAdd;

            OnItemAdded?.Invoke(newSlot);
        }

        if (remainingAmount != amount) {
            OnInventoryChanged?.Invoke();
        }

        return remainingAmount == 0;
    }

    public bool RemoveItem(InventoryItem item, int amount = 1) {
        if (item == null || amount <= 0) return false;

        int remainingToRemove = amount;

        for (int i = items.Count - 1; i >= 0 && remainingToRemove > 0; i--) {
            if (items[i].Item == item) {
                int toRemove = Mathf.Min(remainingToRemove, items[i].Quantity);
                items[i].RemoveFromStack(toRemove);
                remainingToRemove -= toRemove;

                if (items[i].IsEmpty) {
                    InventorySlot removedSlot = items[i];
                    items.RemoveAt(i);
                    OnItemRemoved?.Invoke(removedSlot);
                } else {
                    OnItemUpdated?.Invoke(items[i]);
                }
            }
        }

        if (remainingToRemove != amount) {
            OnInventoryChanged?.Invoke();
        }

        return remainingToRemove == 0;
    }

    public bool RemoveItemAt(int index, int amount = 1) {
        if (index < 0 || index >= items.Count) return false;

        InventorySlot slot = items[index];
        int toRemove = Mathf.Min(amount, slot.Quantity);

        slot.RemoveFromStack(toRemove);

        if (slot.IsEmpty) {
            items.RemoveAt(index);
            OnItemRemoved?.Invoke(slot);
        } else {
            OnItemUpdated?.Invoke(slot);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool IdentifyItemAt(int index) {
        if (index < 0 || index >= items.Count) return false;

        InventorySlot slot = items[index];

        if (!slot.IsIdentified) {
            slot.Identify();
            OnItemIdentified?.Invoke(slot);
            OnItemUpdated?.Invoke(slot);
            return true;
        }

        return false;
    }

    public void IdentifyAllItems() {
        bool anyIdentified = false;

        foreach (var slot in items) {
            if (!slot.IsIdentified) {
                slot.Identify();
                OnItemIdentified?.Invoke(slot);
                anyIdentified = true;
            }
        }

        if (anyIdentified) {
            OnInventoryChanged?.Invoke();
        }
    }

    public int GetUnidentifiedCount() {
        return items.Count(s => !s.IsIdentified);
    }

    public int GetItemCount(InventoryItem item) {
        if (item == null) return 0;
        return items.Where(s => s.Item == item).Sum(s => s.Quantity);
    }

    public bool HasItem(InventoryItem item, int amount = 1) {
        return GetItemCount(item) >= amount;
    }

    public InventorySlot GetItemAt(int index) {
        if (index >= 0 && index < items.Count) {
            return items[index];
        }
        return null;
    }

    public int GetItemIndex(InventorySlot slot) {
        return items.IndexOf(slot);
    }

    public void ClearInventory() {
        items.Clear();
        OnInventoryChanged?.Invoke();
    }

    public bool IsFull() {
        return items.Count >= maxCapacity;
    }

    public void SortByRarity() {
        items.Sort((a, b) => {
            int rarityCompare = b.GetDisplayRarity().CompareTo(a.GetDisplayRarity());
            if (rarityCompare != 0) return rarityCompare;
            return string.Compare(a.GetDisplayName(), b.GetDisplayName());
        });
        OnInventoryChanged?.Invoke();
    }

    public void SortByName() {
        items.Sort((a, b) => string.Compare(a.GetDisplayName(), b.GetDisplayName()));
        OnInventoryChanged?.Invoke();
    }

    public void SortByIdentificationStatus() {
        items.Sort((a, b) => {
            int identifiedCompare = b.IsIdentified.CompareTo(a.IsIdentified);
            if (identifiedCompare != 0) return identifiedCompare;
            return string.Compare(a.GetDisplayName(), b.GetDisplayName());
        });
        OnInventoryChanged?.Invoke();
    }
}

[System.Serializable]
public class InventorySlot {
    [SerializeField] private InventoryItem item;
    [SerializeField] private int quantity;
    [SerializeField] private bool isIdentified;

    public InventoryItem Item => item;
    public int Quantity => quantity;
    public bool IsIdentified => isIdentified;
    public bool IsEmpty => quantity <= 0;
    public bool IsFull => quantity >= item.MaxStackSize;

    public InventorySlot(InventoryItem item, int quantity = 1, bool? isIdentified = null) {
        if (item == null) {
            throw new ArgumentNullException(nameof(item));
        }

        this.item = item;
        this.quantity = Mathf.Clamp(quantity, 1, item.MaxStackSize);
        this.isIdentified = isIdentified ?? !item.StartsUnknown;
    }

    public int AddToStack(int amount) {
        if (amount <= 0) return 0;

        int spaceAvailable = item.MaxStackSize - quantity;
        int toAdd = Mathf.Min(spaceAvailable, amount);
        quantity += toAdd;

        return toAdd;
    }

    public int RemoveFromStack(int amount) {
        if (amount <= 0) return 0;

        int toRemove = Mathf.Min(quantity, amount);
        quantity -= toRemove;

        return toRemove;
    }

    public void Identify() {
        if (!isIdentified) {
            isIdentified = true;
            Debug.Log($"Identified: {item.ItemName}!");
        }
    }

    public string GetDisplayName() => item.GetDisplayName(isIdentified);
    public Sprite GetDisplayIcon() => item.GetDisplayIcon(isIdentified);
    public string GetDisplayDescription() => item.GetDisplayDescription(isIdentified);
    public Rarity GetDisplayRarity() => item.GetDisplayRarity(isIdentified);
}
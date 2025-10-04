using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InventoryUI : MonoBehaviour {
    [Header("Inventory Reference")]
    [SerializeField] private Inventory inventory;

    [Header("UI Components")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private ItemDetailPanel detailPanel;
    [SerializeField] private Button useButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button sortButton;

    [SerializeField] private Button closeButton;

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.I;

    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
    private InventorySlotUI selectedSlotUI;

    private void Start() {
        InitializeUI();
        inventoryPanel.SetActive(false);

        inventory.OnSlotUpdated += OnSlotUpdated;
        inventory.OnInventoryChanged += OnInventoryChanged;
    }

    private void Update() {
        if (Input.GetKeyDown(toggleKey)) {
            ToggleInventory();
        }
    }

    private void InitializeUI() {
        foreach (Transform child in slotContainer) {
            Destroy(child.gameObject);
        }
        slotUIs.Clear();

        for (int i = 0; i < inventory.Slots.Count; i++) {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            slotUI.SetIndex(i);

            slotUI.OnClicked += SelectSlot;

            slotUIs.Add(slotUI);
        }

        RefreshAllSlots();

        if (useButton != null)
            useButton.onClick.AddListener(UseSelectedItem);
        if (dropButton != null)
            dropButton.onClick.AddListener(DropSelectedItem);
        if (sortButton != null)
            sortButton.onClick.AddListener(SortInventory);
        if (closeButton != null)
            closeButton.onClick.AddListener(ToggleInventory);
    }

    private void RefreshAllSlots() {
        for (int i = 0; i < slotUIs.Count; i++) {
            var slot = inventory.GetSlot(i);
            slotUIs[i].UpdateSlot(slot);
        }
    }

    private void RefreshSlot(int index) {
        if (index >= 0 && index < slotUIs.Count) {
            var slot = inventory.GetSlot(index);
            slotUIs[index].UpdateSlot(slot);
        }
    }

    private void OnSlotUpdated(InventorySlot slot) {
        int index = inventory.Slots.IndexOf(slot);
        if (index != -1) {
            RefreshSlot(index);
        }
    }

    private void OnInventoryChanged() {
        RefreshAllSlots();
        UpdateActionButtons();
    }

    public void SelectSlot(InventorySlotUI slotUI, int index) {
        // Скасовуємо попередній вибір
        if (selectedSlotUI != null) {
            selectedSlotUI.SetSelected(false);
        }

        selectedSlotUI = slotUI;
        selectedSlotUI.SetSelected(true);

        var slot = inventory.GetSlot(index);
        if (slot != null && !slot.IsEmpty) {
            detailPanel.ShowItemDetails(slot.Item);
        } else {
            detailPanel.Hide();
        }

        UpdateActionButtons();
    }

    private void UpdateActionButtons() {
        if (selectedSlotUI == null) {
            return;
        }

        var slot = inventory.GetSlot(selectedSlotUI.Index);
        bool hasValidSelection = slot != null && !slot.IsEmpty;
        UpdateButtonInteractivity(hasValidSelection);
    }

    public void UpdateButtonInteractivity(bool isEnabled) {
        if (useButton != null) {
            useButton.interactable = isEnabled;
        }
        
        if (dropButton != null) {
            dropButton.interactable = isEnabled;
        }
       
    }

    public void ToggleInventory() {
        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);

        if (isActive) {
            RefreshAllSlots();
            ClearSelection();
        } else {
            detailPanel.Hide();
        }
    }

    private void ClearSelection() {
        if (selectedSlotUI != null) {
            selectedSlotUI.SetSelected(false);
            selectedSlotUI = null;
        }
        UpdateActionButtons();
    }

    public void UseSelectedItem() {
        if (selectedSlotUI.Index >= 0) {
            UseItem(selectedSlotUI.Index);
        }
    }

    private void UseItem(int slotIndex) {
        var slot = inventory.GetSlot(slotIndex);
        if (slot == null || slot.IsEmpty) return;

        if (slot.Item is ConsumableItem consumable) {
            consumable.Consume();
            inventory.RemoveItem(slot.Item, 1);
        }
        // Додати логіку для екіпірування тощо
    }

    public void DropSelectedItem() {
        if (selectedSlotUI.Index >= 0) {
            var slot = inventory.GetSlot(selectedSlotUI.Index);
            if (slot != null && !slot.IsEmpty) {
                // Тут можна додати логіку створення предмета у світі
                inventory.RemoveItem(slot.Item, slot.Quantity);
                ClearSelection();
                detailPanel.Hide();
            }
        }
    }

    public void SortInventory() {
        // Проста реалізація сортування
        inventory.Slots.Sort((a, b) => {
            if (a.IsEmpty && b.IsEmpty) return 0;
            if (a.IsEmpty) return 1;
            if (b.IsEmpty) return -1;
            return string.Compare(a.Item.ItemName, b.Item.ItemName, StringComparison.Ordinal);
        });

        RefreshAllSlots();
    }

    private void OnDestroy() {
        if (inventory != null) {
            inventory.OnSlotUpdated -= OnSlotUpdated;
            inventory.OnInventoryChanged -= OnInventoryChanged;
        }

        foreach (var slotUI in slotUIs) {
            slotUI.OnClicked -= SelectSlot;
        }

        if (useButton != null)
            useButton.onClick.RemoveListener(UseSelectedItem);
        if (dropButton != null)
            dropButton.onClick.RemoveListener(DropSelectedItem);
        if (sortButton != null)
            sortButton.onClick.RemoveListener(SortInventory);
        if (closeButton != null)
            closeButton.onClick.RemoveListener(ToggleInventory);

    }
}

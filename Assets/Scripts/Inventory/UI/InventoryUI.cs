using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    [Header("Inventory Reference")]
    [SerializeField] private Inventory inventory;

    [Header("UI Components")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private ItemDetailPanel detailPanel;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private TextMeshProUGUI unidentifiedCountText;

    [Header("Action Buttons")]
    [SerializeField] private Button useButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button dropAllButton;
    [SerializeField] private Button identifyButton;

    [Header("Sort Buttons")]
    [SerializeField] private Button sortByNameButton;
    [SerializeField] private Button sortByRarityButton;
    [SerializeField] private Button sortByStatusButton;

    [Header("Special Buttons")]
    [SerializeField] private Button identifyAllButton;
    [SerializeField] private Button closeButton;

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.I;

    private List<InventorySlotUI> slotUIPool = new List<InventorySlotUI>();
    private int selectedIndex = -1;

    private void Start() {
        inventoryPanel.SetActive(false);
        detailPanel?.Toggle(false);

        if (inventory != null) {
            inventory.OnItemAdded += OnItemAdded;
            inventory.OnItemRemoved += OnItemRemoved;
            inventory.OnItemUpdated += OnItemUpdated;
            inventory.OnItemIdentified += OnItemIdentified;
            inventory.OnInventoryChanged += OnInventoryChanged;
        }


        SetupButtons();
        UpdateCountDisplays();
    }

    private void Update() {
        if (Input.GetKeyDown(toggleKey)) {
            ToggleInventory();
        }
    }

    private void SetupButtons() {
        useButton?.onClick.AddListener(UseSelectedItem);
        dropButton?.onClick.AddListener(DropSelectedItem);
        dropAllButton?.onClick.AddListener(DropAllSelectedItems);
        identifyButton?.onClick.AddListener(IdentifySelectedItem);

        sortByNameButton?.onClick.AddListener(() => inventory.SortByName());
        sortByRarityButton?.onClick.AddListener(() => inventory.SortByRarity());
        sortByStatusButton?.onClick.AddListener(() => inventory.SortByIdentificationStatus());

        identifyAllButton?.onClick.AddListener(() => inventory.IdentifyAllItems());
        closeButton?.onClick.AddListener(ToggleInventory);

        UpdateActionButtons();
    }

    private void OnItemAdded(InventorySlot slot) {
        RebuildUI();
        UpdateCountDisplays();
    }

    private void OnItemRemoved(InventorySlot slot) {
        RebuildUI();
        UpdateCountDisplays();

        if (selectedIndex >= inventory.Count) {
            ClearSelection();
        }
    }

    private void OnItemUpdated(InventorySlot slot) {
        int index = inventory.GetItemIndex(slot);
        if (index >= 0 && index < slotUIPool.Count) {
            slotUIPool[index].UpdateDisplay();
        }
    }

    private void OnItemIdentified(InventorySlot slot) {
        int index = inventory.GetItemIndex(slot);
        if (index >= 0 && index < slotUIPool.Count) {
            slotUIPool[index].UpdateDisplay();
        }

        // ������� detail panel ���� �� �������� item
        if (selectedIndex == index) {
            UpdateSelectionDisplay();
        }

        UpdateCountDisplays();
    }

    private void OnInventoryChanged() {
        RebuildUI();
        UpdateCountDisplays();
    }

    private void RebuildUI() {
        foreach (var slotUI in slotUIPool) {
            if (slotUI != null) {
                Destroy(slotUI.gameObject);
            }
        }
        slotUIPool.Clear();

        for (int i = 0; i < inventory.Count; i++) {
            CreateSlotUI(inventory.GetItemAt(i), i);
        }

        if (selectedIndex >= 0 && selectedIndex < slotUIPool.Count) {
            slotUIPool[selectedIndex].SetSelected(true);
        }
    }

    private void CreateSlotUI(InventorySlot slot, int index) {
        GameObject slotObj = Instantiate(itemSlotPrefab, itemContainer);
        InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();

        slotUI.Initialize(slot, index);
        slotUI.OnClicked += SelectItem;
        slotUI.OnRightClicked += QuickUseItem;

        slotUIPool.Add(slotUI);
    }

    private void SelectItem(int index) {
        if (index < 0 || index >= slotUIPool.Count) return;

        if (selectedIndex == index) {
            ClearSelection();
            return;
        }

        if (selectedIndex >= 0 && selectedIndex < slotUIPool.Count) {
            slotUIPool[selectedIndex].SetSelected(false);
        }

        selectedIndex = index;
        slotUIPool[index].SetSelected(true);

        UpdateSelectionDisplay();
        UpdateActionButtons();
    }

    private void UpdateSelectionDisplay() {
        var slot = inventory.GetItemAt(selectedIndex);
        if (slot != null) {
            detailPanel?.ShowItemDetails(slot);
        }
    }

    private void ClearSelection() {
        if (selectedIndex >= 0 && selectedIndex < slotUIPool.Count) {
            slotUIPool[selectedIndex].SetSelected(false);
        }

        selectedIndex = -1;
        detailPanel?.Toggle(false);
        UpdateActionButtons();
    }

    private void QuickUseItem(int index) {
        var slot = inventory.GetItemAt(index);
        if (slot == null || !slot.IsIdentified) return;

        if (slot.Item is ConsumableItem consumable) {
            consumable.Consume();
            inventory.RemoveItemAt(index, 1);
        }
    }

    private void UseSelectedItem() {
        if (selectedIndex < 0) return;

        var slot = inventory.GetItemAt(selectedIndex);
        if (slot == null || !slot.IsIdentified) return;

        if (slot.Item is ConsumableItem consumable) {
            consumable.Consume();
            inventory.RemoveItemAt(selectedIndex, 1);
        }
    }

    private void DropSelectedItem() {
        if (selectedIndex < 0) return;
        inventory.RemoveItemAt(selectedIndex, 1);
    }

    private void DropAllSelectedItems() {
        if (selectedIndex < 0) return;

        var slot = inventory.GetItemAt(selectedIndex);
        if (slot == null) return;

        inventory.RemoveItemAt(selectedIndex, slot.Quantity);
        ClearSelection();
    }

    private void IdentifySelectedItem() {
        if (selectedIndex < 0) return;

        bool identified = inventory.IdentifyItemAt(selectedIndex);
        if (identified) {
            Debug.Log("Item identified!");
        }
    }
    public void IdentifyItem(int Index)
    {
        if (Index < 0) return;

        bool identified = inventory.IdentifyItemAt(Index);
        if (identified) {
            Debug.Log("Item identified!");
        }
    }

    private void UpdateActionButtons()
    {
        bool hasSelection = selectedIndex >= 0 && selectedIndex < inventory.Count;

        if (hasSelection)
        {
            var slot = inventory.GetItemAt(selectedIndex);
            bool isIdentified = slot.IsIdentified;
            bool canUse = isIdentified && slot.Item is ConsumableItem;

            if (useButton != null) useButton.interactable = canUse;
            if (dropButton != null) dropButton.interactable = true;
            if (dropAllButton != null) dropAllButton.interactable = slot.Quantity > 1;
            if (identifyButton != null) identifyButton.interactable = !isIdentified;
        }
        else
        {
            if (useButton != null) useButton.interactable = false;
            if (dropButton != null) dropButton.interactable = false;
            if (dropAllButton != null) dropAllButton.interactable = false;
            if (identifyButton != null) identifyButton.interactable = false;
        }

        // Identify All button
        if (identifyAllButton != null)
        {
            identifyAllButton.interactable = inventory.GetUnidentifiedCount() > 0;
        }
    }

    private void UpdateCountDisplays() {
        if (itemCountText != null) {
            itemCountText.text = $"Items: {inventory.Count}/{inventory.MaxCapacity}";
        }

        if (unidentifiedCountText != null) {
            int unidentifiedCount = inventory.GetUnidentifiedCount();
            unidentifiedCountText.text = $"Unidentified: {unidentifiedCount}";
            unidentifiedCountText.gameObject.SetActive(unidentifiedCount > 0);
        }
    }

    public void ToggleInventory() {
        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);

        if (isActive) {
            RebuildUI();
        } else {
            ClearSelection();
        }
    }

    private void OnDestroy() {
        if (inventory != null) {
            inventory.OnItemAdded -= OnItemAdded;
            inventory.OnItemRemoved -= OnItemRemoved;
            inventory.OnItemUpdated -= OnItemUpdated;
            inventory.OnItemIdentified -= OnItemIdentified;
            inventory.OnInventoryChanged -= OnInventoryChanged;
        }
    }
}
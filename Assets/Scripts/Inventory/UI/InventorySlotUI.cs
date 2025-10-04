using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler {
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image selectedBorder;
    [SerializeField] private Image rarityBorder;

    public event Action<InventorySlotUI, int> OnClicked;

    public int Index { get; private set; }

    public void UpdateSlot(InventorySlot slot) {
        if (slot == null || slot.IsEmpty) {
            itemIcon.enabled = false;
            quantityText.text = "";
            if (rarityBorder != null) rarityBorder.enabled = false;
            return;
        }

        UpdateItemSprite(slot.Item.ItemIcon);
        UpdateItemName(slot.Item.ItemName);


        string resultText = string.Empty;
        if (slot.Item.IsStackable && slot.Quantity > 1) {
            resultText = slot.Quantity.ToString();
        }

        UpdateItemQuantity(resultText);

        UpdateRarity(RarityUtility.GetRarityColor(slot.Item.Rarity));
    }

    public void UpdateItemName(string itemName) {
        if (itemNameText == null) return;

        itemNameText.text = itemName;
    }

    public void UpdateItemSprite(Sprite newSprite) {
        if (itemIcon == null) return;

        itemIcon.enabled = true;
        itemIcon.sprite = newSprite;
    }

    public void UpdateItemQuantity(string quantity) {
        if (quantityText == null) return;

        quantityText.text = quantity;
    }

    public void UpdateRarity(Color rarity) {
        if (rarityBorder == null) return;

        rarityBorder.enabled = true;
        rarityBorder.color = rarity;
    }

    public void SetSelected(bool selected) {
        if (selectedBorder == null) return;

        selectedBorder.enabled = selected;
    }

    public void OnPointerClick(PointerEventData eventData) {
        OnClicked?.Invoke(this, Index);
    }

    public void SetIndex(int i) {
        Index = i;
    }
}

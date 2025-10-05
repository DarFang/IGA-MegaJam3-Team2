using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPanel : MonoBehaviour {
    [SerializeField] private GameObject panel;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject unknownWarning;

    public void ShowItemDetails(InventorySlot slot) {
        if (slot == null) {
            Toggle(false);
            return;
        }

        Toggle(true);


        bool isIdentified = slot.IsIdentified;

        if (itemIcon != null) {
            itemIcon.sprite = slot.GetDisplayIcon();
            itemIcon.color = isIdentified ? Color.white : new Color(0.5f, 0.5f, 0.5f);
        }

        if (itemNameText != null) {
            itemNameText.text = slot.GetDisplayName();
        }

        if (rarityText != null) {
            rarityText.text = slot.GetDisplayRarity().ToString();
            rarityText.color = RarityUtility.GetRarityColor(slot.GetDisplayRarity());
        }

        if (descriptionText != null) {
            descriptionText.text = slot.GetDisplayDescription();
        }

        if (unknownWarning != null) {
            unknownWarning.SetActive(!isIdentified);
        }
    }

    public void Toggle(bool isEnabled) {
        if (panel == null) return;
        panel.SetActive(isEnabled);
    }
}
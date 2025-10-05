using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image selectionHighlight;
    [SerializeField] private Image rarityBorder;
    [SerializeField] private Image hoverHighlight;
    [SerializeField] private GameObject unknownMarker; // "?" icon

    [Header("Colors")]
    [SerializeField] private Color unknownTextColor = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] private Color identifiedTextColor = Color.white;

    [Header("Animation")]
    [SerializeField] private float scaleOnHover = 1.05f;

    private InventorySlot slot;
    private int index;
    private Vector3 originalScale;

    public event Action<int> OnClicked;
    public event Action<int> OnRightClicked;

    public int Index => index;
    public InventorySlot Slot => slot;

    private void Awake() {
        originalScale = transform.localScale;

        if (selectionHighlight != null) {
            selectionHighlight.gameObject.SetActive(false);
        }

        if (hoverHighlight != null) {
            hoverHighlight.gameObject.SetActive(false);
        }

        if (unknownMarker != null) {
            unknownMarker.SetActive(false);
        }
    }

    public void Initialize(InventorySlot slot, int index) {
        this.slot = slot;
        this.index = index;
        UpdateDisplay();
    }

    public void UpdateDisplay() {
        if (slot == null || slot.Item == null) return;

        bool isIdentified = slot.IsIdentified;

        // Icon
        if (itemIcon != null) {
            itemIcon.sprite = slot.GetDisplayIcon();
            itemIcon.color = isIdentified ? Color.white : new Color(0.5f, 0.5f, 0.5f);
        }

        // Name
        if (itemNameText != null) {
            itemNameText.text = slot.GetDisplayName();
            itemNameText.color = isIdentified ? identifiedTextColor : unknownTextColor;

            // Додати italic для невідомих
            if (!isIdentified) {
                itemNameText.fontStyle = FontStyles.Italic;
            } else {
                itemNameText.fontStyle = FontStyles.Normal;
            }
        }

        // Quantity
        if (quantityText != null) {
            if (slot.Item.IsStackable && slot.Quantity > 1) {
                quantityText.gameObject.SetActive(true);
                quantityText.text = $"x{slot.Quantity}";
            } else {
                quantityText.gameObject.SetActive(false);
            }
        }

        // Rarity border
        if (rarityBorder != null) {
            Color rarityColor = RarityUtility.GetRarityColor(slot.GetDisplayRarity());
            rarityBorder.color = isIdentified ? rarityColor : Color.gray;
        }

        // Unknown marker
        if (unknownMarker != null) {
            unknownMarker.SetActive(!isIdentified);
        }
    }

    public void SetSelected(bool selected) {
        if (selectionHighlight != null) {
            selectionHighlight.gameObject.SetActive(selected);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            OnClicked?.Invoke(index);
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            OnRightClicked?.Invoke(index);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (hoverHighlight != null) {
            hoverHighlight.gameObject.SetActive(true);
        }

        transform.localScale = originalScale * scaleOnHover;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (hoverHighlight != null) {
            hoverHighlight.gameObject.SetActive(false);
        }

        transform.localScale = originalScale;
    }
}


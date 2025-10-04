using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPanel : MonoBehaviour {
    [Header("Basic Info")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image itemIconLarge;
    [SerializeField] private TextMeshProUGUI itemLoreText;

    [Header("Ability Section")]
    [SerializeField] private GameObject abilitySection;
    [SerializeField] private TextMeshProUGUI abilityNameText;
    [SerializeField] private Image abilityIconImage;
    [SerializeField] private TextMeshProUGUI abilityDescriptionText;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private TextMeshProUGUI manaCostText;

    [Header("Animation")]
    [SerializeField] private Animator abilityAnimator; // Для GIF-анімації

    public void ShowItemDetails(InventoryItem item) {
        if (item == null) {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        // Basic info
        UpdateName(item.ItemName);
        UpdateItemSprite(item.ItemIcon);
        UpdateDescription(item.ItemLore);

        // Ability section
        if (abilitySection == null) return;

        if (item.HasAbility && item.Ability != null && item.Ability.HasAbility) {
            abilitySection.SetActive(true);
            abilityNameText.text = item.Ability.AbilityName;
            abilityIconImage.sprite = item.Ability.AbilityIcon;
            abilityDescriptionText.text = item.Ability.AbilityDescription;
            cooldownText.text = $"Cooldown: {item.Ability.Cooldown}s";
            manaCostText.text = $"Mana: {item.Ability.ManaCost}";

            // Якщо є аніматор, можна використати RuntimeAnimatorController для GIF
            if (abilityAnimator != null) {
                // Тут можна встановити анімацію здібності
            }
        } else {
            abilitySection.SetActive(false);
        }
    }

    public void UpdateDescription(string description) {
        if (itemLoreText == null) return;
        itemLoreText.text = description;
    }

    public void UpdateName(string name) {
        if (itemNameText == null) return;
        itemNameText.text = name;
    }

    public void UpdateItemSprite(Sprite newSprite) {
        if (itemIconLarge == null) return;
        itemIconLarge.sprite = newSprite;
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}

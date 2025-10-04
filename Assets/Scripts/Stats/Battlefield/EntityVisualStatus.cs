using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EntityVisualStatus : MonoBehaviour {
    // UI-Компоненти
    [Header("Health UI")]
    [SerializeField] private Image healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Graphic[] componentsToFadeOnDeath;

    [Header("Text Fields")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI actionText; // Новий текст для дій

    [Header("Action Text Animation")]
    [SerializeField] private float actionTextFadeDuration = 1.5f;
    [SerializeField] private float actionTextDisplayTime = 2f;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color healColor = Color.green;
    [SerializeField] private Color buffColor = Color.yellow;

    [Header("Formatting")]
    [SerializeField] private string defaultDefenseText = "Defence: ";
    [SerializeField] private string defaultHealthText = "HP: ";
    [SerializeField] private Color aliveColor = Color.white;
    [SerializeField] private Color deadColor = Color.grey;

    private DG.Tweening.Sequence currentActionSequence;
    private Tween currentFadeTween;

    public void UpdateHealth(float percentage, string displayText) {
        if (healthSlider != null) {
            healthSlider.fillAmount = Mathf.Clamp01(percentage);
        }
        if (healthText != null) {
            healthText.text = defaultHealthText + displayText;
        }
    }

    public void UpdateName(string characterName) {
        if (characterNameText != null) {
            characterNameText.text = characterName;
        }
    }

    public void UpdateDefense(string defenseValueText) {
        if (defenseText != null) {
            defenseText.text = defaultDefenseText + defenseValueText;
        }
    }

    public void SetActiveState(bool isAlive) {
        Color targetColor = isAlive ? aliveColor : deadColor;

        // Зміна кольору для візуального ефекту "смерті"
        foreach (var graphic in componentsToFadeOnDeath) {
            if (graphic != null) {
                graphic.color = targetColor;
            }
        }
    }

    public void UpdateStats(float currentHealth, float maxHealth, float defenseValue) {
        // 1. Здоров'я
        float percentage = currentHealth / maxHealth;
        string healthStr = $"{currentHealth:F0} / {maxHealth:F0}";
        UpdateHealth(percentage, healthStr);

        // 2. Захист
        // Форматування та округлення відбувається тут (або краще - у Presenter)
        float trimmedDefenseValue = Mathf.Round(defenseValue * 100f) / 100f;
        UpdateDefense(trimmedDefenseValue.ToString("F1")); // "F1" для відображення одного знака

        // 3. Стан
        SetActiveState(currentHealth > 0);
    }
    public void ShowAction(string actionMessage, Color textColor) {
        // Скасовуємо попередню анімацію якщо вона є
        currentActionSequence?.Kill();
        currentFadeTween?.Kill();

        if (actionText == null) return;

        // Встановлюємо текст та колір
        actionText.text = actionMessage;
        actionText.color = textColor.WithAlpha(1f); // Повна непрозорість
        actionText.gameObject.SetActive(true);

        // Створюємо послідовність анімацій
        currentActionSequence = DOTween.Sequence();

        // Спочатку невелике збільшення (ефект появи)
        currentActionSequence.Append(actionText.transform.DOScale(1.2f, 0.2f));
        currentActionSequence.Append(actionText.transform.DOScale(1f, 0.2f));

        // Чекаємо певний час
        currentActionSequence.AppendInterval(actionTextDisplayTime);

        // Плавне затухання
        currentActionSequence.Append(actionText.DOFade(0f, actionTextFadeDuration));

        // В кінці вимикаємо об'єкт
        currentActionSequence.OnComplete(() => {
            actionText.gameObject.SetActive(false);
            actionText.color = textColor.WithAlpha(1f); // Відновлюємо колір
        });
    }

    public void ShowDealDamage(float damage) {
        string message = $"Attack! {damage:F1}";
        ShowAction(message, damageColor);
    }

    public void ShowHeal(float amount) {
        string message = $"Heal! +{amount:F1}";
        ShowAction(message, healColor);
    }

    public void ShowDefenseBuff(float amount) {
        string message = $"Defense! +{amount:F1}";
        ShowAction(message, buffColor);
    }

    public void ShowCustomAction(string action, Color color) {
        ShowAction(action, color);
    }
}

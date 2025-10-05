using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EntityVisualStatus : MonoBehaviour {
    // UI-����������
    [Header("Health UI")]
    [SerializeField] private Image healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Graphic[] componentsToFadeOnDeath;

    [Header("Mana UI")]
    [SerializeField] private Image manaSlider;
    [SerializeField] private TextMeshProUGUI manaText;

    [Header("Text Fields")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI actionText; // ����� ����� ��� ��

    [Header("Action Text Animation")]
    [SerializeField] private float actionTextFadeDuration = 1.5f;
    [SerializeField] private float actionTextDisplayTime = 2f;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color healColor = Color.green;
    [SerializeField] private Color buffColor = Color.yellow;

    [Header("Formatting")]
    [SerializeField] private string defaultDefenseText = "Defence: ";
    [SerializeField] private string defaultHealthText = "HP: ";
    [SerializeField] private string defaultManaText = "MP: ";
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
    public void UpdateMana(float percentage, string displayText) {
        Debug.Log($"[EntityVisualStatus] Updating Mana: {displayText} ({percentage:P1})");
        if (manaSlider != null) {
            manaSlider.fillAmount = Mathf.Clamp01(percentage);
            Debug.Log(manaSlider.fillAmount);
        }

        if (manaText != null) {
            manaText.text = defaultManaText + displayText;
            Debug.Log(manaText.text);
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

        // ���� ������� ��� ���������� ������ "�����"
        foreach (var graphic in componentsToFadeOnDeath) {
            if (graphic != null) {
                graphic.color = targetColor;
            }
        }
    }

    public void UpdateStats(float currentHealth, float maxHealth, float defenseValue, float currentMana, float maxMana) {
        // 1. ������'�
        float percentage = currentHealth / maxHealth;
        string healthStr = $"{currentHealth:F0} / {maxHealth:F0}";
        UpdateHealth(percentage, healthStr);

        // 2. ������
        // ������������ �� ���������� ���������� ��� (��� ����� - � Presenter)
        float trimmedDefenseValue = Mathf.Round(defenseValue * 100f) / 100f;
        UpdateDefense(trimmedDefenseValue.ToString("F1")); // "F1" ��� ����������� ������ �����

        // 3. ����
        SetActiveState(currentHealth > 0);
        float manaPercentage = currentMana / maxMana;
        UpdateMana(manaPercentage, currentMana.ToString("F0"));
    }
    public void ShowAction(string actionMessage, Color textColor) {
        // ��������� ��������� �������� ���� ���� �
        currentActionSequence?.Kill();
        currentFadeTween?.Kill();

        if (actionText == null) return;

        textColor.a = 1.0f;

        // ������������ ����� �� ����
        actionText.text = actionMessage;
        actionText.color = textColor; 
        actionText.gameObject.SetActive(true);

        currentActionSequence = DOTween.Sequence();

        currentActionSequence.Append(actionText.transform.DOScale(1.2f, 0.2f));
        currentActionSequence.Append(actionText.transform.DOScale(1f, 0.2f));

        currentActionSequence.AppendInterval(actionTextDisplayTime);

        currentActionSequence.Append(actionText.DOFade(0f, actionTextFadeDuration));

        currentActionSequence.OnComplete(() => {
            actionText.gameObject.SetActive(false);
            actionText.color = textColor;
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

    public void ShowManaBuff(float amount) {
        string message = $"Mana! +{amount:F1}";
        ShowAction(message, buffColor);
    }

    public void ShowCustomAction(string action, Color color) {
        ShowAction(action, color);
    }

    public void OnDestroy() {
        transform.DOKill();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityView : MonoBehaviour {
    [SerializeField] Image healthSlider;
    [SerializeField] TextMeshProUGUI characterNameTaxt;
    [SerializeField] TextMeshProUGUI defenseText;

    public void UpdateHealth(float currentValue, float maxValue) {
        float percentage = currentValue / maxValue;

        healthSlider.fillAmount = percentage;
    }

    public void UpdateName(string characterName) {
        characterNameTaxt.text = characterName;
    }

    public void UpdateDefense(float defenseValue) {
        float trimmedDefenseValue = Mathf.Round(defenseValue * 100f) / 100f;
        defenseText.text = trimmedDefenseValue.ToString();
    }
}

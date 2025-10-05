using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneralCombatUI : MonoBehaviour
{
    public TextMeshProUGUI AttackTextAction;
    public TextMeshProUGUI DefendTextAction;
    public TextMeshProUGUI HealTextAction;
    public TextMeshProUGUI ManaTextAction;

    public void UpdateButtonText(UpgradedAbilities upgradedAbilities) {
        AttackTextAction.text = upgradedAbilities.AttackUpgraded ? "Attack (Upgraded)" : "Attack";
        DefendTextAction.text = upgradedAbilities.DefenseUpgraded ? "Defend (Upgraded)" : "Defend";
        HealTextAction.text = upgradedAbilities.HealUpgraded ? "Heal (Upgraded)" : "Heal";
        ManaTextAction.text = upgradedAbilities.ManaUpgraded ? "Mana (Upgraded)" : "Mana";
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GeneralCombatUI : MonoBehaviour
{
    public TextMeshProUGUI AttackTextAction;
    public TextMeshProUGUI DefendTextAction;
    public TextMeshProUGUI HealTextAction;
    public TextMeshProUGUI ManaTextAction;
    String willDrainText = "\n+10 Will";
    String useWillText = "\n-10 Will";

    public void UpdateButtonText(UpgradedAbilities upgradedAbilities)
    {
        AttackTextAction.text = upgradedAbilities.AttackUpgraded ? "Attack+" + useWillText : "Attack" + useWillText;
        DefendTextAction.text = upgradedAbilities.DefenseUpgraded ? "Defend+" + useWillText : "Defend" + useWillText;
        HealTextAction.text = upgradedAbilities.HealUpgraded ? "Heal+" + useWillText: "Heal" + useWillText;
        ManaTextAction.text = upgradedAbilities.ManaUpgraded ? "Will Drain+\n+15 Will" : "Will Drain" + willDrainText;
    }
}

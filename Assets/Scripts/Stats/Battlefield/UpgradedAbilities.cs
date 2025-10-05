using UnityEngine;

[CreateAssetMenu(fileName = "UpgradedAbilities", menuName = "UpgradedAbilities", order = 2)]
public class UpgradedAbilities : ScriptableObject {
    public bool AttackUpgraded;
    public bool DefenseUpgraded;
    public bool HealUpgraded;
    public bool ManaUpgraded;
}

using UnityEngine;

[CreateAssetMenu(fileName = "UpgradedAbilities", menuName = "UpgradedAbilities", order = 2)]
public class UpgradedAbilities : ScriptableObject {
    public bool AttackUpgraded;
    public bool DefenseUpgraded;
    public bool HealUpgraded;
    public bool ManaUpgraded;
    public int AttackManaCost = 10;
    public int DefenseManaCost = 10;
    public int HealManaCost = 10;
    public int ManaManaCost = 10;
}

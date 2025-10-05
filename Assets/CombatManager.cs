using UnityEngine;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
    public GameObject Panel;
    public List<BattleFieldManager> BattlefieldManagers;
    public List<Encounter> Encounters;
    public GeneralCombatUI GeneralCombatUI;
    private void Start() {
        DisableAllBattlefieldManagers();
    }

    public void DisableAllBattlefieldManagers() {
        foreach (var battlefieldManager in BattlefieldManagers) {
            battlefieldManager.gameObject.SetActive(false);
        }
        Panel.SetActive(false);
    }

    public void EnableBattlefieldManager(int index) {
        BattlefieldManagers[index-1].gameObject.SetActive(true);
        Panel.SetActive(true);
        GeneralCombatUI.UpdateButtonText(Encounters[index-1].upgradedAbilities);
    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }
    public GameObject Panel;
    public List<BattleFieldManager> BattlefieldManagers;
    public List<Encounter> Encounters;
    public GeneralCombatUI GeneralCombatUI;
    public ManaManager ManaManager;
    public SlicedFilledImage manaPercentageImage;
    public TextMeshProUGUI manaText;
    private void Start() {
        DisableAllBattlefieldManagers();
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
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
        if(ManaManager != null)
        {
            ManaManager.OnMaxManaChanged.RemoveAllListeners();
        }
        StartCoroutine(DoAfterFrame());

        IEnumerator DoAfterFrame()
        {
            yield return new WaitForEndOfFrame();
            ManaManager.Initialize(Encounters[index-1].Player, Encounters[index-1].EnemyReference);
        }
    }
    public void UpdateMana(int newMana)
    {
        Debug.Log("Updating Mana in CombatManager: " + newMana);
        manaPercentageImage.fillAmount = (float)newMana / (float)ManaManager.MaxMana;
        manaText.text = newMana.ToString();
    }
}
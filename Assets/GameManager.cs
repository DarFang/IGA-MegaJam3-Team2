using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool isInCutScene = false;
    public bool IsInCutScene { get { return isInCutScene; } set { isInCutScene = value; } }
    private bool isInBattle = false;
    public bool IsInBattle { get { return isInBattle; } set { isInBattle = value; } }
    public InventoryUI InventoryUI;
    public GameObject ActionsPanel;
    void Start()
    {
        Instance = this;
    }
    public void SetIsInCutScene(bool value)
    {
        isInCutScene = value;
        if (value == true)
        {
            TriggerPlayerAction(false);
            TurnOffInventory();
        }
    }
    public void SetIsInBattle(bool value)
    {
        isInBattle = value;
        if (value == true)
        {
            TriggerPlayerAction(false);
            TurnOffInventory();
        }
    }
    public void TriggerPlayerAction(bool isInAction)
    {
        ActionsPanel.SetActive(isInAction);
    }
    public void TurnOffInventory()
    {
        InventoryUI.CloseInventory();
    }
    public void GameOver()
    {
        Destroy(DialogueManager.Instance.gameObject);
        SceneController.Instance.StartSceneSwitch(SceneType.Level);
        SoundManager.Instance.ChangeFromBeachToIndoor();
        MusicManager.Instance.ChangeFromCombatToCutscene();
        MusicManager.Instance.SetEnemiesDefeated(0);

    }
    public void EndGame()
    {
        SceneController.Instance.StartSceneSwitch(SceneType.Credits);
        SoundManager.Instance.ChangeAmbienceFromIndoorToBeach();
        MusicManager.Instance.SetEnemiesDefeated(5);
    }
}

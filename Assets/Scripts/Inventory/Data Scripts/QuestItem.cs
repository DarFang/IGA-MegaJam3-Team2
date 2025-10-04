using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestItem", menuName = "Inventory/Quest Item")]
public class QuestItem : InventoryItem {
    [Header("Quest Properties")]
    [SerializeField] private string questID;
    [SerializeField] private bool isQuestComplete;

    public string QuestID => questID;
    public bool IsQuestComplete => isQuestComplete;
}
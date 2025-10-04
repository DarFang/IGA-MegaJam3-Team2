using UnityEngine;

public class Encounter : MonoBehaviour {
    [SerializeField] Player player;
    [SerializeField] Entity entity;
    [SerializeField] BattleFieldManager fieldManager;
    void Start()
    {
        if (entity == null || player == null) return;
        fieldManager.StartBattle(player, entity);
    }
}

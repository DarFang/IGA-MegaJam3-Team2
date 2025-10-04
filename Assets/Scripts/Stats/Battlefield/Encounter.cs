using UnityEngine;

public class Encounter : MonoBehaviour {
    [SerializeField] Player player;
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] BattleFieldManager fieldManager;
    [SerializeField] private RectTransform encounterSpawnPoint;
    void Start()
    {
        Enemy enemy = Instantiate(enemyPrefab);
        
        if (enemy == null || player == null) return;

        enemy.transform.SetParent(encounterSpawnPoint);
        enemy.transform.position = encounterSpawnPoint.transform.position;

        fieldManager.StartBattle(player, enemy);
    }
}

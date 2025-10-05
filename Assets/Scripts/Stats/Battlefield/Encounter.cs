using UnityEngine;

public class Encounter : MonoBehaviour {
    [SerializeField] Player player;
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] BattleFieldManager fieldManager;
    [SerializeField] private RectTransform encounterSpawnPoint;
    [SerializeField] public UpgradedAbilities upgradedAbilities;
    public Player Player => player;
    public Enemy EnemyReference { get; private set;}
    void Start()
    {
        EnemyReference = Instantiate(enemyPrefab);
        
        if (EnemyReference == null || player == null) return;

        EnemyReference.transform.SetParent(encounterSpawnPoint);
        EnemyReference.transform.position = encounterSpawnPoint.transform.position;

        fieldManager.StartBattle(player, EnemyReference, upgradedAbilities);
    }
}

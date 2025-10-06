using System;
using UnityEngine;

public class Encounter : MonoBehaviour {

    public event Action<Enemy> OnEnemyCreated;

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

        OnEnemyCreated?.Invoke(EnemyReference);

        EnemyReference.transform.SetParent(encounterSpawnPoint);
        EnemyReference.transform.position = encounterSpawnPoint.transform.position;
        EnemyReference.transform.localScale = Vector3.one;
        EnemyReference.transform.localRotation = Quaternion.identity;

        fieldManager.StartBattle(player, EnemyReference, upgradedAbilities);
    }
}

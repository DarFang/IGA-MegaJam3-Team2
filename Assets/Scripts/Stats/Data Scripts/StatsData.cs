using UnityEngine;

[CreateAssetMenu(fileName = "EntityStats", menuName = "Stats/EntityStats")]
public class StatsData : ScriptableObject {

    [SerializeField] private RegenerableStatData healthData;
    [SerializeField] private StatData attackData;
    [SerializeField] private StatData defenseData;
    [SerializeField] private RegenerableStatData manaData; 
    [SerializeField] private StatData speedData;

    public RegenerableStatData HealthData => healthData;
    public RegenerableStatData ManaData => manaData;
    public StatData AttackData => attackData;
    public StatData DefenseData => defenseData;
    public StatData SpeedData => speedData;
}

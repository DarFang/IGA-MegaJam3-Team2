using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {
    [SerializeField] StatsData statsData;
    public Stats stats;

    public void Awake() {
        if (statsData == null) {
            Debug.Log($"Stats Data not set for : {this}");
            return;
        }

        stats.Initialze(statsData);
    }
}

public class Stats : MonoBehaviour
{
    public void Initialze(StatsData statsData) {
        

    }
}


[CreateAssetMenu(fileName = "EntityStats", menuName = "Stats/EntityStats")]
public class StatsData : ScriptableObject {
    public int Heath;
    public int Attack;
    public int Defense;
    public int Mana;
    public int Speed;
}

public class Stat {
    int maxValue;
    int CurrentValue;
}

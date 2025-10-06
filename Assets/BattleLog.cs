using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;
public class BattleLog : MonoBehaviour
{
    private List<String> logs = new List<String>();
    [SerializeField] List<BattleFieldManager> battleFieldManagers = new List<BattleFieldManager>();
    [SerializeField] TextMeshProUGUI LogText;

    private void Start()
    {
        foreach (var manager in battleFieldManagers)
        {
            manager.OnActionLogged += LogAction;
        }
    }
    public void LogAction(string action)
    {
        logs.Add(action);
        string combinedLogs = string.Join("\n", logs.TakeLast(10));
        LogText.text = combinedLogs;
    }
}

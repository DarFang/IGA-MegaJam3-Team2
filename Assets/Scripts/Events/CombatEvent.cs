using UnityEngine;
using UnityEngine.Events;

// TODO: Implement combat events logic
public class CombatEvent : Event
{
    [field: SerializeField] public override UnityEvent OnEventStart { get; set; }
    [field: SerializeField] public override UnityEvent OnEventEnd { get; set; }

    public override void StartEvent()
    {
        Debug.Log("Started combat event!");
        Debug.Log("Press (E) to start the combat");
        MusicManager.Instance.ChangeFromCutsceneToCombat();
        OnEventStart?.Invoke();
    }

    public override void EndEvent()
    {
        Debug.Log("Ended combat event");
        MusicManager.Instance.ChangeFromCombatToCutscene();
        OnEventEnd?.Invoke();
    }

    public override string GetType() => "Combat event";
}

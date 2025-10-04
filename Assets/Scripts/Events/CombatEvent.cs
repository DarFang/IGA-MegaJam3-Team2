using UnityEngine;
using UnityEngine.Events;

public class CombatEvent : Event
{
    [field: SerializeField] public override UnityEvent OnEventStart { get; set; }
    [field: SerializeField] public override UnityEvent OnEventEnd { get; set; }

    // TODO: Implement combat events logic
    public override void StartEvent()
    {
        Debug.Log("Started combat event!");
        Debug.Log("Press (E) to start the combat");
        OnEventStart?.Invoke();
    }

    public override void EndEvent()
    {
        Debug.Log("Ended combat event");
        OnEventEnd?.Invoke();
    }

    public override string GetType() => "Combat event";
}

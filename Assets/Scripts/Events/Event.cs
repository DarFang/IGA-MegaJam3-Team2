using UnityEngine;
using UnityEngine.Events;

public abstract class Event : MonoBehaviour
{
    public abstract UnityEvent OnEventStart { get; set; }
    public abstract UnityEvent OnEventEnd { get; set; }
    public abstract void StartEvent();
    public abstract void EndEvent();
    public abstract new string GetType();
}
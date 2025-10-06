using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents an abstract base class for events, providing a structure for handling event operations.
/// </summary>
public abstract class Event : MonoBehaviour
{
    public abstract UnityEvent OnEventStart { get; set; }
    public abstract UnityEvent OnEventEnd { get; set; }
    public abstract void StartEvent();
    public abstract void EndEvent();
    public abstract new string GetType();
    [SerializeField] public bool SkippEvent = false;
}
using System;
using UnityEngine;
using UnityEngine.Events;

public class Waypoint : MonoBehaviour
{
    /// <summary>
    /// The order of this waypoint in the path.
    /// </summary>
    [field: SerializeField] public int Order { get; private set; }
    
    /// <summary>
    /// The event to trigger when the player reaches this waypoint.
    /// </summary>
    [field: SerializeField] public Event Event { get; private set; }
}

using System;
using UnityEngine;
using UnityEngine.Events;

public class Waypoint : MonoBehaviour
{
    [field: SerializeField] public int Order { get; private set; }
    [field: SerializeField] public Event Event { get; private set; }

    /*private void Start() => Event.OnEventEnd.AddListener(DisableVisual);
    private void OnDisable() => Event.OnEventEnd.RemoveListener(DisableVisual);
    private void DisableVisual()
    {
        Debug.Log($"Disabling {name}'s gameObject...");
        GetComponentInChildren<MeshRenderer>().gameObject.SetActive(false);
    }*/
}

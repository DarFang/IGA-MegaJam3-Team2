using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class CameraTransitionsController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _thirdPersonCamera;
    [SerializeField] private CinemachineCamera _eventCamera;

    private Event[] _events;

    private void Awake()
    {
        _events = GetEvents();
        SetInitialCMCameraPriorities();
    }
    private void Start() => ListenToEvents();
    private void OnDisable() => StopListeningToEvents();

    private Event[] GetEvents()
    {
        Waypoint[] waypoints = FindObjectsByType<Waypoint>(FindObjectsSortMode.None);
        return waypoints.Select(wp => wp.Event).Where(ev => ev != null).ToArray();
    }

    private void ListenToEvents()
    {
        foreach(Event @event in _events)
        {
            if(@event == null)
                continue;

            @event.OnEventStart.AddListener(SwitchToEventCamera);
            @event.OnEventEnd.AddListener(SwitchToThirdPersonCamera);
        }
    }
    private void StopListeningToEvents()
    {
        foreach(Event @event in _events)
        {
            if(@event == null)
                continue;

            @event.OnEventStart.RemoveListener(SwitchToEventCamera);
            @event.OnEventEnd.RemoveListener(SwitchToThirdPersonCamera);
        }
    }

    private void SetInitialCMCameraPriorities()
    {
        _thirdPersonCamera.Priority = 10;
        _eventCamera.Priority = 0;
    }

    public void SwitchToEventCamera()
    {
        _thirdPersonCamera.Priority = 0;
        _eventCamera.Priority = 10;
    }
    public void SwitchToThirdPersonCamera()
    {
        _thirdPersonCamera.Priority = 10;
        _eventCamera.Priority = 0;
    }
}

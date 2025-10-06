using System;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    public Transform Target { get; set; }

    // Update is called once per frame
    void LateUpdate() => transform.LookAt(Target);
}

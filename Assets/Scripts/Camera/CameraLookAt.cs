using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    [SerializeField] private Transform _target;
    // Update is called once per frame
    void LateUpdate() => transform.LookAt(_target);
}

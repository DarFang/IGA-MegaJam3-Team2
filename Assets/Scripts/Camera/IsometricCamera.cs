using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _player;
    [Header("Settings")]
    [Tooltip("Height of the camera above the player")]
    [SerializeField] private float _height = 7.5f;
    [Tooltip("Camera offset from the player on the X and Z axes")]
    [SerializeField] private float _offset = 5.0f;
    [Tooltip("Camera angle")]
    [SerializeField] private Vector2 _rotation = new(45, -45);

    private void LateUpdate()
    {
        if(_player != null)
        {
            Vector3 newPosition = _player.position;
            newPosition.y += _height; // Height above the player
            newPosition.z -= _offset;
            newPosition.x += 5;
            transform.SetPositionAndRotation(newPosition , Quaternion.Euler(_rotation));
        }
        else
        {
            Debug.LogError("Player transform not assigned in Inspector.");
        }
    }
}

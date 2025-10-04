using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    #region Editor Exposed Fields

    [Header("References")]
    [SerializeField] private Transform _player;
    [Header("Settings")]
    [Tooltip("Height of the camera above the player")]
    [SerializeField] private float _height = 7.5f;
    [Tooltip("Camera offset from the player on the Z axis")]
    [SerializeField] private float _offset = 1.0f;
    [Tooltip("Camera angle")]
    [SerializeField] private Vector2 _rotation = new(45, -45);

    #endregion

    private const float _xOffset = 5.0f;

    private void LateUpdate()
    {
        if(_player != null)
            FollowPlayerIsometric();
        else
            Debug.LogError("Player transform not assigned in editor", this);
    }

    /// <summary>
    /// Follow the player in an isometric view.
    /// </summary>
    private void FollowPlayerIsometric()
    {
        Vector3 newPosition = _player.position;
        newPosition.y += _height;
        newPosition.z -= _offset;
        newPosition.x += _xOffset;
        transform.SetPositionAndRotation(newPosition , Quaternion.Euler(_rotation));
    }
}

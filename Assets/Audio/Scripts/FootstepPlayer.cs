using UnityEngine;

public class FootstepPlayer : MonoBehaviour
{
    public Sound footstepSound;

    public void PlayFootstep()
    {
        SoundManager.Instance.CreateSound().SetParent(gameObject).Play(footstepSound);
    }
}

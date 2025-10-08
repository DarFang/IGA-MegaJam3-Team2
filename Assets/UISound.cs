using UnityEngine;

public class UISound : MonoBehaviour
{
    public SoundList Sounds;
    public string soundName = "Select";

    public void PlaySound()
    {
        SoundManager.Instance.CreateSound().AutoDuckMusic().Play(Sounds.GetSound(soundName));
    }
}

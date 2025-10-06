using UnityEngine;

public class TestingActionSounds : MonoBehaviour
{
    public SoundList Sounds;

    public void PlaySound(string soundName)
    {
        //print("soundName");
        SoundManager.Instance.CreateSound().AutoDuckMusic().Play(Sounds.GetSound(soundName));
    }

    private void OnEnable()
    {
        SoundManager.Instance.CreateSound().AutoDuckMusic().Play(Sounds.GetSound("OpenBook"));

    }

    private void OnDisable()
    {
        SoundManager.Instance.CreateSound().AutoDuckMusic().Play(Sounds.GetSound("OpenBook"));

    }

}

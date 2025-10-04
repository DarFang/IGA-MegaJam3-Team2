using UnityEngine;

public class SoundList : MonoBehaviour
{
    
    [SerializeField] private Sound[] soundsOnObject;

    public Sound GetSound(int soundID)
    {
        return soundsOnObject[soundID];
    }

    public Sound GetSound(string nameOfSound)
    {
        foreach (Sound sound in soundsOnObject)
        {
            if (sound.name == nameOfSound) return sound;
        }

        Debug.LogWarning("No sound of name " + nameOfSound + " found on " + gameObject.name);
        return null;

    }

}

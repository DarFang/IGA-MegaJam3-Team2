using UnityEngine;

[CreateAssetMenu(menuName = "AudioSystem/SoundList", fileName = "SoundList")]

public class SoundList : ScriptableObject
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

        //Debug.LogWarning("No sound of name " + nameOfSound + " found");
        return null;

    }

}

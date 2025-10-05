using UnityEngine;

public class SoundList : MonoBehaviour
{
    
    [SerializeField] private Sound[] soundsOnObject;
    private bool initialized = false;

    private void Start()
    {
        if (!CheckIfInitialized())
        {
            Debug.LogWarning("SoundList on " + gameObject.name + " has no sounds!");
        }
        else
        {
            initialized = true;
        }
    }

    public Sound GetSound(int soundID)
    {
        if (!initialized)
        {
            print("SoundList on " + gameObject.name + " has no sounds!");
            return null;
        }

        return soundsOnObject[soundID];
    }

    public Sound GetSound(string nameOfSound)
    {
        if (!initialized) 
        { 
            print("SoundList on " + gameObject.name + " has no sounds! Cannot play " + nameOfSound);
            return null;
        }

        foreach (Sound sound in soundsOnObject)
        {
            if (sound.name == nameOfSound) return sound;
        }

        Debug.LogWarning("No sound of name " + nameOfSound + " found on " + gameObject.name);
        return null;

    }

    private bool CheckIfInitialized()
    {
        if (soundsOnObject == null)
        {
            return false;

        } else 
        {
            return true;
        }
    }

}

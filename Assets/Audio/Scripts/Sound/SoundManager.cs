using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class SoundManager : PersistentSingleton<SoundManager>
{

    public readonly LinkedList<SoundEmitter> FrequentSoundEmitters = new();

    public bool muteAllSounds;

    public SoundBuilder CreateSound() => new SoundBuilder(this);

    public bool CanPlaySound(Sound sound)
    {

        if ((AudioManager.Instance.muteAll))
        {
            Debug.Log("muteAll in the AudioManager is set to true!");
            return false;

        }

        if (muteAllSounds)
        {
            Debug.Log("muteAllSounds in the SoundManager is set to true!");
            return false;
        }

        if (sound.mute)
        {
            Debug.Log(sound.name + " is muted!");
            return false;
        }

        if (!sound.frequentSound) return true;

        if (FrequentSoundEmitters.Count >= AudioManager.Instance.maxSoundInstances)
        {
            try
            {
                FrequentSoundEmitters.First.Value.Stop();
                return true;
            }
            catch 
            {
                Debug.Log("SoundEmitter is already released");
            }
            return false;
        
        }

        return true;
    }

    public SoundEmitter Get()
    {
        return AudioManager.Instance.Get();
    }

}

using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class SoundManager : PersistentSingleton<SoundManager>
{

    public readonly LinkedList<SoundEmitter> FrequentSoundEmitters = new();

    public bool muteAllSounds;

    public Sound gainManaSound, bookSound;

    [SerializeField] private Sound ambience, beach;
    private SoundEmitter ambienceEmitter, beachEmitter;

    /// <summary>
    /// Creates a sound to play. Contains the .AtPosition(Vector3), .SetParent(GameObject), .AutoDuckMusic(AudioMixerGroup), .Play(Sound), and .PlayAndGetSoundEmitter(Sound, out SoundEmiiter) extensions.
    /// </summary>
    public SoundBuilder CreateSound() => new SoundBuilder(this);

    private void Start()
    {
        CreateSound().SetParent(gameObject).PlayAndGetSoundEmitter(ambience, out ambienceEmitter);
    }

    public void ChangeAmbienceFromIndoorToBeach()
    {
        ambienceEmitter.Stop(7);
        CreateSound().Play(beach, 7, out beachEmitter);
    }

    public void ChangeFromBeachToIndoor()
    {
        beachEmitter.Stop(7);
        CreateSound().Play(ambience, 7, out ambienceEmitter);
    }

    public bool CanPlaySound(Sound sound)
    {

        if ((AudioManager.Instance.muteAllAudio))
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

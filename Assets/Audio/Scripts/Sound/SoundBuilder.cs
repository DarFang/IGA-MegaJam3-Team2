using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Builds a sound to be played. Contains numerous options for playback behavior.
/// </summary>
public class SoundBuilder
{
    readonly SoundManager soundManager;
    Vector3 _position = Vector3.zero;
    GameObject _parentOverride = null;
    AudioMixerGroup _groupToDuck = null;

    //gets a reference to the sound manager
    public SoundBuilder(SoundManager soundManager)
    {
        this.soundManager = soundManager;
    }

    /// <summary>
    /// Places the SoundEmitter at the specified local position. Careful using this with .SetParent
    /// </summary>
    /// <param name="position">The local position to put the SoundEmitter object </param>
    public SoundBuilder AtPosition(Vector3 position) 
    { 
        _position = position;
        return this;
    }

    /// <summary>
    /// Parents the SoundEmitter to the given GameObject. Careful using this with .AtPosition
    /// </summary>
    /// <param name="parent">The GameObject to parent the SoundEmitter object to. </param>
    public SoundBuilder SetParent(GameObject parent)
    { 
        _parentOverride = parent;
        return this;
    }


    /// <summary>
    /// Auto ducks the Music AudioMixerGroup.Can be extended to affect any Group in the future.
    /// </summary>
    public SoundBuilder AutoDuckMusic()
    {
        _groupToDuck = AudioMixerController.Instance.MusicGroup;
        return this;
    }

    /// <summary>
    /// Plays the Sound from the SoundEmitter. Takes data from the Sound object and applies them to the SoundEmitter.
    /// </summary>
    /// <param name="sound">The Sound that data is taken from and applied to the SoundEmitter. </param>
    public void Play(Sound sound)
    {
        if (sound == null)
        {
            Debug.LogWarning("Sound is null");
            return;
        }

        if (!soundManager.CanPlaySound(sound)) return;

        SoundEmitter soundEmitter = soundManager.Get();
        soundEmitter.Initialize(sound);

        if (_parentOverride == null)
        {
            soundEmitter.transform.parent = soundManager.transform;
            soundEmitter.transform.localPosition = _position;
        }
        else
        {
            soundEmitter.transform.parent = _parentOverride.transform;
            soundEmitter.transform.localPosition = Vector3.zero;
        }

        if (sound.frequentSound)
        {
            soundEmitter.Node = AudioManager.Instance.FrequentSoundEmitters.AddLast(soundEmitter);
        }

        //put this in the other method lol
        soundEmitter.Play();
        if (_groupToDuck != null) { AudioMixerController.Instance.AutoDuckMusicMixerGroup(soundEmitter.AudioSource.clip); }

    }


    /// <summary>
    /// Plays the Sound from the SoundEmitter. Takes data from the Sound object and applies them to the SoundEmitter, and also ggives a reference to the SoundEMitter so that it can be accessed at a later time (looping sounds).
    /// </summary>
    /// <param name="sound">The Sound that data is taken from and applied to the SoundEmitter. </param>
    /// <param name="soundEmitter">The SoundEmitter that can now be accessed by other scripts. </param>
    public void PlayAndGetSoundEmitter(Sound sound, out SoundEmitter soundEmitter)
    {

        soundEmitter = null;

        if (sound == null)
        {
            Debug.LogError("Sound is null");
            return;
        }

        if (!soundManager.CanPlaySound(sound)) return;

        soundEmitter = soundManager.Get();
        soundEmitter.Initialize(sound);

        if (_parentOverride == null)
        {
            soundEmitter.transform.parent = soundManager.transform;
            soundEmitter.transform.localPosition = _position;
        }
        else
        {
            soundEmitter.transform.parent = _parentOverride.transform;
            soundEmitter.transform.localPosition = Vector3.zero;
        }

        if (sound.frequentSound)
        {
            soundEmitter.Node = AudioManager.Instance.FrequentSoundEmitters.AddLast(soundEmitter);
        }

        soundEmitter.Play();
        if (_groupToDuck != null) { AudioMixerController.Instance.AutoDuckMusicMixerGroup(soundEmitter.AudioSource.clip); }
    }

}

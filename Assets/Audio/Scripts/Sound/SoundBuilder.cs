using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using Unity.VisualScripting.FullSerializer;

public class SoundBuilder
{
    readonly SoundManager soundManager;
    Vector3 _position = Vector3.zero;
    GameObject _parentOverride = null;
    AudioMixerGroup _groupToDuck = null;

    public SoundBuilder(SoundManager soundManager)
    {
        this.soundManager = soundManager;
    }

    public SoundBuilder AtPosition(Vector3 position) 
    { 
        _position = position;
        return this;
    }

    public SoundBuilder SetParent(GameObject parent)
    { 
        _parentOverride = parent;
        return this;
    }

    public SoundBuilder AutoDuckMusic(AudioMixerGroup group)
    {
        _groupToDuck = group;
        return this;
    }

    public void Play(Sound sound)
    {
        if (sound == null)
        {
            Debug.LogError("Sound is null");
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

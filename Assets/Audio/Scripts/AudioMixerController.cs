using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;


/// <summary>
/// Controls AudioMixerGroups for ducking (temporarily lowering volume) and settings purposes.
/// </summary>
public class AudioMixerController : PersistentSingleton<AudioMixerController>
{

    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixerGroup masterGroup;
    [SerializeField] private AudioMixerGroup musicGroup;    
    [SerializeField] private AudioMixerGroup sfxGroup;
    private Dictionary<string, float> mixerGroupDefaultVolumes = new();

    [Header("Auto Duck Settings")]
    [SerializeField] private float defaultAttackTime = 0.05f;
    [SerializeField] private float defaultHoldTime = 0.2f;
    [SerializeField] private float defaultDecayTime = 0.2f;
    [SerializeField] private float defaultDuckAmount = 1;

    public AudioMixerGroup MasterGroup => masterGroup;
    public AudioMixerGroup SFXGroup => sfxGroup;
    public AudioMixerGroup MusicGroup => musicGroup;

    protected override void Awake()
    {

        base.Awake();

        mixerGroupDefaultVolumes.Clear();

        mainMixer.GetFloat(masterGroup.name + "Volume", out float masterVolume);
        mixerGroupDefaultVolumes.Add(masterGroup.name, masterVolume);

        mainMixer.GetFloat(musicGroup.name + "Volume", out float musicVolume);
        mixerGroupDefaultVolumes.Add(musicGroup.name, musicVolume);

        mainMixer.GetFloat(sfxGroup.name + "Volume", out float sfxVolume);
        mixerGroupDefaultVolumes.Add(sfxGroup.name, sfxVolume);
    }

    /// <summary>
    /// Ducks the music AudioMixerGroup by the default settings found on the AudioMixerController object.
    /// </summary>
    public void AutoDuckMusicMixerGroup (AudioClip audioClipDuckingMixer)
    {
        StartCoroutine(AutoDuckMixerGroup(musicGroup, audioClipDuckingMixer));
    }

    private IEnumerator AutoDuckMixerGroup(AudioMixerGroup group,  AudioClip clip)
    {
        //print("Ducking " + group.name);

        float targetVolume = mixerGroupDefaultVolumes[group.name] - defaultDuckAmount;
        mainMixer.GetFloat(group.name + "Volume", out float curVolume);

        if (targetVolume > curVolume) yield break;

        //float holdTime = Mathf.Max(clip.length - defaultAttackTime - defaultDecayTime, 0);
        float holdTime = defaultHoldTime;

        float elapsedTime = 0f;

        //lerp to max amount over attack
        while (elapsedTime < defaultAttackTime)
        {
            float newVolume = Mathf.Lerp(mixerGroupDefaultVolumes[group.name], targetVolume, elapsedTime / defaultAttackTime);
            mainMixer.SetFloat(group.name + "Volume", newVolume);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainMixer.SetFloat(group.name + "Volume", targetVolume);
        elapsedTime = 0f;

        //hold for hold
        while (elapsedTime <= holdTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;

        }

        elapsedTime = 0f;

        //lerp to default over decay
        while (elapsedTime < defaultDecayTime)
        {
            float newVolume = Mathf.Lerp(targetVolume, mixerGroupDefaultVolumes[group.name], elapsedTime / defaultDecayTime);
            mainMixer.SetFloat(group.name + "Volume", newVolume);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainMixer.SetFloat(group.name + "Volume", mixerGroupDefaultVolumes[group.name]);

    }

    private void SetVolumeOfMixerGroup(AudioMixerGroup group, float volume)
    {
        string nameOfParamToChange = group.name + "Volume";
        mainMixer.SetFloat(nameOfParamToChange, volume);
    }

    private void SetVolumeAndDefaultVolumeOfMixerGroup(AudioMixerGroup mixerGroup, float volume)
    {
        SetVolumeOfMixerGroup(mixerGroup, volume);
        mixerGroupDefaultVolumes[mixerGroup.name] = volume;
    }

    #region volume settings
    public void SetMasterVolume(float volume) 
    { 
        SetVolumeAndDefaultVolumeOfMixerGroup(masterGroup, volume);
    }
    public void SetMusicVolume(float volume)
    {
        SetVolumeAndDefaultVolumeOfMixerGroup(musicGroup, volume);
    }
    public void SetSFXVolume(float volume)
    {
        SetVolumeAndDefaultVolumeOfMixerGroup(sfxGroup, volume);
    }
    #endregion
}

//for future use, if the game wants it
public class DuckingEnvelope
{

    public float attack, decay, sustain, hold, release;

    public DuckingEnvelope(float attack, float decay, float sustain, float hold, float release)
    {
        this.attack = attack;
        this.decay = decay;
        this.sustain = sustain;
        this.hold = hold;
        this.release = release;
    }

}

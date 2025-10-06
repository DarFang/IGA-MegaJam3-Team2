using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    
    public LinkedListNode<SoundEmitter> Node { get; set; }
    
    AudioSource audioSource;
    Coroutine playingCoroutine;
    float volumeSaver;

    public AudioSource AudioSource => audioSource;

    private void Awake()
    {
        audioSource = gameObject.GetOrAdd<AudioSource>();
    }

    public void Play()
    {
        if (playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);

        }

        audioSource.Play();
        playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        
    }

    public void Play(float fadeInTime)
    {
        StartCoroutine(IFadeIn(fadeInTime));
    }

    IEnumerator IFadeIn(float fadeInTime)
    {
        if (playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);

        }

        audioSource.volume = 0;
        audioSource.Play();
        playingCoroutine = StartCoroutine(WaitForSoundToEnd());

        float elapsedTime = 0;

        while (elapsedTime < fadeInTime)
        {
            float volume = Mathf.Lerp( 0, volumeSaver, elapsedTime / fadeInTime);
            audioSource.volume = volume;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = volumeSaver;

    }

    public void PlayMusic()
    {

        if (playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);
        }

        audioSource.Play();

    }

    IEnumerator WaitForSoundToEnd()
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        Stop();
    }

    public void Stop()
    {
        if (playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);
            playingCoroutine = null;
        }

        audioSource.Stop();
        AudioManager.Instance.ReturnToPool(this);

    }

    public void Stop(float fadeTime)
    {
        StartCoroutine(IFadeThenStop(fadeTime));
    }

    IEnumerator IFadeThenStop(float fadeTime)
    {
        float elapsedTime = 0;
        float startVolume = audioSource.volume;

        while (elapsedTime < fadeTime)
        {
            float volume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeTime);
            audioSource.volume = volume;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0;
        Stop();

    }

    public void Initialize(Sound sound)
    {
        audioSource.clip = sound.GetAudioClip();
        audioSource.outputAudioMixerGroup = sound.soundSettings.mixerGroup;
        audioSource.loop = sound.soundSettings.loop;
        audioSource.volume = sound.soundSettings.GetVolume();
        audioSource.pitch = sound.soundSettings.GetPitch();
        audioSource.playOnAwake = false;
        audioSource.priority = 128;
        audioSource.spatialBlend = 0; //0 = full 2D, 1 = full 3D
        audioSource.spread = 0;
        audioSource.outputAudioMixerGroup = AudioMixerController.Instance.SFXGroup;
        volumeSaver = audioSource.volume;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    
    public LinkedListNode<SoundEmitter> Node { get; set; }
    
    AudioSource audioSource;
    Coroutine playingCoroutine;

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

    public void Initialize(Sound sound)
    {
        audioSource.clip = sound.GetAudioClip();
        audioSource.outputAudioMixerGroup = sound.soundSettings.mixerGroup;
        audioSource.loop = sound.soundSettings.loop;
        audioSource.volume = sound.soundSettings.GetVolume();
        audioSource.pitch = sound.soundSettings.GetPitch();
        audioSource.playOnAwake = false;
        audioSource.priority = 128;
        audioSource.spatialBlend = 1; //0 = full 2D, 1 = full 3D
        audioSource.spread = 0;
        audioSource.outputAudioMixerGroup = AudioMixerController.Instance.SFXGroup;
    }

}

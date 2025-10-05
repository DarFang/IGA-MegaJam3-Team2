using UnityEngine;
using System;
using UnityEngine.Audio;
using AudioSystem;

[Serializable]
public class Sound
{
    public string name;
    public bool frequentSound, mute;
    public AudioClip[] clips;
    public SoundSettings soundSettings;

    public AudioClip GetAudioClip()
    {
        if (clips == null || clips[0] == null)
        {
            Debug.Log("Tried to play a sound with null sound data or audio clip. Name = " + name);
            return null;
        }

        if (clips.Length > 1)
        {
            int randInt = UnityEngine.Random.Range(0, clips.Length);
            return clips[randInt];
        }
        else
        {
            return clips[0];
        }
    }
}

namespace AudioSystem
{
    [Serializable]
    public class SoundSettings
    {

        public AudioMixerGroup mixerGroup;
        public bool loop;
        public bool randomizeVolume;
        public Vector2 volumeRange = Vector2.right;
        public bool randomizePitch;
        public Vector2 pitchRange = Vector2.right;

        public float volume => GetVolume();
        public float pitch => GetPitch();

        public float GetVolume()
        {
            return randomizeVolume ? UnityEngine.Random.Range(volumeRange.x, volumeRange.y) : volumeRange.x;
        }

        public float GetPitch()
        {
            return randomizePitch ? UnityEngine.Random.Range(pitchRange.x, pitchRange.y) : pitchRange.x;


        }
    }

    public enum MusicLayerBehavior
    {
        Additive,
        Single
    }

}



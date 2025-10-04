using UnityEngine;
using AudioSystem;

[CreateAssetMenu(menuName = "AudioSystem/Music Event", fileName = "MusEvent_")]
public class MusicEvent : ScriptableObject
{
    new public string name = "Song Name";
    public MusicLayer[] musicLayers;
    public MusicLayerBehavior musicLayerBehavior = MusicLayerBehavior.Additive;
    [Range(-1, 0)] public float volume = 0;
    public bool mute = false;
    public int defaultLayer = 0;

}

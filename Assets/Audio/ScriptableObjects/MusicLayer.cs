using UnityEngine;

[CreateAssetMenu(menuName = "AudioSystem/Music Layer", fileName = "LayerName")]
public class MusicLayer : ScriptableObject
{
    public string layerName = "Layer Name";
    public AudioClip clip;
    [Range(0, 1)] public float defaultVolume = 1;
    public bool loop = true;

}

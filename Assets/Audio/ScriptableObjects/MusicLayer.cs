using UnityEngine;

[CreateAssetMenu(menuName = "AudioSystem/Music Layer", fileName = "LayerName")]
public class MusicLayer : ScriptableObject
{
    public string layerName = "Layer Name";
    public AudioClip clip;
    public float defaultVolume = 1;

}

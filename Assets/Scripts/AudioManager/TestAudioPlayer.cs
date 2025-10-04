using UnityEngine;

public class TestAudioPlayer : MonoBehaviour {
    [SerializeField] AudioClip AudioClip;

    AudioManager _audioManager;


    private void Start() {
        _audioManager = AudioManager.Instance;
        _audioManager.SetMusicPlaylist("UI");

        TemporaryAudioSource temporaryAudioSource = _audioManager.PlaySoundAtPosition(AudioClip, transform.position, 1f, 1f);
        temporaryAudioSource.transform.SetParent(transform);
        temporaryAudioSource.transform.localPosition = Vector3.zero;
    }
}

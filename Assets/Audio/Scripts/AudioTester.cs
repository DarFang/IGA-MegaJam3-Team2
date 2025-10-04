using UnityEngine;

public class AudioTester : MonoBehaviour
{
    public Sound testSound;
    public GameObject playSoundObj, playSoundObj2, playSoundObj3;
    SoundEmitter testingEmitter;

    private void Start()
    {
        MusicManager.Instance.PlaySong(0, 5);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SoundManager.Instance.CreateSound().SetParent(playSoundObj).AutoDuckMusic(AudioMixerController.Instance.MusicGroup).PlayAndGetSoundEmitter(testSound, out testingEmitter);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            testingEmitter.Stop();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SoundManager.Instance.CreateSound().SetParent(playSoundObj2).PlayAndGetSoundEmitter(testSound, out testingEmitter);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SoundManager.Instance.CreateSound().SetParent(playSoundObj3).PlayAndGetSoundEmitter(testSound, out testingEmitter);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            AudioMixerController.Instance.SetMasterVolume(-20);

        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MusicManager.Instance.StopAllMusic(5);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            MusicManager.Instance.AddLayer(0, 1, 5);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            MusicManager.Instance.RemoveLayer(0, 1, 5);
        }

    }
}

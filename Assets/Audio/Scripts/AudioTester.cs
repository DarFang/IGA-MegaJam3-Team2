using UnityEngine;

public class AudioTester : MonoBehaviour
{
    public Sound testSound;
    public GameObject playSoundObj, playSoundObj2, playSoundObj3;
    SoundEmitter testingEmitter;

    private void Start()
    {
        

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            MusicManager.Instance.ChangeFromCutsceneToCombat();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            testingEmitter.Stop();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            MusicManager.Instance.ChangeFromCombatToCutscene(5);

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SoundManager.Instance.CreateSound().SetParent(playSoundObj3).PlayAndGetSoundEmitter(testSound, out testingEmitter);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            MusicManager.Instance.ChangeLayers(0, new int[] { 2 }, new int[] { 1 }, 7);

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

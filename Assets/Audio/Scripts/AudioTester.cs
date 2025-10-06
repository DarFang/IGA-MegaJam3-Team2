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
            MusicManager.Instance.SetEnemiesDefeated(1);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            MusicManager.Instance.ChangeFromCombatToCutscene();

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SoundManager.Instance.CreateSound().SetParent(playSoundObj3).PlayAndGetSoundEmitter(testSound, out testingEmitter);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            MusicManager.Instance.SetEnemiesDefeated(4);

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

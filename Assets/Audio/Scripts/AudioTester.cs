using UnityEngine;

public class AudioTester : MonoBehaviour
{
    public SoundList list;
    SoundEmitter testingEmitter;

    private void Start()
    {
        

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SoundManager.Instance.CreateSound().Play(list.GetSound("Attack"));
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SoundManager.Instance.CreateSound().Play(list.GetSound("Defend"));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SoundManager.Instance.CreateSound().Play(list.GetSound("TakeDamage"));
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

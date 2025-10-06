using System;
using UnityEngine;

public class AudioTester : MonoBehaviour
{
    public SoundList list;
    private bool boolean = true;

    private void Start()
    {
        

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            MusicManager.Instance.SetEnemiesDefeated(1);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            MusicManager.Instance.SetEnemiesDefeated(2);

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            MusicManager.Instance.SetEnemiesDefeated(3);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (boolean)
            {
                MusicManager.Instance.ChangeFromCutsceneToCombat();
            }
            else
            {
                MusicManager.Instance.ChangeFromCombatToCutscene();
            }

            boolean = !boolean;

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            MusicManager.Instance.SetEnemiesDefeated(4);

        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            MusicManager.Instance.SetEnemiesDefeated(5);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            MusicManager.Instance.RemoveLayer(0, 1, 5);
        }

    }
}

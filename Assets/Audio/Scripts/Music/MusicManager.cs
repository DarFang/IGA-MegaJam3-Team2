using UnityEngine;
using AudioSystem;
using System.Collections.Generic;
using System.Collections;

public class MusicManager : PersistentSingleton<MusicManager>
{

    public bool muteAllMusic;
    public MusicEvent[] allMusicEvents;
    private Dictionary<string, MusicPlayer> loadedMusicPlayers = new();
    private Dictionary<string, MusicPlayer> activeMusicPlayers = new();

    public void PlaySong(int musicEventID, float fadeInTime)
    {
        if (AudioManager.Instance.muteAllAudio) return;
        if (muteAllMusic) return;
        if (!loadedMusicPlayers.ContainsKey(allMusicEvents[musicEventID].name))
        {
            MusicPlayer newMusicPlayer = gameObject.AddComponent<MusicPlayer>();
            newMusicPlayer.Initialize(allMusicEvents[musicEventID]);
            loadedMusicPlayers.Add(newMusicPlayer.name, newMusicPlayer);
        }

        MusicPlayer playerToStart = loadedMusicPlayers[allMusicEvents[musicEventID].name]; 
        playerToStart.Play(fadeInTime);
        activeMusicPlayers.Add(playerToStart.name, playerToStart);

    }

    public void ChangeLayers(int songID, int[] layerIDsToAdd, int[] layerIDsToRemove, float crossfadeTime)
    {
        activeMusicPlayers[allMusicEvents[songID].name].AddAndRemoveLayers(layerIDsToAdd, layerIDsToRemove, crossfadeTime);
    }

    public void StopSong(int songID, float fadeOutTime)
    {

        MusicPlayer playerToStop = activeMusicPlayers[allMusicEvents[songID].name];

        if (playerToStop != null) StartCoroutine(IStopMusicPlayerThenRemoveFromList(playerToStop, fadeOutTime));

    }

    public void StopAllMusic(float fadeOutTime)
    {
        foreach (KeyValuePair<string, MusicPlayer> entry in activeMusicPlayers)
        {
            StartCoroutine(IStopMusicPlayerThenRemoveFromList(entry.Value, fadeOutTime));
        }

    }

    public void AddLayer(int songID, int layerID, float fadeInTime)
    {
        activeMusicPlayers[allMusicEvents[songID].name].AddLayer(layerID, fadeInTime);
    }

    public void RemoveLayer(int songID, int layerID, float fadeOutTime)
    {
        activeMusicPlayers[allMusicEvents[songID].name].RemoveLayer(layerID, fadeOutTime);
    }

    private IEnumerator IStopMusicPlayerThenRemoveFromList(MusicPlayer playerToRemove, float fadeOutTime)
    {
        playerToRemove.Stop(fadeOutTime);

        yield return new WaitWhile(() => playerToRemove.isPlaying);

        activeMusicPlayers.Remove(playerToRemove.name);

    }

    

}

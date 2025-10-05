using UnityEngine;
using AudioSystem;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Manages all music in the game. Starting, stopping, and changing layers. Pausing can be a thing if we want it. 
/// </summary>
public class MusicManager : PersistentSingleton<MusicManager>
{

    public bool muteAllMusic;
    public MusicEvent[] allMusicEvents;
    private Dictionary<string, MusicPlayer> loadedMusicPlayers = new();
    private Dictionary<string, MusicPlayer> activeMusicPlayers = new();

    private void Start()
    {

        StartCombatMusic();

    }

    public void StartCombatMusic()
    {
        if (AudioManager.Instance.muteAllAudio) return;
        if (muteAllMusic) return;
        if (!loadedMusicPlayers.ContainsKey(allMusicEvents[0].name))
        {
            MusicPlayer newMusicPlayer = gameObject.AddComponent<MusicPlayer>();
            newMusicPlayer.Initialize(allMusicEvents[0]);
            loadedMusicPlayers.Add(newMusicPlayer.name, newMusicPlayer);
        }
        
        MusicPlayer playerToStart = loadedMusicPlayers[allMusicEvents[0].name];
        playerToStart.Play(0.5f);
        playerToStart.PlayDelayed(1, 12.8f);
        activeMusicPlayers.Add(playerToStart.name, playerToStart);
    }

    /// <summary>
    /// Starts a song. See the MusicManager for the musicEventIDs.
    /// </summary>
    /// <param name="musicEventID">See the MusicManager GameObject for all songIDs.</param>
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
    ///<summary>
    ///Changes multiple layers of the given song. See also the .AddLayer and .RemoveLayer methods.
    ///</summary>
    public void ChangeLayers(int musicEventID, int[] layerIDsToAdd, int[] layerIDsToRemove, float crossfadeTime)
    {
        activeMusicPlayers[allMusicEvents[musicEventID].name].AddAndRemoveLayers(layerIDsToAdd, layerIDsToRemove, crossfadeTime);
    }

    /// <summary>
    /// Stops a song after the given fade out time.
    /// </summary>
    /// <param name="musicEventID"></param>
    /// <param name="fadeOutTime"></param>
    public void StopSong(int musicEventID, float fadeOutTime)
    {

        MusicPlayer playerToStop = activeMusicPlayers[allMusicEvents[musicEventID].name];

        if (playerToStop != null) StartCoroutine(IStopMusicPlayerThenRemoveFromList(playerToStop, fadeOutTime));

    }

    /// <summary>
    /// stops all music after the given fade out time.
    /// </summary>
    /// <param name="fadeOutTime"></param>
    public void StopAllMusic(float fadeOutTime)
    {
        foreach (KeyValuePair<string, MusicPlayer> entry in activeMusicPlayers)
        {
            StartCoroutine(IStopMusicPlayerThenRemoveFromList(entry.Value, fadeOutTime));
        }

    }

    /// <summary>
    /// Adds the given layer to the given song over the fade in time
    /// </summary>
    /// <param name="musicEventID">See the MusicManager GameObject for all songIDs.</param>
    /// <param name="layerID">See the MusicManager GameObject for all layerIDs.</param>
    /// <param name="fadeInTime"></param>
    public void AddLayer(int musicEventID, int layerID, float fadeInTime)
    {
        activeMusicPlayers[allMusicEvents[musicEventID].name].AddLayer(layerID, fadeInTime);
    }

    /// <summary>
    /// Removes the given layer from the given song after fading the volume out over the fadeOutTime.
    /// </summary>
    /// <param name="musicEventID">See the MusicManager GameObject for all songIDs.</param>
    /// <param name="layerID">See the MusicManager GameObject for all layerIDs.</param>
    /// <param name="fadeOutTime"></param>
    public void RemoveLayer(int musicEventID, int layerID, float fadeOutTime)
    {
        activeMusicPlayers[allMusicEvents[musicEventID].name].RemoveLayer(layerID, fadeOutTime);
    }

    private IEnumerator IStopMusicPlayerThenRemoveFromList(MusicPlayer playerToRemove, float fadeOutTime)
    {
        playerToRemove.Stop(fadeOutTime);

        yield return new WaitWhile(() => playerToRemove.isPlaying);

        activeMusicPlayers.Remove(playerToRemove.name);

    }

    

}

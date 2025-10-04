using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

[RequireComponent(typeof(PlaylistPlayer))]
public class AudioManager : SingletonManager<AudioManager> {
    [SerializeField] private AudioMixer mixer;

    [Header("Mixer Group Names")]
    [SerializeField] private string masterVolumeName = "masterVol";
    [SerializeField] private string masterMusicVolumeName = "masterMusicVol";
    [SerializeField] private string masterSFXVolumeName = "masterSFXVol";
    [SerializeField] private string musicVolumeName = "musicVol";
    [SerializeField] private string sfxVolumeName = "sfxVol";
    [SerializeField] private string ambientVolumeName = "ambientVol";

    [Header("Audio Sources")]
    [SerializeField] private PlaylistPlayer playlistPlayer;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private TemporaryAudioSource audioSourcePrefab;

    [Header("Music Playlists")]
    [SerializeField] private List<MusicPlaylistData> musicPlaylists = new List<MusicPlaylistData>();

    private ObjectPool<TemporaryAudioSource> audioSourcePool;
    private Transform poolContainer;
    private bool isInitialized = false;

    public bool IsMasterEnabled { get; private set; }
    public bool IsMusicEnabled { get; private set; }
    public bool IsSfxEnabled { get; private set; }

    protected override void Awake() {
        base.Awake();
        Initialize();

        MusicPlaylistData musicPlaylistData = musicPlaylists.First();
        SetMusicPlaylist(musicPlaylistData.categoryName);
    }


    private void Initialize() {
        if (isInitialized) return;

        gameObject.name = "Audio Manager";

        if (mixer == null) {
            Debug.LogError("AudioManager: Mixer is not assigned!");
            return;
        }

        InitializeAudioSources();
        InitializeAudioSourcePool();
        LoadVolumeSettings();

        isInitialized = true;
    }

    private void InitializeAudioSources() {
        if (playlistPlayer == null) {
            playlistPlayer = GetComponent<PlaylistPlayer>();
            if (playlistPlayer == null) {
                Debug.LogError("AudioManager: PlaylistPlayer component not found!");
            }
        }

        if (sfxSource == null) {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    #region Volume Controls

    public void SetMasterVolume(float volume) {
        if (!ValidateMixer()) return;
        float logVolume = ToLogVolume(volume);
        mixer.SetFloat(masterVolumeName, logVolume);
    }

    public void SetMusicVolume(float volume) {
        if (!ValidateMixer()) return;
        float logVolume = ToLogVolume(volume);
        mixer.SetFloat(musicVolumeName, logVolume);
    }

    public void SetSoundVolume(float volume) {
        if (!ValidateMixer()) return;
        float logVolume = ToLogVolume(volume);
        mixer.SetFloat(sfxVolumeName, logVolume);
    }

    public void SetAmbientVolume(float volume) {
        if (!ValidateMixer()) return;
        float logVolume = ToLogVolume(volume);
        mixer.SetFloat(ambientVolumeName, logVolume);
    }

    public void ToggleMaster(bool isEnabled) {
        if (!ValidateMixer()) return;

        IsMasterEnabled = isEnabled;
        mixer.SetFloat(masterVolumeName, isEnabled ? 0f : -80f);

        // якщо вимкнули майстер, зупин€Їмо музику
        if (!isEnabled && IsMusicEnabled) {
            playlistPlayer?.Stop();
        } else if (isEnabled && IsMusicEnabled) {
            playlistPlayer?.Play().Forget();
        }
    }

    public void ToggleMusic(bool isEnabled) {
        if (!ValidateMixer() || !ValidatePlaylistPlayer()) return;
        if (IsMusicEnabled == isEnabled) return;

        IsMusicEnabled = isEnabled;
        mixer.SetFloat(masterMusicVolumeName, isEnabled ? 0f : -80f);

        if (isEnabled && IsMasterEnabled) {
            playlistPlayer.Play().Forget();
        } else {
            playlistPlayer.Stop();
        }
    }

    public void ToggleSounds(bool isEnabled) {
        if (!ValidateMixer()) return;

        IsSfxEnabled = isEnabled;
        mixer.SetFloat(masterSFXVolumeName, isEnabled ? 0f : -80f);
    }

    public float GetMasterVolume() {
        if (!ValidateMixer()) return 0f;
        mixer.GetFloat(masterVolumeName, out float value);
        return FromLogVolume(value);
    }

    public float GetMusicVolume() {
        if (!ValidateMixer()) return 0f;
        mixer.GetFloat(musicVolumeName, out float value);
        return FromLogVolume(value);
    }

    public float GetSoundVolume() {
        if (!ValidateMixer()) return 0f;
        mixer.GetFloat(sfxVolumeName, out float value);
        return FromLogVolume(value);
    }

    public float GetAmbientVolume() {
        if (!ValidateMixer()) return 0f;
        mixer.GetFloat(ambientVolumeName, out float value);
        return FromLogVolume(value);
    }

    private float ToLogVolume(float volume) {
        return volume > 0.001f ? Mathf.Log10(volume) * 20f : -80f;
    }

    private float FromLogVolume(float logVolume) {
        return logVolume > -79f ? Mathf.Pow(10, logVolume / 20f) : 0f;
    }

    #endregion

    #region Sound Effects

    private void InitializeAudioSourcePool() {
        poolContainer = new GameObject("AudioSourcePool").transform;
        poolContainer.SetParent(transform);

        audioSourcePool = new ObjectPool<TemporaryAudioSource>(
            createFunc: CreatePooledAudioSource,
            actionOnGet: OnGetFromPool,
            actionOnRelease: OnReleaseToPool,
            actionOnDestroy: OnDestroyPoolObject,
            defaultCapacity: 10,
            maxSize: 30
        );
    }

    private TemporaryAudioSource CreatePooledAudioSource() {
        TemporaryAudioSource source = Instantiate(audioSourcePrefab, poolContainer);
        source.Initialize(ReturnToPool);
        return source;
    }

    private void OnGetFromPool(TemporaryAudioSource source) {
        source.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(TemporaryAudioSource source) {
        source.Stop();
        source.SetClip(null);
        source.transform.SetParent(poolContainer);
        source.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(TemporaryAudioSource source) {
        if (source != null) {
            Destroy(source.gameObject);
        }
    }

    private void ReturnToPool(TemporaryAudioSource source) {
        if (audioSourcePool != null && source != null) {
            audioSourcePool.Release(source);
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1.0f) {
        if (!IsSfxEnabled || clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public TemporaryAudioSource PlaySoundAtPosition(AudioClip clip, Vector3 position, float power = 1.0f, float spatialBlend = 1.0f) {
        if (!IsSfxEnabled || clip == null || audioSourcePool == null) return null;

        TemporaryAudioSource source = audioSourcePool.Get();
        source.SetupSource(clip, position, power, spatialBlend);
        source.Play();
        return source;
    }

    #endregion

    #region Playlist Management

    public void SetMusicPlaylist(string categoryName, bool autoPlay = true) {
        EnsureInitialized();

        if (!ValidatePlaylistPlayer()) return;

        if (musicPlaylists == null || musicPlaylists.Count == 0) {
            Debug.LogError("AudioManager: Music playlists collection is empty");
            return;
        }

        var playlist = musicPlaylists.Find(p => p != null && p.categoryName == categoryName);

        if (playlist == null || playlist.musicTracks == null || playlist.musicTracks.Count == 0) {
            Debug.LogWarning($"AudioManager: Playlist '{categoryName}' not found or empty");
            return;
        }

        playlistPlayer.SetPlaylist(playlist, autoPlay && IsMusicEnabled && IsMasterEnabled);
    }

    public void AddMusicPlaylist(string categoryName, List<AudioClip> tracks) {
        if (string.IsNullOrEmpty(categoryName)) {
            Debug.LogError("AudioManager: Category name cannot be empty");
            return;
        }

        if (tracks == null || tracks.Count == 0) {
            Debug.LogError("AudioManager: Cannot add empty playlist");
            return;
        }

        var existingPlaylist = musicPlaylists.Find(p => p.categoryName == categoryName);

        if (existingPlaylist != null) {
            existingPlaylist.musicTracks = new List<AudioClip>(tracks);
            Debug.Log($"AudioManager: Updated playlist '{categoryName}' with {tracks.Count} tracks");
        } else {
            MusicPlaylistData newPlaylist = new MusicPlaylistData {
                categoryName = categoryName,
                musicTracks = new List<AudioClip>(tracks),
            };
            musicPlaylists.Add(newPlaylist);
            Debug.Log($"AudioManager: Added new playlist '{categoryName}' with {tracks.Count} tracks");
        }
    }

    public bool RemoveMusicPlaylist(string categoryName) {
        var playlist = musicPlaylists.Find(p => p.categoryName == categoryName);

        if (playlist != null) {
            if (playlistPlayer != null && playlistPlayer.CurrentPlaylistName == categoryName) {
                playlistPlayer.Stop();
            }

            musicPlaylists.Remove(playlist);
            return true;
        }

        return false;
    }

    public List<string> GetAvailablePlaylists() {
        List<string> playlistNames = new List<string>();

        if (musicPlaylists != null) {
            foreach (var playlist in musicPlaylists) {
                if (playlist != null && !string.IsNullOrEmpty(playlist.categoryName)) {
                    playlistNames.Add(playlist.categoryName);
                }
            }
        }

        return playlistNames;
    }

    #endregion

    #region Settings

    public void LoadVolumeSettings() {
        if (!ValidateMixer()) return;

        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float ambientVol = PlayerPrefs.GetFloat("AmbientVolume", 1f);

        SetMasterVolume(masterVol);
        SetMusicVolume(musicVol);
        SetSoundVolume(sfxVol);
        SetAmbientVolume(ambientVol);

        IsMasterEnabled = PlayerPrefs.GetInt("MasterVolumeEnabled", 1) == 1;
        IsMusicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        IsSfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        mixer.SetFloat(masterVolumeName, IsMasterEnabled ? 0f : -80f);
        mixer.SetFloat(masterMusicVolumeName, IsMusicEnabled ? 0f : -80f);
        mixer.SetFloat(masterSFXVolumeName, IsSfxEnabled ? 0f : -80f);
    }

    public void SaveVolumeSettings() {
        if (!ValidateMixer()) return;

        PlayerPrefs.SetFloat("MasterVolume", GetMasterVolume());
        PlayerPrefs.SetFloat("MusicVolume", GetMusicVolume());
        PlayerPrefs.SetFloat("SFXVolume", GetSoundVolume());
        PlayerPrefs.SetFloat("AmbientVolume", GetAmbientVolume());

        PlayerPrefs.SetInt("MasterVolumeEnabled", IsMasterEnabled ? 1 : 0);
        PlayerPrefs.SetInt("MusicEnabled", IsMusicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("SFXEnabled", IsSfxEnabled ? 1 : 0);

        PlayerPrefs.Save();
    }

    #endregion

    #region Validation

    private void EnsureInitialized() {
        if (!isInitialized) {
            Initialize();
        }
    }

    private bool ValidateMixer() {
        if (mixer == null) {
            Debug.LogError("AudioManager: Mixer is not assigned!");
            return false;
        }
        return true;
    }

    private bool ValidatePlaylistPlayer() {
        if (playlistPlayer == null) {
            Debug.LogError("AudioManager: PlaylistPlayer is not assigned!");
            return false;
        }
        return true;
    }

    #endregion

    protected override void OnDestroy() {
        base.OnDestroy();
        SaveVolumeSettings();

        // ќчищуЇмо пул
        if (audioSourcePool != null) {
            audioSourcePool.Clear();
        }
    }
}


[System.Serializable]
public class MusicPlaylist {
    public MusicPlaylistData Data { get; private set; }
    private List<AudioClip> _currentOrder;
    private int _currentIndex = -1;

    public AudioClip CurrentTrack => _currentIndex >= 0 && _currentIndex < _currentOrder.Count ? _currentOrder[_currentIndex] : null;
    public int TrackCount => _currentOrder?.Count ?? 0;
    public bool HasTracks => TrackCount > 0;

    public MusicPlaylist(MusicPlaylistData data) {
        Data = data;
        ResetToOriginalOrder();
    }

    public AudioClip GetNext(bool loop = true) {
        if (!HasTracks) return null;

        if (_currentIndex < _currentOrder.Count - 1) {
            _currentIndex++;
        } else if (loop) {
            _currentIndex = 0;
        } else {
            return null;
        }

        return _currentOrder[_currentIndex];
    }

    public AudioClip GetPrevious(bool loop = true) {
        if (!HasTracks) return null;

        if (_currentIndex > 0) {
            _currentIndex--;
        } else if (loop) {
            _currentIndex = _currentOrder.Count - 1;
        } else {
            return null;
        }

        return _currentOrder[_currentIndex];
    }

    public AudioClip GetCurrent() {
        return CurrentTrack;
    }

    public AudioClip GetAt(int index) {
        if (index >= 0 && index < _currentOrder.Count) {
            _currentIndex = index;
            return _currentOrder[index];
        }
        return null;
    }

    public void Shuffle() {
        if (!HasTracks) return;
        System.Random random = new System.Random();

        var currentTrack = CurrentTrack;
        _currentOrder = _currentOrder.OrderBy(x => random.Next()).ToList();

        // «бер≥гаЇмо поточний трек на тому ж м≥сц≥
        if (currentTrack != null) {
            var newIndex = _currentOrder.IndexOf(currentTrack);
            if (newIndex >= 0) {
                // ћ≥н€Їмо м≥сц€ми з поточним ≥ндексом
                (_currentOrder[newIndex], _currentOrder[_currentIndex]) =
                    (_currentOrder[_currentIndex], _currentOrder[newIndex]);
            }
        }
    }

    public void ResetToOriginalOrder() {
        _currentOrder = Data?.musicTracks?.ToList() ?? new List<AudioClip>();
        _currentIndex = -1;
    }

    public int GetCurrentTrackIndex() => _currentIndex;
    public string GetCurrentTrackName() => CurrentTrack?.name ?? "No track";
}
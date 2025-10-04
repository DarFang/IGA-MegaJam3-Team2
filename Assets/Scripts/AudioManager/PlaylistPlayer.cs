using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class PlaylistPlayer : MonoBehaviour {
    [SerializeField] private AudioSource _musicSource;
    [Header("Settings")]
    [SerializeField] private bool _autoPlayOnStart = true;

    private MusicPlaylist _currentPlaylist;
    private AudioTrackPlayer _trackPlayer;

    public bool IsLooping { get; set; } = true;
    public bool IsPlaying => _trackPlayer?.IsPlaying ?? false;
    public bool IsPaused => _trackPlayer?.IsPaused ?? false;
    public MusicPlaylistData CurrentPlaylistData => _currentPlaylist?.Data;
    public string CurrentPlaylistName => _currentPlaylist?.Data?.categoryName ?? string.Empty;
    public string CurrentTrackName => _currentPlaylist?.GetCurrentTrackName() ?? "No track";

    public int CurrentTrackIndex => _currentPlaylist?.GetCurrentTrackIndex() ?? 0;
    public int TotalTracks => _currentPlaylist?.TrackCount ?? 0;

    public event Action<AudioClip> OnTrackStarted;
    //public event Action<AudioClip> OnTrackCompleted;
    public event Action<AudioClip> OnTrackStopped;
    public event Action<MusicPlaylistData> OnPlaylistChanged;

    private void Awake() {
        if (_musicSource == null) {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = false;
        }

        _trackPlayer = new AudioTrackPlayer(_musicSource);
        _trackPlayer.OnTrackCompleted += HandleTrackCompleted;
        _trackPlayer.OnTrackStarted += (clip) => OnTrackStarted?.Invoke(clip);
        _trackPlayer.OnTrackStopped += (clip) => OnTrackStopped?.Invoke(clip);
    }

    private void Start() {
        if (_autoPlayOnStart && _currentPlaylist != null) {
            Play().Forget();
        }
    }

    private void Update() {
        // Автоматичне відтворення наступного треку
        if (ShouldPlayNextTrack()) {
            PlayNextTrack().Forget();
        }
    }

    private bool ShouldPlayNextTrack() {
        return IsPlaying &&
               !IsPaused &&
               _currentPlaylist != null &&
               _currentPlaylist.HasTracks &&
               !_trackPlayer.IsPlaying &&
               Application.isFocused;
    }

    private async void HandleTrackCompleted(AudioClip completedClip) {
        if (IsPlaying && !IsPaused) {
            await PlayNextTrack();
        }
    }

    public void SetPlaylist(MusicPlaylistData playlistData, bool autoPlay = true) {
        if (_currentPlaylist != null && _currentPlaylist.Data == playlistData) {
            Debug.LogWarning("PlaylistPlayer: Trying to assign the same playlist");
            return;
        }

        Stop();
        _currentPlaylist = new MusicPlaylist(playlistData);
        OnPlaylistChanged?.Invoke(playlistData);

        if (autoPlay) {
            Play().Forget();
        }
    }

    public async UniTask Play() {
        if (_currentPlaylist == null || !_currentPlaylist.HasTracks) return;

        Stop();
        await PlayCurrentTrack();
    }

    public void Stop() {
        _trackPlayer?.Stop();
    }

    public void Pause() => _trackPlayer?.Pause();
    public void Resume() => _trackPlayer?.Resume();

    public async UniTask PlayNextTrack() {
        if (_currentPlaylist == null) return;

        var nextTrack = _currentPlaylist.GetNext(IsLooping);
        if (nextTrack != null) {
            await _trackPlayer.Play(nextTrack);
        } else {
            Stop();
        }
    }

    public async UniTask PlayPreviousTrack() {
        if (_currentPlaylist == null) return;

        var prevTrack = _currentPlaylist.GetPrevious(IsLooping);
        if (prevTrack != null) {
            await _trackPlayer.Play(prevTrack);
        }
    }

    public void RestartCurrentTrack() {
        if (_currentPlaylist == null) return;

        var currentTrack = _currentPlaylist.GetCurrent();
        if (currentTrack != null) {
            _trackPlayer.Play(currentTrack).Forget();
        }
    }

    private async UniTask PlayCurrentTrack() {
        if (_currentPlaylist == null) return;

        var track = _currentPlaylist.GetCurrent();
        if (track == null) {
            track = _currentPlaylist.GetNext(IsLooping);
        }

        if (track != null && _trackPlayer != null) {
            await _trackPlayer.Play(track);
        }
    }

    public void ToggleShuffle(bool shuffle) {
        _currentPlaylist?.Shuffle();
    }

    public void SetVolume(float volume) {
        if (_trackPlayer != null) {
            _trackPlayer.Volume = Mathf.Clamp01(volume);
        }
    }

    private void OnDestroy() {
        _trackPlayer?.Dispose();
    }

    public float GetCurrentVolume() {
        if (_trackPlayer == null) {
            return 0f;
        }
        return _trackPlayer.Volume;
    }

    public float GetCurrentTime() {
        return _trackPlayer.CurrentTime;
    }

    public async UniTask PlayTrackAt(int i) {
        AudioClip audioClip = _currentPlaylist.GetAt(i);
        await _trackPlayer.Play(audioClip);
    }
}

public class AudioTrackPlayer {
    private readonly AudioSource _audioSource;
    private CancellationTokenSource _playbackCTS;

    public bool IsPlaying { get; private set; }
    public bool IsPaused { get; private set; }
    public AudioClip CurrentClip { get; private set; }
    public float CurrentTime => _audioSource?.time ?? 0f;
    public float Volume {
        get => _audioSource.volume;
        set => _audioSource.volume = value;
    }

    public event Action<AudioClip> OnTrackStarted;
    public event Action<AudioClip> OnTrackCompleted;
    public event Action<AudioClip> OnTrackStopped;

    public AudioTrackPlayer(AudioSource audioSource) {
        _audioSource = audioSource;
        _audioSource.loop = false;
    }

    public async UniTask Play(AudioClip clip, float startTime = 0f, CancellationToken externalToken = default) {
        if (clip == null) return;
        Stop();

        CurrentClip = clip;
        IsPlaying = true;
        IsPaused = false;

        _playbackCTS = CancellationTokenSource.CreateLinkedTokenSource(externalToken);

        try {
            _audioSource.clip = clip;
            _audioSource.time = startTime;
            _audioSource.Play();

            OnTrackStarted?.Invoke(clip);

            await WaitForTrackCompletion(_playbackCTS.Token);

            if (IsPlaying && !IsPaused) {
                OnTrackCompleted?.Invoke(clip);
            }
        } catch (OperationCanceledException) {
            // Playback was cancelled - normal case
        } finally {
            IsPlaying = false;
            CurrentClip = null;
        }
    }

    public void Stop() {
        if (!IsPlaying) return;

        var stoppedClip = CurrentClip;

        _playbackCTS?.Cancel();
        _playbackCTS?.Dispose();
        _playbackCTS = null;

        _audioSource.Stop();
        IsPlaying = false;
        IsPaused = false;

        if (stoppedClip != null) {
            OnTrackStopped?.Invoke(stoppedClip);
        }
    }

    public void Pause() {
        if (!IsPlaying || IsPaused) return;

        IsPaused = true;
        _audioSource.Pause();
    }

    public void Resume() {
        if (!IsPlaying || !IsPaused) return;

        IsPaused = false;
        _audioSource.Play();
    }

    public void SetTime(float time) {
        if (_audioSource != null && CurrentClip != null) {
            _audioSource.time = Mathf.Clamp(time, 0f, CurrentClip.length);
        }
    }

    private async UniTask WaitForTrackCompletion(CancellationToken cancellationToken) {
        await UniTask.WaitWhile(() =>
            _audioSource != null &&
            _audioSource.isPlaying &&
            IsPlaying &&
            !IsPaused,
            cancellationToken: cancellationToken
        );
    }

    public void Dispose() {
        Stop();
    }
}

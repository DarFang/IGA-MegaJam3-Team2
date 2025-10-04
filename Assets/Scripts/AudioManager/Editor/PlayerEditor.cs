#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlaylistPlayer))]
public class PlaylistPlayerEditor : Editor {
    private PlaylistPlayer player;
    private MusicPlaylistData newPlaylistData;

    private bool showAudioSettings = true;
    private bool showPlaylistInfo = true;
    private bool showPlayerControls = true;
    private bool showPlaybackOptions = true;
    private bool showRuntimeInfo = true;

    private Vector2 trackListScroll;
    private GUIStyle headerStyle;
    private GUIStyle infoBoxStyle;
    private bool stylesInitialized;

    private void OnEnable() {
        player = (PlaylistPlayer)target;
        EditorApplication.update += Repaint;
    }

    private void OnDisable() {
        EditorApplication.update -= Repaint;
    }

    private void InitializeStyles() {
        if (stylesInitialized) return;

        headerStyle = new GUIStyle(EditorStyles.boldLabel) {
            fontSize = 12,
            margin = new RectOffset(0, 0, 10, 5)
        };

        infoBoxStyle = new GUIStyle(EditorStyles.helpBox) {
            padding = new RectOffset(10, 10, 10, 10)
        };

        stylesInitialized = true;
    }

    public override void OnInspectorGUI() {
        InitializeStyles();

        serializedObject.Update();

        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        DrawSeparator();

        DrawPlaylistManagement();
        DrawPlaylistInfo();
        DrawPlayerControls();
        DrawPlaybackOptions();
        DrawRuntimeInfo();

        serializedObject.ApplyModifiedProperties();

        // Для live update в Editor mode
        if (GUI.changed) {
            EditorUtility.SetDirty(target);
        }
    }

    private void DrawPlaylistManagement() {
        EditorGUILayout.Space(5);
        showPlaylistInfo = EditorGUILayout.BeginFoldoutHeaderGroup(showPlaylistInfo, "Playlist Management");

        if (showPlaylistInfo) {
            EditorGUILayout.BeginVertical(infoBoxStyle);

            // Вибір нового плейлиста
            EditorGUI.BeginChangeCheck();
            newPlaylistData = (MusicPlaylistData)EditorGUILayout.ObjectField(
                "Assign New Playlist",
                newPlaylistData,
                typeof(MusicPlaylistData),
                false
            );

            if (EditorGUI.EndChangeCheck() && newPlaylistData != null) {
                EditorGUILayout.HelpBox(
                    $"Playlist: {newPlaylistData.categoryName}\nTracks: {newPlaylistData.musicTracks?.Count ?? 0}",
                    MessageType.Info
                );
            }

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = newPlaylistData != null;
            if (GUILayout.Button("Apply & Play", GUILayout.Height(25))) {
                player.SetPlaylist(newPlaylistData, true);
                newPlaylistData = null;
            }

            if (GUILayout.Button("Apply (No Play)", GUILayout.Height(25))) {
                player.SetPlaylist(newPlaylistData, false);
                newPlaylistData = null;
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawPlaylistInfo() {
        if (player.CurrentPlaylistData == null) return;

        EditorGUILayout.Space(5);
        showAudioSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showAudioSettings, "Current Playlist Info");

        if (showAudioSettings) {
            EditorGUILayout.BeginVertical(infoBoxStyle);

            var data = player.CurrentPlaylistData;

            EditorGUILayout.LabelField("Playlist Name", data.categoryName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Total Tracks", player.TotalTracks.ToString());

            if (player.TotalTracks > 0) {
                EditorGUILayout.LabelField(
                    "Current Track",
                    $"{player.CurrentTrackIndex + 1} / {player.TotalTracks}"
                );

                EditorGUILayout.Space(5);
                DrawTrackList();
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawTrackList() {
        if (player.CurrentPlaylistData == null || player.CurrentPlaylistData.musicTracks == null)
            return;

        EditorGUILayout.LabelField("Track List:", EditorStyles.boldLabel);

        trackListScroll = EditorGUILayout.BeginScrollView(
            trackListScroll,
            GUILayout.Height(Mathf.Min(player.TotalTracks * 22 + 10, 150))
        );

        var tracks = player.CurrentPlaylistData.musicTracks;

        for (int i = 0; i < tracks.Count; i++) {
            if (tracks[i] == null) continue;

            EditorGUILayout.BeginHorizontal();

            // Виділяємо поточний трек
            bool isCurrent = i == player.CurrentTrackIndex;
            GUI.backgroundColor = isCurrent ? new Color(0.5f, 1f, 0.5f) : Color.white;

            string trackName = tracks[i].name;
            if (isCurrent && player.IsPlaying) {
                trackName = $"▶ {trackName}";
            } else if (isCurrent) {
                trackName = $"● {trackName}";
            }

            EditorGUILayout.LabelField($"{i + 1}.", GUILayout.Width(30));
            EditorGUILayout.LabelField(trackName);

            GUI.backgroundColor = Color.white;

            // Кнопка для швидкого переходу до треку
            if (GUILayout.Button("Play", GUILayout.Width(50))) {
                player.PlayTrackAt(i).Forget();
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawPlayerControls() {
        EditorGUILayout.Space(5);
        showPlayerControls = EditorGUILayout.BeginFoldoutHeaderGroup(showPlayerControls, "Player Controls");

        if (showPlayerControls) {
            EditorGUILayout.BeginVertical(infoBoxStyle);

            bool hasPlaylist = player.CurrentPlaylistData != null && player.TotalTracks > 0;
            GUI.enabled = hasPlaylist;

            // Основні кнопки
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(player.IsPlaying && !player.IsPaused ? "■ Stop" : "▶ Play", GUILayout.Height(30))) {
                if (player.IsPlaying && !player.IsPaused) {
                    player.Stop();
                } else {
                    player.Play().Forget();
                }
            }

            if (player.IsPlaying) {
                if (GUILayout.Button(player.IsPaused ? "▶ Resume" : "❚❚ Pause", GUILayout.Height(30))) {
                    if (player.IsPaused) {
                        player.Resume();
                    } else {
                        player.Pause();
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Навігація
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("⏮ Previous", GUILayout.Height(25))) {
                player.PlayPreviousTrack().Forget();
            }

            if (GUILayout.Button("⟲ Restart", GUILayout.Height(25))) {
                player.RestartCurrentTrack();
            }

            if (GUILayout.Button("Next ⏭", GUILayout.Height(25))) {
                player.PlayNextTrack().Forget();
            }

            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawPlaybackOptions() {
        EditorGUILayout.Space(5);
        showPlaybackOptions = EditorGUILayout.BeginFoldoutHeaderGroup(showPlaybackOptions, "Playback Options");

        if (showPlaybackOptions) {
            EditorGUILayout.BeginVertical(infoBoxStyle);

            bool hasPlaylist = player.CurrentPlaylistData != null;
            GUI.enabled = hasPlaylist;

            // Loop toggle
            EditorGUI.BeginChangeCheck();
            bool newLoop = EditorGUILayout.Toggle("Loop Playlist", player.IsLooping);
            if (EditorGUI.EndChangeCheck()) {
                player.IsLooping = newLoop;
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space(3);

            // Shuffle toggle
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Shuffle", GUILayout.Width(EditorGUIUtility.labelWidth));

            if (GUILayout.Button("Enable Shuffle")) {
                player.ToggleShuffle(true);
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Disable Shuffle")) {
                player.ToggleShuffle(false);
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;

            // Volume control
            EditorGUILayout.Space(5);
            float newVolume = EditorGUILayout.Slider("Volume", player.GetCurrentVolume(), 0f, 1f);
            if (!Mathf.Approximately(newVolume, player.GetCurrentVolume())) {
                player.SetVolume(newVolume);
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawRuntimeInfo() {
        EditorGUILayout.Space(5);
        showRuntimeInfo = EditorGUILayout.BeginFoldoutHeaderGroup(showRuntimeInfo, "Runtime Info");

        if (showRuntimeInfo) {
            EditorGUILayout.BeginVertical(infoBoxStyle);

            // Status
            string status = GetStatusString();
            Color statusColor = GetStatusColor();

            var oldColor = GUI.contentColor;
            GUI.contentColor = statusColor;
            EditorGUILayout.LabelField("Status:", status, EditorStyles.boldLabel);
            GUI.contentColor = oldColor;

            // Current track info
            if (player.CurrentPlaylistData != null) {
                EditorGUILayout.LabelField("Current Track:", player.CurrentTrackName);

                // Прогрес бар
                if (player.IsPlaying) {
                    float currentTime = player.GetCurrentTime();
                    var currentClip = player.CurrentPlaylistData.musicTracks?[player.CurrentTrackIndex];

                    if (currentClip != null) {
                        float duration = currentClip.length;
                        float progress = duration > 0 ? currentTime / duration : 0f;

                        EditorGUILayout.Space(3);
                        Rect progressRect = EditorGUILayout.GetControlRect(GUILayout.Height(20));
                        EditorGUI.ProgressBar(progressRect, progress,
                            $"{FormatTime(currentTime)} / {FormatTime(duration)}");
                    }
                }
            } else {
                EditorGUILayout.HelpBox("No playlist assigned", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private string GetStatusString() {
        if (!player.IsPlaying) return "⏹ Stopped";
        if (player.IsPaused) return "⏸ Paused";
        return "▶ Playing";
    }

    private Color GetStatusColor() {
        if (!player.IsPlaying) return Color.gray;
        if (player.IsPaused) return Color.yellow;
        return Color.green;
    }

    private string FormatTime(float timeInSeconds) {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    private void DrawSeparator() {
        EditorGUILayout.Space(5);
        Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(2));
        rect.height = 1;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        EditorGUILayout.Space(5);
    }
}

// Extension methods для PlaylistPlayer (додайте ці методи в PlaylistPlayer.cs)
public static class PlaylistPlayerEditorExtensions {
    // Ці методи потрібно додати в сам клас PlaylistPlayer:
    // public float GetCurrentVolume() => trackPlayer?.Volume ?? 0f;
}
#endif
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StatsData))]
public class StatsDataEditor : Editor {
    private SerializedProperty healthDataProp;
    private SerializedProperty attackDataProp;
    private SerializedProperty defenseDataProp;
    private SerializedProperty manaDataProp;
    private SerializedProperty speedDataProp;

    private bool showHealth = true;
    private bool showAttack = true;
    private bool showDefense = true;
    private bool showMana = true;
    private bool showSpeed = true;
    private bool showQuickActions = true;

    private void OnEnable() {
        healthDataProp = serializedObject.FindProperty("healthData");
        attackDataProp = serializedObject.FindProperty("attackData");
        defenseDataProp = serializedObject.FindProperty("defenseData");
        manaDataProp = serializedObject.FindProperty("manaData");
        speedDataProp = serializedObject.FindProperty("speedData");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.Space(10);

        // Quick Actions
        showQuickActions = EditorGUILayout.Foldout(showQuickActions, "🚀 Quick Actions", true);
        if (showQuickActions) {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Quick Setup", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset All To Default")) {
                ResetAllToDefault();
            }
            if (GUILayout.Button("Initialize All Stats")) {
                InitializeAllStats();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        // Health Section
        DrawStatSection(ref showHealth, "❤️ Health", healthDataProp, typeof(RegenerableStatData));

        // Attack Section
        DrawStatSection(ref showAttack, "⚔️ Attack", attackDataProp, typeof(StatData));

        // Defense Section
        DrawStatSection(ref showDefense, "🛡️ Defense", defenseDataProp, typeof(StatData));

        // Mana Section
        DrawStatSection(ref showMana, "🔮 Mana", manaDataProp, typeof(RegenerableStatData));

        // Speed Section
        DrawStatSection(ref showSpeed, "💨 Speed", speedDataProp, typeof(StatData));

        // Summary
        DrawSummary();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawStatSection(ref bool foldout, string title, SerializedProperty property, System.Type type) {
        foldout = EditorGUILayout.Foldout(foldout, title, true);

        if (foldout) {
            EditorGUILayout.BeginVertical("box");

            // Header with type info
            EditorGUILayout.LabelField($"Type: {type.Name}", EditorStyles.miniLabel);

            // Draw all child properties of the serialized object
            SerializedProperty currentProp = property.Copy();
            SerializedProperty endProp = property.GetEndProperty();

            bool enterChildren = true;
            while (currentProp.NextVisible(enterChildren) && !SerializedProperty.EqualContents(currentProp, endProp)) {
                enterChildren = false;
                EditorGUILayout.PropertyField(currentProp, true);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
    }

    private void DrawSummary() {
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("📊 Stats Summary", EditorStyles.boldLabel);

        var statsData = (StatsData)target;
        int initializedStats = 0;
        int totalStats = 5;

        // Check if stats have non-zero values (considered initialized)
        if (IsStatInitialized(statsData.HealthData)) initializedStats++;
        if (IsStatInitialized(statsData.AttackData)) initializedStats++;
        if (IsStatInitialized(statsData.DefenseData)) initializedStats++;
        if (IsStatInitialized(statsData.ManaData)) initializedStats++;
        if (IsStatInitialized(statsData.SpeedData)) initializedStats++;

        EditorGUILayout.LabelField($"Initialized: {initializedStats}/{totalStats}");

        // Progress bar
        float progress = (float)initializedStats / totalStats;
        Rect progressRect = GUILayoutUtility.GetRect(200, 20);
        EditorGUI.ProgressBar(progressRect, progress,
            initializedStats == totalStats ? "Complete!" : "Incomplete");

        // Show basic stats if all initialized
        if (initializedStats > 0) {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Quick Stats Overview:", EditorStyles.miniBoldLabel);

            if (IsStatInitialized(statsData.HealthData))
                EditorGUILayout.LabelField($"HP: {statsData.HealthData.BaseValue}/{statsData.HealthData.MaxValue}");
            if (IsStatInitialized(statsData.AttackData))
                EditorGUILayout.LabelField($"ATK: {statsData.AttackData.BaseValue}");
            if (IsStatInitialized(statsData.DefenseData))
                EditorGUILayout.LabelField($"DEF: {statsData.DefenseData.BaseValue}");
            if (IsStatInitialized(statsData.ManaData))
                EditorGUILayout.LabelField($"MP: {statsData.ManaData.BaseValue}/{statsData.ManaData.MaxValue}");
            if (IsStatInitialized(statsData.SpeedData))
                EditorGUILayout.LabelField($"SPD: {statsData.SpeedData.BaseValue}");
        }

        EditorGUILayout.EndVertical();
    }

    private bool IsStatInitialized(StatData stat) {
        return stat != null && (stat.BaseValue > 0 || stat.MaxValue > 0);
    }

    private void InitializeAllStats() {
        if (EditorUtility.DisplayDialog("Initialize All Stats",
            "This will set default values for all stats. Continue?",
            "Yes", "No")) {

            InitializeStat(healthDataProp, 100f, 100f);
            InitializeStat(attackDataProp, 10f, 100f);
            InitializeStat(defenseDataProp, 5f, 100f);
            InitializeStat(manaDataProp, 50f, 50f);
            InitializeStat(speedDataProp, 5f, 20f);

            serializedObject.ApplyModifiedProperties();
        }
    }

    private void InitializeStat(SerializedProperty property, float baseValue, float maxValue) {
        var baseValueProp = property.FindPropertyRelative("baseValue");
        var maxValueProp = property.FindPropertyRelative("maxValue");

        if (baseValueProp != null) baseValueProp.floatValue = baseValue;
        if (maxValueProp != null) maxValueProp.floatValue = maxValue;
    }

    private void ResetAllToDefault() {
        if (EditorUtility.DisplayDialog("Reset All Stats",
            "Are you sure you want to reset all stat values to zero?",
            "Yes", "No")) {

            ResetStat(healthDataProp);
            ResetStat(attackDataProp);
            ResetStat(defenseDataProp);
            ResetStat(manaDataProp);
            ResetStat(speedDataProp);

            serializedObject.ApplyModifiedProperties();
        }
    }

    private void ResetStat(SerializedProperty property) {
        var baseValueProp = property.FindPropertyRelative("baseValue");
        var maxValueProp = property.FindPropertyRelative("maxValue");
        var minValueProp = property.FindPropertyRelative("minValue");

        if (baseValueProp != null) baseValueProp.floatValue = 0f;
        if (maxValueProp != null) maxValueProp.floatValue = 0f;
        if (minValueProp != null) minValueProp.floatValue = 0f;
    }
}
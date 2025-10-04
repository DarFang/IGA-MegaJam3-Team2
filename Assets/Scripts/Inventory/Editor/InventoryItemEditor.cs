using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InventoryItem), true)]
public class InventoryItemEditor : Editor {
    private SerializedProperty itemNameProp;
    private SerializedProperty itemIconProp;
    private SerializedProperty itemLoreProp;
    private SerializedProperty itemTypeProp;
    private SerializedProperty rarityProp;
    private SerializedProperty isStackableProp;
    private SerializedProperty maxStackSizeProp;
    private SerializedProperty hasAbilityProp;
    private SerializedProperty abilityProp;

    private bool showBasicInfo = true;
    private bool showStackSettings = true;
    private bool showAbility = true;
    private bool showPreview = true;

    private void OnEnable() {
        itemNameProp = serializedObject.FindProperty("itemName");
        itemIconProp = serializedObject.FindProperty("itemIcon");
        itemLoreProp = serializedObject.FindProperty("itemLore");
        itemTypeProp = serializedObject.FindProperty("itemType");
        rarityProp = serializedObject.FindProperty("rarity");
        isStackableProp = serializedObject.FindProperty("isStackable");
        maxStackSizeProp = serializedObject.FindProperty("maxStackSize");
        hasAbilityProp = serializedObject.FindProperty("hasAbility");
        abilityProp = serializedObject.FindProperty("ability");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        // Header with item name
        EditorGUILayout.Space(5);
        DrawHeader();

        // Basic Info Section
        showBasicInfo = EditorGUILayout.Foldout(showBasicInfo, "📦 Basic Information", true, EditorStyles.foldoutHeader);
        if (showBasicInfo) {
            EditorGUILayout.BeginVertical("box");
            DrawBasicInfo();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        // Stack Settings Section
        showStackSettings = EditorGUILayout.Foldout(showStackSettings, "📚 Stack Settings", true, EditorStyles.foldoutHeader);
        if (showStackSettings) {
            EditorGUILayout.BeginVertical("box");
            DrawStackSettings();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        // Ability Section
        showAbility = EditorGUILayout.Foldout(showAbility, "✨ Item Ability", true, EditorStyles.foldoutHeader);
        if (showAbility) {
            EditorGUILayout.BeginVertical("box");
            DrawAbilitySection();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        // Preview Section
        showPreview = EditorGUILayout.Foldout(showPreview, "👁️ Preview", true, EditorStyles.foldoutHeader);
        if (showPreview) {
            EditorGUILayout.BeginVertical("box");
            DrawPreview();
            EditorGUILayout.EndVertical();
        }

        // Draw specialized properties if exist
        DrawSpecializedProperties();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeader() {
        EditorGUILayout.BeginVertical("box");

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        string itemName = string.IsNullOrEmpty(itemNameProp.stringValue) ? "New Item" : itemNameProp.stringValue;
        EditorGUILayout.LabelField(itemName, titleStyle);

        // Rarity color indicator
        Color rarityColor = RarityUtility.GetRarityColor((Rarity)rarityProp.enumValueIndex);
        Rect colorRect = GUILayoutUtility.GetRect(100, 3);
        EditorGUI.DrawRect(colorRect, rarityColor);

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }

    private void DrawBasicInfo() {
        EditorGUILayout.PropertyField(itemNameProp, new GUIContent("Item Name"));
        EditorGUILayout.PropertyField(itemIconProp, new GUIContent("Item Icon"));
        EditorGUILayout.PropertyField(itemTypeProp, new GUIContent("Item Type"));
        EditorGUILayout.PropertyField(rarityProp, new GUIContent("Rarity"));
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Item Lore", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(itemLoreProp, GUIContent.none);
    }

    private void DrawStackSettings() {
        EditorGUILayout.PropertyField(isStackableProp, new GUIContent("Is Stackable"));

        if (isStackableProp.boolValue) {
            EditorGUILayout.PropertyField(maxStackSizeProp, new GUIContent("Max Stack Size"));

            if (maxStackSizeProp.intValue < 1) {
                maxStackSizeProp.intValue = 1;
            }
        } else {
            EditorGUILayout.HelpBox("This item cannot be stacked.", MessageType.Info);
        }
    }

    private void DrawAbilitySection() {
        EditorGUILayout.PropertyField(hasAbilityProp, new GUIContent("Has Ability"));

        if (hasAbilityProp.boolValue) {
            EditorGUILayout.Space(5);
            EditorGUI.indentLevel++;

            SerializedProperty abilityNameProp = abilityProp.FindPropertyRelative("abilityName");
            SerializedProperty abilityIconProp = abilityProp.FindPropertyRelative("abilityIcon");
            SerializedProperty abilityDescProp = abilityProp.FindPropertyRelative("abilityDescription");
            SerializedProperty cooldownProp = abilityProp.FindPropertyRelative("cooldown");
            SerializedProperty manaCostProp = abilityProp.FindPropertyRelative("manaCost");

            EditorGUILayout.PropertyField(abilityNameProp, new GUIContent("Ability Name"));
            EditorGUILayout.PropertyField(abilityIconProp, new GUIContent("Ability Icon/GIF"));

            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Ability Description", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(abilityDescProp, GUIContent.none);

            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(cooldownProp, new GUIContent("Cooldown (sec)"));
            EditorGUILayout.PropertyField(manaCostProp, new GUIContent("Mana Cost"));
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        } else {
            EditorGUILayout.HelpBox("This item has no special ability.", MessageType.Info);
        }
    }

    private void DrawPreview() {
        InventoryItem item = (InventoryItem)target;

        EditorGUILayout.BeginHorizontal();

        // Icon preview
        if (item.ItemIcon != null) {
            GUILayout.Box(item.ItemIcon.texture, GUILayout.Width(64), GUILayout.Height(64));
        } else {
            GUILayout.Box("No Icon", GUILayout.Width(64), GUILayout.Height(64));
        }

        // Item info
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(item.ItemName, EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Type: {item.Type}");
        EditorGUILayout.LabelField($"Rarity: {item.Rarity}");

        if (item.IsStackable) {
            EditorGUILayout.LabelField($"Stackable: Yes (Max: {item.MaxStackSize})");
        } else {
            EditorGUILayout.LabelField($"Stackable: No");
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        // Ability preview
        if (item.HasAbility && item.Ability != null && item.Ability.HasAbility) {
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginVertical("helpBox");
            EditorGUILayout.LabelField("⚡ " + item.Ability.AbilityName, EditorStyles.boldLabel);

            if (item.Ability.AbilityIcon != null) {
                GUILayout.Box(item.Ability.AbilityIcon.texture, GUILayout.Width(48), GUILayout.Height(48));
            }

            EditorGUILayout.LabelField(item.Ability.AbilityDescription, EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField($"Cooldown: {item.Ability.Cooldown}s | Mana: {item.Ability.ManaCost}");
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawSpecializedProperties() {
        // Draw additional properties from derived classes
        DrawPropertiesExcluding(serializedObject,
            "m_Script",
            "itemName",
            "itemIcon",
            "itemLore",
            "itemType",
            "rarity",
            "isStackable",
            "maxStackSize",
            "hasAbility",
            "ability"
        );
    }
}
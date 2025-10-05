using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryItem), true)]
public class InventoryItemEditor : Editor {
    private SerializedProperty itemName;
    private SerializedProperty itemIcon;
    private SerializedProperty description;
    private SerializedProperty startsUnknown;
    private SerializedProperty unknownName;
    private SerializedProperty unknownIcon;
    private SerializedProperty unknownDescription;
    private SerializedProperty isStackable;
    private SerializedProperty maxStackSize;
    private SerializedProperty rarity;
    private SerializedProperty itemType;
    private SerializedProperty equipmentSlot;
    private SerializedProperty primaryAbility;

    private bool showBasicInfo = true;
    private bool showUnknownState = false;
    private bool showStackSettings = false;
    private bool showProperties = true;
    private bool showEquipmentSettings = false;
    private bool showAbilitySettings = false;

    private void OnEnable() {
        itemName = serializedObject.FindProperty("itemName");
        itemIcon = serializedObject.FindProperty("itemIcon");
        description = serializedObject.FindProperty("description");
        startsUnknown = serializedObject.FindProperty("startsUnknown");
        unknownName = serializedObject.FindProperty("unknownName");
        unknownIcon = serializedObject.FindProperty("unknownIcon");
        unknownDescription = serializedObject.FindProperty("unknownDescription");
        isStackable = serializedObject.FindProperty("isStackable");
        maxStackSize = serializedObject.FindProperty("maxStackSize");
        rarity = serializedObject.FindProperty("rarity");
        itemType = serializedObject.FindProperty("itemType");
        equipmentSlot = serializedObject.FindProperty("equipmentSlot");
        primaryAbility = serializedObject.FindProperty("primaryAbility");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.Space(10);

        // Basic Info Section
        showBasicInfo = EditorGUILayout.BeginFoldoutHeaderGroup(showBasicInfo, "📝 Basic Information");
        if (showBasicInfo) {
            EditorGUILayout.PropertyField(itemName);
            EditorGUILayout.PropertyField(itemIcon);
            EditorGUILayout.PropertyField(description);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(5);

        // Unknown State Section
        showUnknownState = EditorGUILayout.BeginFoldoutHeaderGroup(showUnknownState, "❓ Unknown State Settings");
        if (showUnknownState) {
            EditorGUILayout.PropertyField(startsUnknown);

            if (startsUnknown.boolValue) {
                EditorGUILayout.PropertyField(unknownName);
                EditorGUILayout.PropertyField(unknownIcon);
                EditorGUILayout.PropertyField(unknownDescription);

                EditorGUILayout.HelpBox("When unidentified, item will show unknown information instead of its actual properties.", MessageType.Info);
            } else {
                EditorGUILayout.HelpBox("Item will always be identified. No unknown state.", MessageType.Info);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(5);

        // Stack Settings Section
        showStackSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showStackSettings, "📦 Stack Settings");
        if (showStackSettings) {
            EditorGUILayout.PropertyField(isStackable);

            if (isStackable.boolValue) {
                EditorGUILayout.PropertyField(maxStackSize);
                EditorGUILayout.HelpBox($"Maximum stack size: {maxStackSize.intValue}", MessageType.None);
            } else {
                EditorGUILayout.HelpBox("This item cannot be stacked.", MessageType.Info);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(5);

        // Properties Section
        showProperties = EditorGUILayout.BeginFoldoutHeaderGroup(showProperties, "⚙️ Properties");
        if (showProperties) {
            EditorGUILayout.PropertyField(itemType);
            EditorGUILayout.PropertyField(rarity);

            // Show rarity color indicator
            var rarityValue = (Rarity)rarity.enumValueIndex;
            string rarityText = $"Current Rarity: {rarityValue}";
            MessageType messageType = MessageType.None;

            switch (rarityValue) {
                case Rarity.Common:
                    messageType = MessageType.Info;
                    break;
                case Rarity.Uncommon:
                    messageType = MessageType.Info;
                    break;
                case Rarity.Rare:
                    messageType = MessageType.Warning;
                    break;
                case Rarity.Epic:
                    messageType = MessageType.Warning;
                    break;
                case Rarity.Legendary:
                    messageType = MessageType.Error;
                    break;
            }

            EditorGUILayout.HelpBox(rarityText, messageType);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(5);

        // Equipment Settings (only show if item type is Equipment)
        if (itemType.enumValueIndex == (int)ItemType.Equipment) {
            showEquipmentSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showEquipmentSettings, "⚔️ Equipment Settings");
            if (showEquipmentSettings) {
                EditorGUILayout.PropertyField(equipmentSlot);

                // Show equipment slot icon
                string slotInfo = $"Equipment Slot: {((EquipmentSlot)equipmentSlot.enumValueIndex)}";
                EditorGUILayout.HelpBox(slotInfo, MessageType.Info);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(5);

            // Ability Settings
            showAbilitySettings = EditorGUILayout.BeginFoldoutHeaderGroup(showAbilitySettings, "✨ Ability Settings");
            if (showAbilitySettings) {
                DrawAbilityProperty(primaryAbility);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        // Preview Section
        EditorGUILayout.Space(10);
        DrawPreviewSection();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawAbilityProperty(SerializedProperty abilityProperty) {
        SerializedProperty abilityName = abilityProperty.FindPropertyRelative("abilityName");
        SerializedProperty abilityIcon = abilityProperty.FindPropertyRelative("abilityIcon");
        SerializedProperty abilityDescription = abilityProperty.FindPropertyRelative("abilityDescription");
        SerializedProperty cooldown = abilityProperty.FindPropertyRelative("cooldown");
        SerializedProperty manaCost = abilityProperty.FindPropertyRelative("manaCost");

        EditorGUILayout.PropertyField(abilityName, new GUIContent("Ability Name"));
        EditorGUILayout.PropertyField(abilityIcon, new GUIContent("Ability Icon"));
        EditorGUILayout.PropertyField(abilityDescription, new GUIContent("Ability Description"));
        EditorGUILayout.PropertyField(cooldown, new GUIContent("Cooldown (seconds)"));
        EditorGUILayout.PropertyField(manaCost, new GUIContent("Mana Cost"));

        bool hasAbility = !string.IsNullOrEmpty(abilityName.stringValue);
        if (hasAbility) {
            EditorGUILayout.HelpBox($"Ability: {abilityName.stringValue}\nCooldown: {cooldown.floatValue}s | Mana Cost: {manaCost.floatValue}",
                MessageType.Info);
        } else {
            EditorGUILayout.HelpBox("No ability configured.", MessageType.Info);
        }
    }

    private void DrawPreviewSection() {
        EditorGUILayout.LabelField("🎯 Item Preview", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // Basic preview
        string previewName = string.IsNullOrEmpty(itemName.stringValue) ? "Unnamed Item" : itemName.stringValue;
        string previewType = $"Type: {(ItemType)itemType.enumValueIndex}";
        string previewRarity = $"Rarity: {(Rarity)rarity.enumValueIndex}";

        EditorGUILayout.HelpBox($"{previewName}\n{previewType}\n{previewRarity}", MessageType.Info);

        // Unknown state preview if enabled
        if (startsUnknown.boolValue) {
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Unknown State Preview:", EditorStyles.miniBoldLabel);

            string unknownPreviewName = string.IsNullOrEmpty(unknownName.stringValue) ? "Unknown Item" : unknownName.stringValue;
            EditorGUILayout.HelpBox($"❓ {unknownPreviewName}\n(Unidentified State)", MessageType.Warning);
        }

        // Stack info
        if (isStackable.boolValue) {
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField($"Stackable: Yes (Max: {maxStackSize.intValue})", EditorStyles.miniLabel);
        } else {
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Stackable: No", EditorStyles.miniLabel);
        }
    }
}
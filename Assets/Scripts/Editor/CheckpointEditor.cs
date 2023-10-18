using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CheckpointSystem.Checkpoint))]
public class CheckpointEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label.text = "";
        EditorGUI.BeginProperty(position, label, property);

        Rect textPos = position;
        textPos.width -= 12;
        textPos.height = EditorGUIUtility.singleLineHeight;
        textPos.min = position.min;
        SerializedProperty textProperty = property.FindPropertyRelative("checkpoint");
        textProperty.stringValue = EditorGUI.TextField(textPos, textProperty.stringValue).ToLower().Replace(' ', '_');

        Rect boolPos = position;
        boolPos.height = EditorGUIUtility.singleLineHeight;
        boolPos.min = new Vector2(textPos.max.x + 2.5f, textPos.min.y);
        SerializedProperty boolProperty = property.FindPropertyRelative("isReached");
        boolProperty.boolValue = EditorGUI.ToggleLeft(boolPos, "", boolProperty.boolValue);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight + 1;
    }
}

[CustomPropertyDrawer(typeof(CheckpointSystem.CheckpointFlag))]
public class CheckpointFlagEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        SerializedProperty name = property.FindPropertyRelative("name");
        try
        {
            string[] checkpoints = CheckpointSystem.checkpoints.Select(x => x.checkpoint).ToArray();
            int selected = Array.IndexOf(checkpoints, name.stringValue);
            selected = EditorGUI.Popup(position, label.text, selected, checkpoints);
            name.stringValue = checkpoints[selected];

        }
        catch (ArgumentNullException)
        {
            EditorGUI.LabelField(position, label.text, $"Reconnect Checkpoint [{name.stringValue}]", EditorStyles.boldLabel);
            InspectorView inspectorView = new();
            GameObject gameManager = Resources.Load<GameObject>("GameManager");
            Transform checkpoints = gameManager.transform.Find("Checkpoints");
            CheckpointSystem checkpointSystem = checkpoints.GetComponent<CheckpointSystem>();
            inspectorView.UpdateSelection(checkpointSystem);
        }

        EditorGUI.EndProperty();
    }
}

[CustomEditor(typeof(CheckpointSystem))]
public class CheckpointSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CheckpointSystem.checkpoints = (target as CheckpointSystem).checkpointFlags;
    }
}

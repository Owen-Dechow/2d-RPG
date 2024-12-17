using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
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
        EditorGUI.PropertyField(textPos, textProperty, GUIContent.none);

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
            List<string> checkpoints = CheckpointSystem.checkpoints.Select(x => x.checkpoint).ToList();
            checkpoints.Insert(0, "_");

            int selected = Array.IndexOf(checkpoints.ToArray(), name.stringValue);
            
            if (selected == -1)
                selected = 0;

            selected = EditorGUI.Popup(position, label.text, selected, checkpoints.ToArray());
            name.stringValue = checkpoints[selected];

        }
        catch (ArgumentNullException)
        {
            CheckpointSystemEditor.Reconnect();
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
        Reconnect();
    }

    public static void Reconnect()
    {
        ReconnectCheckpoints reconnectCheckpoints = AssetDatabase.LoadAssetAtPath<ReconnectCheckpoints>("Assets/Scripts/Editor/Reconnect Checkpoints.asset");
        reconnectCheckpoints.Reconnect();
    }
}
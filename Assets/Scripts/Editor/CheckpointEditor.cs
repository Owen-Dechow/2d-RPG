using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

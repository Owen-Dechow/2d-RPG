using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Items.DataSet))]
[CustomPropertyDrawer(typeof(Magic.DataSet))]
public class ItemMagicEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label.text = "";
        EditorGUI.BeginProperty(position, label, property);

        Rect enumPos = position;
        enumPos.width /= 1.7f;
        enumPos.height = EditorGUIUtility.singleLineHeight;
        enumPos.min = position.min;
        SerializedProperty enumField = property.FindPropertyRelative("identity");
        EditorGUI.PropertyField(enumPos, enumField, new GUIContent());

        Rect scriptablePos = position;
        scriptablePos.height = EditorGUIUtility.singleLineHeight;
        scriptablePos.min = new Vector2(enumPos.max.x + 2.5f, enumPos.min.y);
        SerializedProperty sciptableProperty = property.FindPropertyRelative("scriptable");
        EditorGUI.PropertyField(scriptablePos, sciptableProperty, new GUIContent());

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight + 1;
    }
}

using Data;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Max100))]
public class Max100Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Calculate rects
        var valueRect = new Rect(position.x, position.y, position.width, position.height);

        // Draw fields - pass GUIContent.none to each so they don't get labeled
        var value = property.FindPropertyRelative("value");

        EditorGUI.BeginChangeCheck();
        var newValue = (byte)EditorGUI.IntSlider(valueRect, value.intValue, 0, 100);

        if (EditorGUI.EndChangeCheck())
        {
            value.intValue = newValue;
        }

        EditorGUI.EndProperty();
    }
}
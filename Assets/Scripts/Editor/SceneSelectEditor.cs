using System.Collections.Generic;
using Controllers;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(SceneSelect))]
public class SceneSelectEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        List<string> possibleScenes = new();

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                possibleScenes.Add(scene.path);
            }
        }

        SerializedProperty path = property.FindPropertyRelative("path");
        path.stringValue = possibleScenes[EditorGUI.Popup(position, "To Scene", possibleScenes.IndexOf(path.stringValue), possibleScenes.ToArray())];
    }
}

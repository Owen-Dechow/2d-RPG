using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GameManager.GetCleanedText((target as Door).disallowEnterText).Length == 0)
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Follow Door To Scene"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                string path = $"Assets\\Scenes\\{(target as Door).toLevel}.unity";
                EditorSceneManager.OpenScene(path);
            }
        }
    }
}
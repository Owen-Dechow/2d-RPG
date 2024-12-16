using Controllers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(DoorController))]
public class DoorEditor : Editor
{
    SerializedProperty toLevel;
    SerializedProperty doorOpening;
    SerializedProperty doorTag;
    SerializedProperty loadType;
    SerializedProperty connectedDoor;
    SerializedProperty spanPosition;
    SerializedProperty disallowEnterText;

    void OnEnable()
    {
        toLevel = serializedObject.FindProperty("toLevel");
        doorOpening = serializedObject.FindProperty("doorOpening");
        doorTag = serializedObject.FindProperty("doorTag");
        loadType = serializedObject.FindProperty("loadType");
        connectedDoor = serializedObject.FindProperty("connectedDoor");
        spanPosition = serializedObject.FindProperty("spanPosition");
        disallowEnterText = serializedObject.FindProperty("disallowEnterText");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(doorOpening);
        EditorGUILayout.PropertyField(doorTag);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(loadType);

        DoorController.LoadType doorLoadType = (DoorController.LoadType)loadType.intValue;

        if (doorLoadType == DoorController.LoadType.NoEnter)
            EditorGUILayout.PropertyField(disallowEnterText, new GUILayoutOption[0]);
        else
            EditorGUILayout.PropertyField(toLevel);

        if (doorLoadType == DoorController.LoadType.DoorToDoor)
            EditorGUILayout.PropertyField(connectedDoor);

        if (doorLoadType == DoorController.LoadType.DoorToSpawnPoint)
            EditorGUILayout.PropertyField(spanPosition);

        if (doorLoadType != DoorController.LoadType.NoEnter)
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Follow Door To Scene"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                string path = $"Assets\\Scenes\\{(target as DoorController).toLevel}.unity";
                EditorSceneManager.OpenScene(path);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
using Controllers;
using Managers;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChestController))]
public class ChestEditor : Editor
{
    SerializedProperty openSprite;
    SerializedProperty closedSprite;
    SerializedProperty contentType;

    SerializedProperty itemOption;
    SerializedProperty goldOption;

    SerializedProperty uniqueId;

    void OnEnable()
    {
        openSprite = serializedObject.FindProperty("openSprite");
        closedSprite = serializedObject.FindProperty("closedSprite");
        contentType = serializedObject.FindProperty("contentType");
        itemOption = serializedObject.FindProperty("itemOption");
        goldOption = serializedObject.FindProperty("goldOption");
        uniqueId = serializedObject.FindProperty("uniqueId");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(openSprite);
        EditorGUILayout.PropertyField(closedSprite);
        EditorGUILayout.PropertyField(contentType);

        switch ((target as ChestController).contentType)
        {
            case ChestController.ContentType.Item:
                EditorGUILayout.PropertyField(itemOption);
                break;
            case ChestController.ContentType.Gold:
                EditorGUILayout.PropertyField(goldOption);
                break;
        }

        EditorGUILayout.Space();
        GUILayout.Label("IMPORTENT: Do not forget this step!");
        if (GUILayout.Button("Generate Id"))
        {
            (target as ChestController).uniqueId = GameManager.GetRandomIntId();
        }
        EditorGUILayout.PropertyField(uniqueId);


        serializedObject.ApplyModifiedProperties();
    }

}

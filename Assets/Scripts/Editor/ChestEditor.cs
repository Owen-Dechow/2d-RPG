using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Chest))]
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

        switch ((target as Chest).contentType)
        {
            case Chest.ContentType.Item:
                EditorGUILayout.PropertyField(itemOption);
                break;
            case Chest.ContentType.Gold:
                EditorGUILayout.PropertyField(goldOption);
                break;
        }

        EditorGUILayout.Space();
        GUILayout.Label("IMPORTENT: Do not forget this step!");
        if (GUILayout.Button("Generate Id"))
        {
            (target as Chest).uniqueId = GameManager.GetRandomIntId();
        }
        EditorGUILayout.PropertyField(uniqueId);


        serializedObject.ApplyModifiedProperties();
    }

}

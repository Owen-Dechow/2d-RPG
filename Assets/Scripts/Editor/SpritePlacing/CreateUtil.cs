using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class CreateUtil
{
    public static void Place(GameObject go)
    {
        // Find location
        SceneView sceneView = SceneView.lastActiveSceneView;
        go.transform.position = sceneView ? sceneView.pivot : Vector3.zero;

        // Place object in scene
        StageUtility.PlaceGameObjectInCurrentStage(go);
        GameObjectUtility.EnsureUniqueNameForSibling(go);

        // Record undo/redo
        Undo.RegisterCreatedObjectUndo(go, $"Created Object {go.name}");
        Selection.activeObject = go;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

    }
}

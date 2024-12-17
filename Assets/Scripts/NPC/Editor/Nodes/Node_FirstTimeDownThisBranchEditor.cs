using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Node_FirstTimeDownThisBranchEditor : Editor
{
    [CustomEditor(typeof(Node_FirstTimeDownThisBranch))]
    class Node_FistTimeDownThisBranch_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            GUILayout.Label("IMPORTENT: Do not forget this step!");
            if (GUILayout.Button("Generate Id"))
            {
                Node_FirstTimeDownThisBranch node = (Node_FirstTimeDownThisBranch)target;
                node.uniqueId = GameManager.GetRandomIntId();
            }
        }
    }

}

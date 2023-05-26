using UnityEngine;
using UnityEditor;

public class Node_FirstTimeDownThisBranch : IFNode
{
    [SerializeField] int uniqueId;

    protected override bool Eval()
    {
        if (GameManager.NPCActionTreeBranchProtectors.Contains(uniqueId))
        {
            return false;
        }
        else
        {
            GameManager.NPCActionTreeBranchProtectors.Add(uniqueId);
            return true;
        }
    }

#if UNITY_EDITOR

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

#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Action Tree")]
public class BehaviorTree : ScriptableObject
{
    [HideInInspector] public Node rootNode;
    [HideInInspector] public List<Node> nodes = new();

    public IEnumerator Run()
    {
        if (rootNode == null) throw new System.NotImplementedException("No behavior implemented for tree.");
        yield return rootNode.Run();
    }

    #region Editor
#if UNITY_EDITOR

    public Node CreateNode(System.Type type, Vector2 position)
    {
        Node node = CreateInstance(type) as Node;
        node.position = position;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();

        Undo.RecordObject(this, "Action Tree (Create Node)");
        nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);
        Undo.RegisterCreatedObjectUndo(node, "Action Tree (Create Node)");
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(Node node)
    {
        Undo.RecordObject(this, "Action Tree (Create Node)");
        nodes.Remove(node);

        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child, bool trueFalsePort)
    {
        Node_OnInteract root = parent as Node_OnInteract;
        if (root != null)
        {
            Undo.RecordObject(root, "Action Tree (Create Node)");
            root.child = child;
            EditorUtility.SetDirty(root);
        }

        ActionNode action = parent as ActionNode;
        if (action != null)
        {
            Undo.RecordObject(action, "Action Tree (Create Node)");
            action.child = child;
            EditorUtility.SetDirty(action);
        }

        IFNode @if = parent as IFNode;
        if (@if != null)
        {
            Undo.RecordObject(@if, "Action Tree (Create Node)");
            if (trueFalsePort)
            {
                @if.@if = child;
            }
            else
            {
                @if.@else = child;
            }
            EditorUtility.SetDirty(@if);
        }
    }

    public void RemoveChild(Node parent, bool trueFalsePort)
    {

        Node_OnInteract root = parent as Node_OnInteract;
        if (root != null)
        {
            Undo.RecordObject(root, "Action Tree (Remove Node)");
            root.child = null;
            EditorUtility.SetDirty(root);
        }

        ActionNode action = parent as ActionNode;
        if (action != null)
        {
            Undo.RecordObject(action, "Action Tree (Remove Node)");
            action.child = null;
            EditorUtility.SetDirty(action);
        }

        IFNode @if = parent as IFNode;
        if (@if != null)
        {
            Undo.RecordObject(@if, "Action Tree (Remove Node)");
            if (trueFalsePort)
            {
                @if.@if = null;
            }
            else
            {
                @if.@else = null;
            }
            EditorUtility.SetDirty(@if);
        }
    }

    public Node GetChild(Node parent, bool trueFalsePort)
    {
        Node_OnInteract root = parent as Node_OnInteract;
        if (root != null)
        {
            return root.child;
        }

        ActionNode action = parent as ActionNode;
        if (action != null)
        {
            return action.child;
        }

        IFNode @if = parent as IFNode;
        if (@if != null)
        {
            if (trueFalsePort)
            {
                return @if.@if;
            }
            else
            {
                return @if.@else;
            }
        }

        throw new System.Exception("Proper node kind not found");
    }
#endif
    #endregion
}

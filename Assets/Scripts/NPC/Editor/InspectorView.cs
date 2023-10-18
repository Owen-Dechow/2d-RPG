using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }
    public InspectorView() { }

    Editor editor;
    public void UpdateSelection(NodeView node)
    {
        Clear();
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(node.node);
        IMGUIContainer container = new(() => { editor.OnInspectorGUI(); });
        Add(container);
    }

    public void UpdateSelection(UnityEngine.Object obj)
    {
        Clear();
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(obj);
        IMGUIContainer container = new(() => { editor.OnInspectorGUI(); });
        Add(container);
    }

    [CustomEditor(typeof(Node), editorForChildClasses:true)]
    public class NodeViewInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}

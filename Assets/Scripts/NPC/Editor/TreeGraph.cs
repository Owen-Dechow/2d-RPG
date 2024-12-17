using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NPC;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TreeGraph : GraphView
{
    BehaviorTree tree;

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        if (tree == null)
        {
            return;
        }

        List<Type> nodesInMenu = TypeCache.GetTypesDerivedFrom<ActionNode>().ToList();
        nodesInMenu.AddRange(TypeCache.GetTypesDerivedFrom<IFNode>().ToList());

        foreach (Type node in nodesInMenu)
        {

            VisualElement contentViewContainer = ElementAt(1);
            Vector3 screenMousePosition = evt.localMousePosition;
            Vector2 worldMousePosition = screenMousePosition - contentViewContainer.transform.position;
            worldMousePosition *= 1 / contentViewContainer.transform.scale.x;

            PropertyInfo menuLocationProperty = node.GetProperty("MenuLocation");
            ScriptableObject scriptableObject = ScriptableObject.CreateInstance(node);

            var val = menuLocationProperty.GetValue(scriptableObject);

            UnityEngine.Object.DestroyImmediate(scriptableObject);

            evt.menu.AppendAction(val as string, a => CreateNode(node, worldMousePosition));
        }
        evt.menu.AppendSeparator();

    }

    public new class UxmlFactory : UxmlFactory<TreeGraph, GraphView.UxmlTraits> { }
    public TreeGraph()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/NPC/Editor/ActionTreeEditor.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateGraph(tree);
        AssetDatabase.SaveAssets();
    }

    NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    public void PopulateGraph(BehaviorTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(Node_OnInteract), Vector2.zero) as Node_OnInteract;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        tree.nodes.ForEach(node => CreateNodeView(node));

        tree.nodes.ForEach(node =>
        {
            if (node is IFNode)
            {
                NodeView parentView = FindNodeView(node);

                Node ifChild = tree.GetChild(node, true);
                if (ifChild != null)
                {
                    NodeView ifView = FindNodeView(ifChild);
                    Edge ifEdge = parentView.outputTrue.ConnectTo(ifView.input);
                    AddElement(ifEdge);
                }

                Node elseChild = tree.GetChild(node, false);
                if (elseChild != null)
                {
                    NodeView elseView = FindNodeView(elseChild);
                    Edge elseEdge = parentView.outputFalse.ConnectTo(elseView.input);
                    AddElement(elseEdge);
                }
            }
            else
            {
                Node child = tree.GetChild(node, false);
                if (child != null)
                {
                    NodeView parentView = FindNodeView(node);
                    NodeView childView = FindNodeView(child);

                    Edge edge = parentView.outputTrue.ConnectTo(childView.input);
                    AddElement(edge);
                }
            }
        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
        endPort.direction != startPort.direction
        && endPort.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        graphViewChange.elementsToRemove?.ForEach(element =>
        {
            if (element is NodeView nodeView)
            {
                if (nodeView.node is not Node_OnInteract)
                {
                    tree.DeleteNode(nodeView.node);
                }
            }

            if (element is Edge edge)
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;

                tree.RemoveChild(parentView.node, childView.node);
            }
        });

        graphViewChange.edgesToCreate?.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;

                if (parentView.node is IFNode)
                {
                    if (edge.output == parentView.outputTrue)
                    {
                        tree.AddChild(parentView.node, childView.node, true);
                    }
                    else
                    {
                        tree.AddChild(parentView.node, childView.node, false);
                    }
                }
                else
                {
                    tree.AddChild(parentView.node, childView.node, false);
                }
            });

        OnUndoRedo();
        return graphViewChange;
    }

    void CreateNodeView(Node node)
    {
        NodeView nodeView = new(node);
        AddElement(nodeView);
    }

    void CreateNode(System.Type type, Vector2 mousePosition)
    {
        Node node = tree.CreateNode(type, mousePosition);
        CreateNodeView(node);
    }
}

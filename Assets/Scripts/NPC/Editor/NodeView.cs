using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Node node;
    public Port input;
    public Port outputTrue;
    public Port outputFalse;

    public NodeView(Node node) : base("Assets/Scripts/NPC/Editor/NodeView.uxml")
    {
        this.node = node;
        viewDataKey = node.guid;

        style.left = node.position.x;
        style.top = node.position.y;
        title = node.name.Replace("Node_", "");
        AddToClassList("t-" + node.ClassName);

        CreateInputPorts();
        CreateOutputPorts();
    }

    private void CreateInputPorts()
    {
        if (node is ActionNode || node is IFNode)
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        }

        if (input != null)
        {
            input.portName = "";
            inputContainer.Add(input);
        }

        InspectorView inspectorView = new();
        inspectorView.UpdateSelection(this);
        inspectorView.AddToClassList("inspector-view");
        contentContainer.Q("inspector").Add(inspectorView);
    }

    private void CreateOutputPorts()
    {
        if (node is IFNode)
        {
            outputTrue = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputFalse = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

            outputTrue.portName = "True";
            outputFalse.portName = "False";
            outputContainer.Add(outputTrue);
            outputContainer.Add(outputFalse);
        }
        else
        {
            outputTrue = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputTrue.portName = "";
            outputContainer.Add(outputTrue);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(node, "Action Tree (Set Position)");
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
        EditorUtility.SetDirty(node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
    }
}

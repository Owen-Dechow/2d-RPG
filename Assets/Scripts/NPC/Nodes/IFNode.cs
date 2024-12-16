using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

public abstract class IFNode : Node
{
    [HideInInspector] public Node @if;
    [HideInInspector] public Node @else;
    public override string ClassName => "if";
    public override abstract string MenuLocation { get; }

    public override IEnumerator Run(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return new WaitForEndOfFrame();
        if (Evaluate(npc, treeData))
        {
            if (@if != null) yield return @if.Run(npc, treeData);
        }
        else
        {
            if (@else != null) yield return @else.Run(npc, treeData);
        }
    }

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return null;
    }

    protected abstract bool Evaluate(Npc npc, BehaviorTree.TreeData treeData);
}

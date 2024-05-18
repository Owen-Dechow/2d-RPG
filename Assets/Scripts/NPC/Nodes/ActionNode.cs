using System.Collections;
using UnityEngine;

public abstract class ActionNode : Node
{
    [HideInInspector] public Node child;
    public override string ClassName => "action";
    public abstract override string MenuLocation { get; }

    public override IEnumerator Run(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return Execute(npc, treeData);
        yield return new WaitForEndOfFrame();
        if (child != null)
        {
            yield return child.Run(npc, treeData);
        }
    }
}

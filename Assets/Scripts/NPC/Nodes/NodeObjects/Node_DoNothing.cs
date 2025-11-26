using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;

public class Node_DoNothing : ActionNode
{
    public override string ClassName => "other";

    public override string MenuLocation => "Debug/Do Nothing";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return null;
    }
}

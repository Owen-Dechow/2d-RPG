using System.Collections;
using UnityEngine;

public class Node_TakeItem : ActionNode
{
    [SerializeField] GameItems.Options item;

    public override string MenuLocation => "Actions/Take Player Item";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        if (Player.Items.Contains(item))
        {
            Player.Items.Remove(item);
        }
        yield return null;
    }
}

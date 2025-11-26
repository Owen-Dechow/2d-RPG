using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Managers;
using UnityEngine;

public class Node_Say : ActionNode
{
    [SerializeField] readonly string text;

    public override string MenuLocation => "Menus/Text Only";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return GameUIManager.TypeOut(text);
    }
}

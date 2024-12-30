using System.Collections;
using Controllers;
using Data;
using Managers;
using NPC;
using UnityEngine;

public class Node_TeachPower : ActionNode
{
    [SerializeField] MagicScriptable magic;

    public override string MenuLocation => "Actions/Teach Player Power";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return GameUIManager.TypeOut($"{PlayerManager.Name} learned the power of {GameUIManager.GetCleanedText(magic.ToString())}!");
        PlayerManager.Magic.Add(magic);
        PlayerManager.Magic.Sort();
    }
}
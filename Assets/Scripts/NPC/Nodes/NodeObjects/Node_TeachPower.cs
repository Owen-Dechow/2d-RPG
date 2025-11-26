using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Data;
using Managers;
using UnityEngine;

public class Node_TeachPower : ActionNode
{
    [SerializeField] readonly MagicScriptable magic;

    public override string MenuLocation => "Actions/Teach Player Power";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return GameUIManager.TypeOut($"{PlayerManager.Name} learned the power of {GameUIManager.GetCleanedText(magic.ToString())}!");
        PlayerManager.Magic.Add(magic);
        PlayerManager.Magic.Sort();
    }
}

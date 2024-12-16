using System.Collections;
using Controllers;
using UnityEngine;

public class Node_TeachPower : ActionNode
{
    [SerializeField] GameMagic.Options magic;

    public override string MenuLocation => "Actions/Teach Player Power";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return GameUI.TypeOut($"{Player.Name} learned the power of {GameManager.GetCleanedText(magic.ToString())}!");
        Player.Magic.Add(magic);
        Player.Magic.Sort();
    }
}
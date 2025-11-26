using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Managers;
using UnityEngine;

public class Node_AnswerIndexIs : IFNode
{
    [SerializeField] readonly int[] answerIndexes;

    public override string MenuLocation => "Control/Answer Index is";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        foreach (int i in answerIndexes)
        {
            if (GameUIManager.AnswerIndex == i)
                return true;
        }

        return false;
    }
}

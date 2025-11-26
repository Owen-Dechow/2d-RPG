using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Managers;
using UnityEngine;

public class Node_AnswerIs : IFNode
{
    [SerializeField] readonly string[] answers;

    public override string MenuLocation => "Control/Answer Is";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        foreach (string answer in answers)
        {
            if (GameUIManager.Answer == GameUIManager.GetCleanedText(answer))
                return true;
        }

        return false;
    }
}

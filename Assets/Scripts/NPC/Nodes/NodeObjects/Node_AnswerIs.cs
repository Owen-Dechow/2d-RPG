using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_AnswerIs : IFNode
{
    [SerializeField] string[] answers;

    public override string MenuLocation => "Control/Answer Is";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        foreach (string answer in answers)
        {
            if (GameManager.Answer == GameManager.GetCleanedText(answer))
                return true;
        }
        return false;
    }
}

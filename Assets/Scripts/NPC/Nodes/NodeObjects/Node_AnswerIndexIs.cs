using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

public class Node_AnswerIndexIs : IFNode
{
    [SerializeField] int[] answerIndexes;

    public override string MenuLocation => "Control/Answer Index is";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        foreach (int i in answerIndexes)
        {
            if( GameManager.AnswerIndex == i)
                return true;
        }
        return false;
    }
}

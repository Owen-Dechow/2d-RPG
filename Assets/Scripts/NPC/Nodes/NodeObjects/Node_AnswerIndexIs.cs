using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_AnswerIndexIs : IFNode
{
    [SerializeField] int[] answerIndexes;
    protected override bool Evaluate(Npc npc)
    {
        foreach (int i in answerIndexes)
        {
            if( GameManager.AnswerIndex == i)
                return true;
        }
        return false;
    }
}

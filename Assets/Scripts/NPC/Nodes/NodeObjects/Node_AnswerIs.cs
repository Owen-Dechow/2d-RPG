using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_AnswerIs : IFNode
{
    [SerializeField] string[] answers;
    protected override bool Eval()
    {
        foreach (string answer in answers)
        {
            if (GameManager.Answer == GameManager.GetCleanedText(answer))
                return true;
        }
        return false;
    }
}

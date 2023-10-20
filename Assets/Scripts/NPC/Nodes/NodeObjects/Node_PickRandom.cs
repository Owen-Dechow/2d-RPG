using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_PickRandom : IFNode
{
    protected override bool Evaluate(Npc npc)
    {
        return Random.Range(0, 2) == 1;
    }
}
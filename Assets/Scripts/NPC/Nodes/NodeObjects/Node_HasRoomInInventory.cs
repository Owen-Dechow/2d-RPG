using System.Collections;
using UnityEngine;

public class Node_HasRoomInInventory : IFNode
{

    protected override bool Evaluate(Npc npc)
    {
        return Player.HasRoomInInventory();
    }
}

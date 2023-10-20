using System.Collections;
using UnityEngine;

public class Node_GiveItem : ActionNode
{
    [SerializeField] GameItems.Options item;
    protected override IEnumerator Execute(Npc npc)
    {
        if (Player.HasRoomInInventory())
            Player.AddItemToInventory(item);

        yield return null;
    }
}

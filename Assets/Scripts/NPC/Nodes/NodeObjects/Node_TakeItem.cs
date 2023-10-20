using System.Collections;
using UnityEngine;

public class Node_TakeItem : ActionNode
{
    [SerializeField] GameItems.Options item;
    protected override IEnumerator Execute(Npc npc)
    {
        if (Player.Items.Contains(item))
        {
            Player.Items.Remove(item);
        }
        yield return null;
    }
}

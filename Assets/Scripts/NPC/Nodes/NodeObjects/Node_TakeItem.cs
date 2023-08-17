using System.Collections;
using UnityEngine;

public class Node_TakeItem : ActionNode
{
    [SerializeField] GameItems.Options item;
    protected override IEnumerator Execute()
    {
        if (GameManager.player.Items.Contains(item))
        {
            GameManager.player.Items.Remove(item);
        }
        yield return null;
    }
}

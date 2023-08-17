using System.Collections;
using UnityEngine;

public class Node_GiveItem : ActionNode
{
    [SerializeField] GameItems.Options item;
    protected override IEnumerator Execute()
    {
        GameManager.player.Items.Add(item);
        yield return null;
    }
}

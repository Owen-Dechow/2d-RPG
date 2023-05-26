using System.Collections;
using UnityEngine;

public class Node_GiveItem : ActionNode
{
    [SerializeField] Items.Options item;
    protected override IEnumerator Execute()
    {
        GameManager.player.Items.Add(item);
        yield return null;
    }
}

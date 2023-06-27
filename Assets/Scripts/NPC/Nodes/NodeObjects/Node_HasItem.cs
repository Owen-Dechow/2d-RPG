using System.Collections;
using UnityEngine;


public class Node_HasItem : IFNode
{
    [SerializeField] Items.Options item;

    protected override bool Evaluate()
    {
        return GameManager.player.Items.Contains(item);
    }
}

using System.Collections;
using UnityEngine;

public class Node_TeachPower : ActionNode
{
    [SerializeField] GameMagic.Options magic;

    protected override IEnumerator Execute(Npc npc)
    {
        yield return GameUI.TypeOut($"{Player.Name} learned the power of {GameManager.GetCleanedText(magic.ToString())}!");
        Player.Magic.Add(magic);
        Player.Magic.Sort();
    }
}
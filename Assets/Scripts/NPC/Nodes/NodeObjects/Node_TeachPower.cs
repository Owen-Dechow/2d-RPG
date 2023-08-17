using System.Collections;
using UnityEngine;

public class Node_TeachPower : ActionNode
{
    [SerializeField] GameMagic.Options magic;

    protected override IEnumerator Execute()
    {
        yield return GameUI.TypeOut($"{GameManager.player.Name} learned the power of {GameManager.GetCleanedText(magic.ToString())}!");
        GameManager.player.playerBattleUnit.data.magicOptionsForUnit.Add(magic);
        GameManager.player.playerBattleUnit.data.magicOptionsForUnit.Sort();
    }
}
using System.Collections;
using UnityEngine;

public class Node_JoinPlayer : ActionNode
{
    [SerializeField] Sprite sprite;
    [SerializeField] BattleUnit.BattleUnitData battleUnit;

    protected override IEnumerator Execute()
    {
        yield return new WaitForEndOfFrame();
        GameManager.player.AddBattleUnit(battleUnit, sprite);
    }
}

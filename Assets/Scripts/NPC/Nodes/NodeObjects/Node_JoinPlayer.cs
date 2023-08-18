using System.Collections;
using UnityEngine;

public class Node_JoinPlayer : ActionNode
{
    [SerializeField] Sprite sprite;
    [SerializeField] BattleUnit.BattleUnitData battleUnit;
    [SerializeField] string joinedPlayerCheckpoint;

    protected override IEnumerator Execute()
    {
        yield return GameUI.TypeOut($"{battleUnit.title} joined {Player.Name}");
        Player.AddBattleUnit(battleUnit, sprite);
        CheckpointSystem.SetCheckpoint(joinedPlayerCheckpoint);
    }
}

using System.Collections;
using Battle;
using Controllers;
using Managers;
using NPC;
using UnityEngine;

public class Node_JoinPlayer : ActionNode
{
    [SerializeField] Sprite sprite;
    [SerializeField] BattleUnit.BattleUnitData battleUnit;
    [SerializeField] string joinedPlayerCheckpoint;

    public override string MenuLocation => "Actions/NPC Join";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return GameUI.TypeOut($"{battleUnit.title} joined {Player.Name}");
        Player.AddBattleUnit(battleUnit, sprite);
        CheckpointSystem.SetCheckpoint(joinedPlayerCheckpoint);
    }
}

using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Battle;
using Managers;
using UnityEngine;

public class Node_JoinPlayer : ActionNode
{
    [SerializeField] readonly Sprite sprite;
    [SerializeField] BattleUnit.BattleUnitData battleUnit;
    [SerializeField] readonly string joinedPlayerCheckpoint;

    public override string MenuLocation => "Actions/NPC Join";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return GameUIManager.TypeOut($"{battleUnit.title} joined {PlayerManager.Name}");
        battleUnit.spriteId = UnserializableDataIdMap.GetSpriteID(sprite);
        PlayerManager.AddBattleUnit(battleUnit);
        CheckpointSystem.SetCheckpoint(joinedPlayerCheckpoint);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using static Door;

public class Player : MonoBehaviour
{
    static Player i;

    public static string Name => i.playerBattleUnit.data.title; 
    public static int Gold { get => i.playerBattleUnit.data.gold; set => i.playerBattleUnit.data.gold = value; }
    public static List<GameItems.Options> Items => i.playerBattleUnit.data.itemOptionsForUnit;
    public static List<GameMagic.Options> Magic => i.playerBattleUnit.data.magicOptionsForUnit;
    public static List<BattleUnit> ComradeBattleUnits => i.comradeBattleUnits;

    BattleUnit playerBattleUnit;
    List<BattleUnit> comradeBattleUnits;

    private void Start()
    {
        i = this;
        playerBattleUnit = GetComponent<BattleUnit>();
        comradeBattleUnits = new();
    }

    public static void UpdateToLevel1()
    {
        LevelUp.LevelData levelData = LevelUp.GetDataForLevelStatic(1);
        i.playerBattleUnit.data.level = levelData.level;
        i.playerBattleUnit.data.maxLife = levelData.life;
        i.playerBattleUnit.data.maxMagic = levelData.magic;
        i.playerBattleUnit.data.attack = levelData.attack;
        i.playerBattleUnit.data.defense = levelData.defense;

        i.playerBattleUnit.data.life = levelData.life;
        i.playerBattleUnit.data.magic = levelData.magic;
    }

    public static void AddBattleUnit(BattleUnit.BattleUnitData battleUnit, Sprite sprite)
    {
        GameObject go = new(battleUnit.title);
        go.transform.parent = i.transform;

        BattleUnit bu = go.AddComponent<BattleUnit>();
        bu.data = battleUnit;
        bu.sprite = sprite;
        i.comradeBattleUnits.Add(bu);
    }

    public static BattleUnit[] GetBattleUnits()
    {
        List<BattleUnit> playerBattleUnits = new()
        {
            i.playerBattleUnit
        };
        playerBattleUnits.AddRange(i.comradeBattleUnits);

        return playerBattleUnits.ToArray();
    }

    public static BattleUnit.BattleUnitData GetBattleUnitData() => i.playerBattleUnit.data;

    public static void SetBattleUnitData(BattleUnit.BattleUnitData battleUnitData)
    {
        i.playerBattleUnit.data = battleUnitData;
    }

    public static void MoveToDoor(Vector3 position, Door.DoorOpenDir doorOpening)
    {
        AnimPlus.Direction direction = doorOpening switch
        {
            DoorOpenDir.top => AnimPlus.Direction.up,
            DoorOpenDir.bottom => AnimPlus.Direction.down,
            DoorOpenDir.left => AnimPlus.Direction.left,
            DoorOpenDir.right => AnimPlus.Direction.right,
            _ => throw new System.NotImplementedException(),
        };

        PlayerController.playerController.GetComponent<AnimPlus>().SetDirection(direction);

        PlayerController.playerController.transform.position = position;
        Vector2 delta = new Vector2(
            Mathf.Cos((int)doorOpening),
            Mathf.Sin((int)doorOpening)
            ) * 0.16f;
        PlayerController.playerController.transform.Translate(delta.x + 0.08f, delta.y, 0);
    }

    public static void SetName(string name)
    {
        i.playerBattleUnit.data.title = name;
    }
}

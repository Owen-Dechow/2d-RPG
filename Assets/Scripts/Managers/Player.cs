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

    public static void SetName(string name)
    {
        i.playerBattleUnit.data.title = name;
    }

    public static bool HasRoomInInventory()
    {
        foreach (BattleUnit unit in GetBattleUnits())
        {
            if (unit.data.itemOptionsForUnit.Count < BattleUnit.BattleUnitData.MaxItems)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AddItemToInventory(GameItems.Options item)
    {
        foreach (BattleUnit unit in GetBattleUnits())
        {
            if (unit.data.itemOptionsForUnit.Count < BattleUnit.BattleUnitData.MaxItems)
            {
                unit.data.itemOptionsForUnit.Add(item);
                return true;
            }
        }

        return false;
    }
}

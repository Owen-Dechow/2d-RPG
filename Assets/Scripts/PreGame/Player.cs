using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerController playerObject;
    [HideInInspector] public BattleUnit playerBattleUnit;
    [HideInInspector] public List<BattleUnit> playerComradeBattleUnits;

    public List<GameItems.Options> Items { get => playerBattleUnit.data.items; }
    public string Name { get => playerBattleUnit.data.title; }

    private void Start()
    {
        playerBattleUnit = GetComponent<BattleUnit>();
    }

    public void UpdateToLevel1()
    {
        LevelUp.LevelData levelData = LevelUp.GetDataForLevelStatic(1);
        playerBattleUnit.data.level = levelData.level;
        playerBattleUnit.data.maxLife = levelData.life;
        playerBattleUnit.data.maxMagic = levelData.magic;
        playerBattleUnit.data.attack = levelData.attack;
        playerBattleUnit.data.defense = levelData.defense;

        playerBattleUnit.data.life = levelData.life;
        playerBattleUnit.data.magic = levelData.magic;
    }

    public void AddBattleUnit(BattleUnit.BattleUnitData battleUnit, Sprite sprite)
    {
        BattleUnit bu = gameObject.AddComponent<BattleUnit>();
        bu.data = battleUnit;
        bu.sprite = sprite;
        playerComradeBattleUnits.Add(bu);
    }

    public BattleUnit[] GetBattleUnits()
    {
        List<BattleUnit> playerBattleUnits = new()
        {
            playerBattleUnit
        };
        playerBattleUnits.AddRange(playerComradeBattleUnits);

        return playerBattleUnits.ToArray();
    }
}

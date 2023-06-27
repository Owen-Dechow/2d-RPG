using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerController playerObject;
    [HideInInspector] public BattleUnit playerBattleUnit;
    public List<Items.Options> Items { get => playerBattleUnit.data.items; }
    public string Name { get => playerBattleUnit.data.title; }

    private void Awake()
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

    public BattleUnit[] GetBattleUnits()
    {
        return new BattleUnit[] { playerBattleUnit };
    }
}

using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public Sprite sprite;
    public string title;
    public UnitRefexive refexive;
    public int life;
    public int maxLife;
    public int magic;
    public int maxMagic;
    public byte escapePercentageAllowed;
    public List<Magic.Options> magicOptionsForUnit;
    public List<Items.Options> items;
    public Badges.Equip badges;
    public EnemyAI enemyAI;

    [HideInInspector] public bool onDefence;
    [HideInInspector] public GameObject stationGO;
    [HideInInspector] public SpriteRenderer spr;

    public bool Alive { get => life > 0; }

    public enum TurnOptions
    {
        Attack,
        Defend,
        Item,
        Magic,
        Run
    }

    public enum UnitRefexive
    {
        himself,
        herself,
    }

    private void OnValidate()
    {
        escapePercentageAllowed = (byte)Mathf.Clamp(escapePercentageAllowed, 0, 100);
        if (enemyAI != null)
        {
            enemyAI.ClampData();
            if (enemyAI.GetTotal() == 100) enemyAI.dataStatus = EnemyAI.DataStatus.Valid;
            else
            {
                if (enemyAI.dataStatus == EnemyAI.DataStatus.Valid && enemyAI.lastCheck == EnemyAI.DataStatus.Invalid)
                {
                    enemyAI.CorrectData();
                }
                else enemyAI.dataStatus = EnemyAI.DataStatus.Invalid;
            }
            enemyAI.lastCheck = enemyAI.dataStatus;
        }
    }

    public BattleUnit Copy()
    {
        BattleUnit newBattleUnit = stationGO.AddComponent<BattleUnit>();
        newBattleUnit.sprite = sprite;
        newBattleUnit.title = title;
        newBattleUnit.life = life;
        newBattleUnit.maxLife = maxLife;
        newBattleUnit.magic = magic;
        newBattleUnit.maxMagic = maxMagic;
        newBattleUnit.badges = badges;
        newBattleUnit.escapePercentageAllowed = escapePercentageAllowed;
        newBattleUnit.magicOptionsForUnit = magicOptionsForUnit;
        newBattleUnit.stationGO = stationGO;
        newBattleUnit.spr = spr;
        newBattleUnit.enemyAI = enemyAI;
        newBattleUnit.items = items;

        return newBattleUnit;
    }
}

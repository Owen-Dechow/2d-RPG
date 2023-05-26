using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerController playerObject;
    [HideInInspector] public BattleUnit playerBattleUnit;
    public Badges.Inventory badgeInventory;
    public List<Items.Options> Items { get => playerBattleUnit.items; }
    public string Name { get => playerBattleUnit.title; }

    private void Start()
    {
        playerBattleUnit = GetComponent<BattleUnit>();
    }

    public BattleUnit[] GetBattleUnits()
    {
        return new BattleUnit[] { playerBattleUnit };
    }
}

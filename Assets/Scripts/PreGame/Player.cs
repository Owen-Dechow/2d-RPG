using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerController playerObject;
    [HideInInspector] public BattleUnit playerBattleUnit;
    public List<Items.Options> Items { get => playerBattleUnit.data.items; }
    public string Name { get => playerBattleUnit.data.title; }

    private void Start()
    {
        playerBattleUnit = GetComponent<BattleUnit>();
    }

    public BattleUnit[] GetBattleUnits()
    {
        return new BattleUnit[] { playerBattleUnit };
    }
}

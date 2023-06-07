using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    [System.Serializable]
    public struct BattleUnitData
    {
        [Header("Appearance Options")]
        public string title;
        public UnitReflexive reflexive;

        [Header("Stat Options")]
        public int life;
        public int maxLife;
        public int magic;
        public int maxMagic;
        public int defense;
        public int attack;

        [Header("Items/Magic")]
        public List<Magic.Options> magicOptionsForUnit;
        public List<Items.Options> items;

        [Header("Player Only")]
        public int exp;
        public int level;

        [Header("Enemy Only")]
        public int expAward;
        public EnemyAI enemyAI;

        [Header("Static")]
        public byte escapePercentageAllowed;


        public readonly bool Alive { get => life > 0; }
    }

    public enum TurnOptions
    {
        Attack,
        Defend,
        Item,
        Magic,
        Run
    }

    public Sprite sprite;
    public BattleUnitData data;
    [HideInInspector] public bool onDefense;
    [HideInInspector] public GameObject stationGO;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    public enum UnitReflexive
    {
        himself,
        herself,
    }

    #region AI_VALIDATOR
#if UNITY_EDITOR
    private void OnValidate()
    {
        data.escapePercentageAllowed = (byte)Mathf.Clamp(data.escapePercentageAllowed, 0, 100);
        if (data.enemyAI != null)
        {
            data.enemyAI.ClampData();
            if (data.enemyAI.GetTotal() == 100) data.enemyAI.dataStatus = EnemyAI.DataStatus.Valid;
            else
            {
                if (data.enemyAI.dataStatus == EnemyAI.DataStatus.Valid && data.enemyAI.lastCheck == EnemyAI.DataStatus.Invalid)
                {
                    data.enemyAI.CorrectData();
                }
                else data.enemyAI.dataStatus = EnemyAI.DataStatus.Invalid;
            }
            data.enemyAI.lastCheck = data.enemyAI.dataStatus;
        }
    }
#endif
    #endregion

    public BattleUnit CopyAtStationGO()
    {
        BattleUnit newBattleUnit = stationGO.AddComponent<BattleUnit>();
        newBattleUnit.data = data;
        newBattleUnit.stationGO = stationGO;
        newBattleUnit.spriteRenderer = spriteRenderer;
        newBattleUnit.onDefense = onDefense;
        newBattleUnit.sprite = sprite;

        return newBattleUnit;
    }

    public int GetAttack()
    {
        int maxAttack = data.attack;
        int minAttack = Mathf.CeilToInt(maxAttack * 0.75f);
        int attack = Random.Range(minAttack, maxAttack + 1);
        return Mathf.Max(attack, 1);
    }

    public int GetDefenseChange(int power, bool onDefense)
    {
        int maxTake = Mathf.CeilToInt(data.defense * 0.75f);
        int minTake = Mathf.FloorToInt(maxTake * 0.5f);
        int newAttack = power - Random.Range(minTake, maxTake + 1);
        
        if (onDefense)
        {
            newAttack = Mathf.CeilToInt(newAttack * 0.6f);
        }

        return Mathf.Max(newAttack, 1);
    }
}

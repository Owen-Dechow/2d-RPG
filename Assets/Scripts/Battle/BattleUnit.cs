using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Battle
{
    public class BattleUnit : MonoBehaviour
    {
        [System.Serializable]
        public struct BattleUnitData
        {
            [Header("Appearance Options")]
            public string title;
            public UnitSex sex;

            [Header("Stat Options")]
            public int life;
            public int maxLife;
            public int magic;
            public int maxMagic;
            public int defense;
            public int attack;

            [Header("Items/Magic")] public const int MaxItems = 30;

            public List<GameMagic.Options> magicOptionsForUnit;
            public List<GameItems.Options> itemOptionsForUnit;

            [Header("Player Only")]
            public int exp;
            public int level;
            public int gold;

            [Header("Enemy Only")]
            public int expAward;
            public int goldAward;
            public EnemyAI enemyAI;

            [Header("Static")]
            public byte escapePercentageAllowed;

            public readonly bool Alive => life > 0;
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
        [FormerlySerializedAs("stationGO")] [HideInInspector] public GameObject stationGo;
        [HideInInspector] public SpriteRenderer spriteRenderer;

        public enum UnitSex
        {
            Male,
            Female,
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

        public BattleUnit CopyAtStationGo()
        {
            BattleUnit newBattleUnit = stationGo.AddComponent<BattleUnit>();
            newBattleUnit.data = data;
            newBattleUnit.stationGo = stationGo;
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

        public int GetDefenseChange(int power)
        {
            int maxTake = Mathf.CeilToInt(data.defense * 1);
            int minTake = Mathf.FloorToInt(maxTake * 0.5f);
            int newAttack = power - Random.Range(minTake, maxTake + 1);

            if (onDefense)
            {
                newAttack = Mathf.CeilToInt(newAttack * 0.75f);
            }

            return Mathf.Max(newAttack, 1);
        }

        public IEnumerator LevelUpUnit()
        {
            LevelUp.LevelData levelData = LevelUp.GetDataForLevelStatic(data.level + 1);
            if (data.exp >= levelData.expNeeded)
            {
                data.exp -= levelData.expNeeded;

                yield return GameUI.TypeOut($"{data.title} reached level {levelData.level}!");
                yield return GameUI.TypeOut($"Max life is now {levelData.life}! ({levelData.life - data.maxLife:+#;-#;+0})");
                yield return GameUI.TypeOut($"Max magic is now {levelData.magic}! ({levelData.magic - data.maxMagic:+#;-#;+0})");
                yield return GameUI.TypeOut($"Attack is now {levelData.attack}! ({levelData.attack - data.attack:+#;-#;+0})");
                yield return GameUI.TypeOut($"Defense is now {levelData.defense}! ({levelData.defense - data.defense:+#;-#;+0})");

                data.level = levelData.level;
                data.maxLife = levelData.life;
                data.maxMagic = levelData.magic;
                data.attack = levelData.attack;
                data.defense = levelData.defense;

                yield return LevelUpUnit();
            }
        }
    }
}

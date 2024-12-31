using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Battle
{
    public class BattleUnit : MonoBehaviour
    {
        [System.Serializable]
        public struct BattleUnitData
        {
            public static int MaxItems => 20;

            [Header("Appearance Options")] public string title;
            public UnitSex sex;

            [Header("Stat Options")] public Max100 life;
            public int maxBadges;
            public List<bool> badgesEquipped;
            public Max100 magic;
            public int gold;

            [Header("Enemy Only")] public EnemyAI enemyAI;

            [Header("Static")] public Max100 escapePercentageAllowed;

            [Header("Save Only Data")] public short spriteId;
            public short[] magicOptionsForUnitIds;
            public short[] itemsIds;
            public short[] badgesOwnedIds;

            public readonly bool Alive => life > 0;

            public void SyncSelf(BattleUnit unit)
            {
                spriteId = UnserializableDataIdMap.GetSpriteID(unit.sprite);
                magicOptionsForUnitIds = unit.magicOptionsForUnit.Select(UnserializableDataIdMap.GetMagicID).ToArray();
                itemsIds = unit.itemOptionsForUnit.Select(UnserializableDataIdMap.GetItemID).ToArray();
                badgesOwnedIds = unit.badgesForUnit.Select(UnserializableDataIdMap.GetBadgeID).ToArray();
            }

            public void SyncUnit(BattleUnit unit)
            {
                unit.sprite = UnserializableDataIdMap.GetSprite(spriteId);
                unit.magicOptionsForUnit = magicOptionsForUnitIds.Select(UnserializableDataIdMap.GetMagic).ToList();
                unit.itemOptionsForUnit = itemsIds.Select(UnserializableDataIdMap.GetItem).ToList();
                unit.badgesForUnit = badgesOwnedIds.Select(UnserializableDataIdMap.GetBadge).ToList();
            }
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

        [FormerlySerializedAs("stationGO")] [HideInInspector]
        public GameObject stationGo;

        [HideInInspector] public SpriteRenderer spriteRenderer;

        [Header("Items/Magic/Badges")] public List<MagicScriptable> magicOptionsForUnit;
        public List<ItemScriptable> itemOptionsForUnit;
        public List<BadgesScriptable> badgesForUnit;

        public Max100 GetMaxHealth() => (Max100)Mathf.Clamp(badgesForUnit
            .Where((item, idx) => data.badgesEquipped[idx] && item.Type == BadgesScriptable.BadgeType.Health)
            .Sum(item => item.Power), 1, 100);

        private Max100 GetAttackTrait() =>
            Mathf.Clamp(
                badgesForUnit
                    .Where((item, idx) => data.badgesEquipped[idx] && item.Type == BadgesScriptable.BadgeType.Attack)
                    .Sum(item => item.Power), 1, 100);

        private Max100 GetDefenseTrait() =>
            Mathf.Clamp(
                badgesForUnit
                    .Where((item, idx) => data.badgesEquipped[idx] && item.Type == BadgesScriptable.BadgeType.Defense)
                    .Sum(item => item.Power), 1, 100);

        public Max100 GetMaxMagic() =>
            Mathf.Clamp(
                badgesForUnit
                    .Where((item, idx) => data.badgesEquipped[idx] && item.Type == BadgesScriptable.BadgeType.Magic)
                    .Sum(item => item.Power), 0, 100);

        public enum UnitSex
        {
            Male,
            Female,
        }

        private void OnValidate()
        {
            if (data.enemyAI != null)
            {
                data.enemyAI.ClampData();
                if (data.enemyAI.GetTotal() == 100) data.enemyAI.dataStatus = EnemyAI.DataStatus.Valid;
                else
                {
                    if (data.enemyAI.dataStatus == EnemyAI.DataStatus.Valid &&
                        data.enemyAI.lastCheck == EnemyAI.DataStatus.Invalid)
                    {
                        data.enemyAI.CorrectData();
                    }
                    else data.enemyAI.dataStatus = EnemyAI.DataStatus.Invalid;
                }

                data.enemyAI.lastCheck = data.enemyAI.dataStatus;
            }
        }

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

        public Max100 GetAttack()
        {
            Max100 maxAttack = GetAttackTrait();
            return maxAttack * Random.Range(0.75f, 1);
        }

        public Max100 GetDefenseChange(Max100 attack)
        {
            Max100 maxTake = GetDefenseTrait();
            Max100 newAttack = maxTake * Random.Range(0.75f, 0);

            if (onDefense)
                newAttack *= 0.5f;

            return newAttack;
        }

        public BattleUnitData GetSyncedData()
        {
            data.SyncSelf(this);
            return data;
        }
    }
}
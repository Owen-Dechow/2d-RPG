using System.Collections.Generic;
using System.Linq;
using Battle;
using Controllers;
using Data;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : MonoBehaviour
    {
        private static PlayerManager _i;

        private PlayerController.PlacementSettings placementSettings;

        public static PlayerController.PlacementSettings PlacementSettings
        {
            get => _i.placementSettings;
            set => _i.placementSettings = value;
        }

        private BattleUnit playerBattleUnit;
        public static string Name => _i.playerBattleUnit.data.title;

        public static int Gold
        {
            get => _i.playerBattleUnit.data.gold;
            set => _i.playerBattleUnit.data.gold = value;
        }

        public static List<ItemScriptable> Items => _i.playerBattleUnit.data.itemOptionsForUnit;
        public static List<MagicScriptable> Magic => _i.playerBattleUnit.data.magicOptionsForUnit;

        private List<BattleUnit> comradeBattleUnits;
        public static List<BattleUnit> ComradeBattleUnits => _i.comradeBattleUnits;

        private void Start()
        {
            _i = this;
            playerBattleUnit = GetComponent<BattleUnit>();
            comradeBattleUnits = new List<BattleUnit>();
        }

        public static void UpdateToLevel1()
        {
            LevelUp.LevelData levelData = LevelUp.GetDataForLevelStatic(1);
            _i.playerBattleUnit.data.level = levelData.level;
            _i.playerBattleUnit.data.maxLife = levelData.life;
            _i.playerBattleUnit.data.maxMagic = levelData.magic;
            _i.playerBattleUnit.data.attack = levelData.attack;
            _i.playerBattleUnit.data.defense = levelData.defense;

            _i.playerBattleUnit.data.life = levelData.life;
            _i.playerBattleUnit.data.magic = levelData.magic;
        }

        public static void AddBattleUnit(BattleUnit.BattleUnitData battleUnit, Sprite sprite)
        {
            GameObject go = new(battleUnit.title)
            {
                transform =
                {
                    parent = _i.transform
                }
            };

            BattleUnit bu = go.AddComponent<BattleUnit>();
            bu.data = battleUnit;
            bu.sprite = sprite;
            _i.comradeBattleUnits.Add(bu);
        }

        public static BattleUnit[] GetBattleUnits()
        {
            List<BattleUnit> playerBattleUnits = new()
            {
                _i.playerBattleUnit
            };
            playerBattleUnits.AddRange(_i.comradeBattleUnits);

            return playerBattleUnits.ToArray();
        }

        public static BattleUnit.BattleUnitData GetBattleUnitData() => _i.playerBattleUnit.data;

        public static void SetBattleUnitData(BattleUnit.BattleUnitData battleUnitData)
        {
            _i.playerBattleUnit.data = battleUnitData;
        }

        public static void SetName(string name)
        {
            _i.playerBattleUnit.data.title = name;
        }

        public static bool HasRoomInInventory()
        {
            return GetBattleUnits()
                .Any(unit => unit.data.itemOptionsForUnit.Count < BattleUnit.BattleUnitData.MaxItems);
        }

        public static bool AddItemToInventory(ItemScriptable item)
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
}
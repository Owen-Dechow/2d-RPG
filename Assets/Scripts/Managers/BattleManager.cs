using UnityEngine;
using System.Collections;
using Battle;

namespace Managers
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private GameObject battleSystemPrefab;
        private static BattleManager _i;

        private void Awake() => _i = this;

        public static void StartBattle(BattleUnit[] enemyUnits, GameObject enemyGameObject)
        {
            BattleSystem battleSystem = _i.battleSystemPrefab.GetComponent<BattleSystem>();
            battleSystem.enemies = enemyUnits;
            battleSystem.players = PlayerManager.GetBattleUnits();
            battleSystem.enemyGameObject = enemyGameObject;
            
            Instantiate(_i.battleSystemPrefab);
        }

        public static void LostBattle()
        {
            _i.StartCoroutine(LostBattleCoroutine());
        }

        static IEnumerator LostBattleCoroutine()
        {
            using (new CutScene.CutScene.Window(true))
            {
                yield return GameUIManager.ToggleLoadingScreen(true, instant: true);

                GameManager.LoadFromSavePoint(GameManager.Id);

                yield return GameUIManager.ToggleLoadingScreen(false);
            }
        }
    }
}
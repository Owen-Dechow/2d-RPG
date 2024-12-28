using System.Collections;
using System.Collections.Generic;
using Battle;
using Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _i;

        [SerializeField] private CheckpointSystem.CheckpointFlag checkpoint;

        [SerializeField] private GameObject battleSystemPrefab;

        [SerializeField] private GameItems.DataSet[] itemData;
        public static GameItems.DataSet[] ItemData => _i.itemData;

        [SerializeField] private GameMagic.DataSet[] magicData;
        public static GameMagic.DataSet[] MagicData => _i.magicData;

        public static List<int> postInteractionProtectionIDs;

        public static int id;

        public static string Answer { get; set; }
        public static int AnswerIndex { get; set; }

        public static PlayerController.PlacementSettings PlayerPlacementSettings { get; private set; }

        void Start()
        {
            DontDestroyOnLoad(this);

            postInteractionProtectionIDs = new();

            id = SaveSystem.GetNewId();

            _i = this;
        }

        public static void StartBattle(BattleUnit[] enemyUnits, GameObject enemyGameObject)
        {
            BattleSystem battleSystem = _i.battleSystemPrefab.GetComponent<BattleSystem>();
            battleSystem.enemies = enemyUnits;
            battleSystem.players = Player.GetBattleUnits();
            battleSystem.enemyGameObject = enemyGameObject;
            Instantiate(_i.battleSystemPrefab);
        }

        public static string GetCleanedText(string text)
        {
            string cleanedText = text.Trim();
            cleanedText = cleanedText.Replace("{{NAME}}", Player.Name);
            cleanedText = cleanedText.Replace("{{ANSWER}}", Answer);
            cleanedText = cleanedText.Replace('_', ' ');
            cleanedText = cleanedText.Replace("{{ANSWER_IDX}}", AnswerIndex.ToString());

            return cleanedText;
        }

        public static void LoadFromSavePoint(int savePointId)
        {
            SaveDataSerializable data = SaveSystem.LoadData(savePointId);
            SaveDataSerializable.UnpackSaveData(data);
            LoadLevel(data.levelScene, new Vector2(data.position[0], data.position[1]), AnimPlus.Direction.Down);
        }

        public static void LostBattle()
        {
            static IEnumerator lostBattle()
            {
                using (new CutScene.CutScene.Window(true))
                {
                    yield return GameUI.ToggleLoadingScreen(true, instant: true);

                    LoadFromSavePoint(id);

                    yield return GameUI.ToggleLoadingScreen(false);
                }
            }

            _i.StartCoroutine(lostBattle());
        }

        public static void LoadLevel(LevelScene scene, Vector2 playerSpanPoint, AnimPlus.Direction playerSpanDirection)
        {
            PlayerPlacementSettings = new(playerSpanPoint, playerSpanDirection);
            SceneManager.LoadScene(scene.ToString());
            CutScene.CutScene.DisableAllSoftCutscenes();
        }

        public static void LoadLevel(LevelScene scene, DoorController.DoorTag playerSpanDoor)
        {
            PlayerPlacementSettings = new(playerSpanDoor);
            SceneManager.LoadScene(scene.ToString());
        }

        public static IEnumerator LoadLevelAnimated(LevelScene scene, Vector2 playerSpanPoint,
            AnimPlus.Direction playerSpanDirection)
        {
            using (new CutScene.CutScene.Window())
            {
                yield return GameUI.ToggleLoadingScreen(true);
                LoadLevel(scene, playerSpanPoint, playerSpanDirection);
                yield return GameUI.ToggleLoadingScreen(false);
            }
        }

        public static IEnumerator LoadLevelAnimated(LevelScene scene, DoorController.DoorTag playerSpanDoor)
        {
            using (new CutScene.CutScene.Window())
            {
                yield return GameUI.ToggleLoadingScreen(true);
                LoadLevel(scene, playerSpanDoor);
                yield return GameUI.ToggleLoadingScreen(false);
            }
        }

        public static int GetRandomIntId()
        {
            return Random.Range(int.MinValue, int.MaxValue);
        }
    }
}
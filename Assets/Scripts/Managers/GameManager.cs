using System.Collections;
using System.Collections.Generic;
using Controllers;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _i;

        private HashSet<int> postInteractionProtectionIDs;
        public static HashSet<int> PostInteractionProtectionIDs => _i.postInteractionProtectionIDs;

        [SerializeField] private int id;
        public static int Id => _i.id;

        [SerializeField] private bool saveAsJson;
        public static bool SaveAsJson => _i.saveAsJson;

        void Start()
        {
            DontDestroyOnLoad(this);
            postInteractionProtectionIDs = new HashSet<int>();
            id = SaveSystem.GetNewId();
            _i = this;
        }

        public static void SetId(int id)
        {
            _i.id = id;
        }

        public static void SetPostInteractionProtectionIDs(HashSet<int> postInteractionProtectionIDs)
        {
            _i.postInteractionProtectionIDs = postInteractionProtectionIDs;
        }

        public static void LoadFromSavePoint(int savePointId)
        {
            SaveDataSerializable data = SaveSystem.LoadData(savePointId);
            SaveDataSerializable.UnpackSaveData(data);
            LoadLevel(data.levelScene, new Vector2(data.position[0], data.position[1]), AnimPlus.Direction.Down);
        }


        public static void LoadLevel(LevelScene scene, Vector2 playerSpanPoint, AnimPlus.Direction playerSpanDirection)
        {
            PlayerManager.PlacementSettings =
                new PlayerController.PlacementSettings(playerSpanPoint, playerSpanDirection);
            SceneManager.LoadScene(scene.ToString());
            CutScene.CutScene.DisableAllSoftCutscenes();
        }

        public static void LoadLevel(LevelScene scene, DoorController.DoorTag playerSpanDoor)
        {
            PlayerManager.PlacementSettings = new PlayerController.PlacementSettings(playerSpanDoor);
            SceneManager.LoadScene(scene.ToString());
        }

        public static IEnumerator LoadLevelAnimated(LevelScene scene, Vector2 playerSpanPoint,
            AnimPlus.Direction playerSpanDirection)
        {
            using (new CutScene.CutScene.Window())
            {
                yield return GameUIManager.ToggleLoadingScreen(true);
                LoadLevel(scene, playerSpanPoint, playerSpanDirection);
                yield return GameUIManager.ToggleLoadingScreen(false);
            }
        }

        public static IEnumerator LoadLevelAnimated(LevelScene scene, DoorController.DoorTag playerSpanDoor)
        {
            using (new CutScene.CutScene.Window())
            {
                yield return GameUIManager.ToggleLoadingScreen(true);
                LoadLevel(scene, playerSpanDoor);
                yield return GameUIManager.ToggleLoadingScreen(false);
            }
        }

        public static int GetRandomIntId()
        {
            return Random.Range(int.MinValue, int.MaxValue);
        }
    }
}
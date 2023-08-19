using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager i;

    [SerializeField] GameObject battleSystemPrefab;

    [SerializeField] GameItems.DataSet[] itemData;
    public static GameItems.DataSet[] ItemData { get => i.itemData; }

    [SerializeField] GameMagic.DataSet[] magicData;
    public static GameMagic.DataSet[] MagicData { get => i.magicData; }

    public static List<int> NPCActionTreeBranchProtectors;

    public static int id;

    public static string Answer { get; set; }
    public static int AnswerIndex { get; set; }

    public static PlayerController.PlacementSettings PlayerPlacementSettings { get; set; }

    void Start()
    {
        DontDestroyOnLoad(this);

        NPCActionTreeBranchProtectors = new();

        id = SaveSystem.GetNewId();

        i = this;
    }

    public static void StartBattle(BattleUnit[] enemyUnits, GameObject enemyGameObject)
    {
        Time.timeScale = 0;
        BattleSystem battleSystem = i.battleSystemPrefab.GetComponent<BattleSystem>();
        battleSystem.enemies = enemyUnits;
        battleSystem.players = Player.GetBattleUnits();
        battleSystem.enemyGameObject = enemyGameObject;
        Instantiate(i.battleSystemPrefab);
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

    public static void LoadFromSavePoint(int id)
    {
        SaveDataSerializable data = SaveSystem.LoadData(id);
        SaveDataSerializable.UnpackSaveData(data);
        LoadLevel(data.levelScene, new Vector2(data.position[0], data.position[1]), AnimPlus.Direction.down);
    }
    public static void LostBattle()
    {
        static IEnumerator lostBattle()
        {
            Time.timeScale = 0;
            yield return GameUI.ToggleLoadingScreen(true, instant: true);

            LoadFromSavePoint(id);

            yield return GameUI.ToggleLoadingScreen(false);
            Time.timeScale = 1;
        }
        i.StartCoroutine(lostBattle());
    }

    public static void LoadLevel(LevelScene scene, Vector2 playerSpanPoint, AnimPlus.Direction playerSpanDirection)
    {
        PlayerPlacementSettings = new(playerSpanPoint, playerSpanDirection);
        SceneManager.LoadScene(scene.ToString());
    }
    public static void LoadLevel(LevelScene scene, string playerSpanDoor)
    {
        PlayerPlacementSettings = new(playerSpanDoor);
        SceneManager.LoadScene(scene.ToString());
    }
    public static IEnumerator LoadLevelAnimated(LevelScene scene, Vector2 playerSpanPoint, AnimPlus.Direction playerSpanDirection)
    {
        Time.timeScale = 0;
        yield return GameUI.ToggleLoadingScreen(true);
        LoadLevel(scene, playerSpanPoint, playerSpanDirection);
        yield return GameUI.ToggleLoadingScreen(false);
        Time.timeScale = 1;
    }
    public static IEnumerator LoadLevelAnimated(LevelScene scene, string playerSpanDoor)
    {
        Time.timeScale = 0;
        yield return GameUI.ToggleLoadingScreen(true);
        LoadLevel(scene, playerSpanDoor);
        yield return GameUI.ToggleLoadingScreen(false);
        Time.timeScale = 1;
    }
    
    public static int GetRandomIntId()
    {
        return Random.Range(int.MinValue, int.MaxValue);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager i;

    [HideInInspector] public List<GameObject> playerUnits;
    [SerializeField] GameObject battleSystemPrefab;

    [SerializeField] Items.DataSet[] itemData;
    public static Items.DataSet[] ItemData { get => i.itemData; }

    [SerializeField] Magic.DataSet[] magicData;
    public static Magic.DataSet[] MagicData { get => i.magicData; }


    public static List<int> NPCActionTreeBranchProtectors = new();

    public static int id;
    public static readonly float tileWidth = 0.16f;

    public static Player player;
    public static CheckpointSystem checkpoints;

    public static string Answer { get; set; }
    public static int AnswerIndex { get; set; }

    public static bool setPlayerLocationOnLoad;
    public static LevelScene currentLevelScene;

    public static Vector2 playerLocationLoad;
    public static AnimPlus.Direction playerDirectionLoad;
    public static string playerDoorEnter;

    void Awake()
    {
        DontDestroyOnLoad(this);

        id = SaveSystem.GetNewId();

        i = this;
        player = GetComponent<Player>();
        checkpoints = GetComponent<CheckpointSystem>();
    }

    public static void SaveGame()
    {
        SaveData saveData = new();
        SaveSystem.SaveData(saveData);
        Debug.Log("Game Saved: [path] " + SaveSystem.path);
    }
    public static void LoadScene(LevelScene scene, Vector2 playerSpanPoint, AnimPlus.Direction playerSpanDirection)
    {
        playerLocationLoad = playerSpanPoint;
        playerDirectionLoad = playerSpanDirection;
        playerDoorEnter = null;

        setPlayerLocationOnLoad = true;
        currentLevelScene = scene;
        SceneManager.LoadScene(scene.ToString());
    }
    public static void LoadScene(LevelScene scene, string playerSpanDoor)
    {
        playerDoorEnter = playerSpanDoor;

        setPlayerLocationOnLoad = false;
        currentLevelScene = scene;
        SceneManager.LoadScene(scene.ToString());
    }

    public static void StartBattle(BattleUnit[] enemyUnits, GameObject enemyGameObject)
    {
        Time.timeScale = 0;
        BattleSystem battleSystem = i.battleSystemPrefab.GetComponent<BattleSystem>();
        battleSystem.enemies = enemyUnits;
        battleSystem.players = player.GetBattleUnits();
        battleSystem.enemyGameObject = enemyGameObject;
        Instantiate(i.battleSystemPrefab);
    }
    public static string GetCleanedText(string text)
    {
        string cleanedText = text.Trim();
        cleanedText = cleanedText.Replace("{{NAME}}", player.Name);
        cleanedText = cleanedText.Replace("{{ANSWER}}", Answer);
        cleanedText = cleanedText.Replace("_", " ");
        cleanedText = cleanedText.Replace("{{ANSWER_IDX}}", AnswerIndex.ToString());

        return cleanedText;
    }
    public static void SnapTransformToGrid(Transform transform, bool betweenTilesX=false, bool betweenTilesY=false)
    {
        Vector3 positionIDX = (transform.position / tileWidth);
        positionIDX.x = Mathf.Floor(positionIDX.x);
        positionIDX.y = Mathf.Floor(positionIDX.y);
        positionIDX.z = 0;

        Vector3 newPosition = positionIDX * tileWidth;
        if (!betweenTilesX) newPosition.x += tileWidth / 2;
        if (!betweenTilesY) newPosition.y += tileWidth / 2;

        transform.position = newPosition;
    }
    public static void LoadFromSavePoint(int id)
    {
        SaveData data = SaveSystem.LoadData(id);
        SaveData.UnpackSaveData(data);
        LoadScene(data._levelScene, new Vector2(data._position[0], data._position[1]), AnimPlus.Direction.down);
    }
    public static void LostBattle()
    {
        static IEnumerator lostbattle()
        {
            Time.timeScale = 0;
            yield return GameUI.ToggleLoadingScreen(true, instant: true);

            LoadFromSavePoint(id);

            yield return GameUI.ToggleLoadingScreen(false);
            Time.timeScale = 1;
        }
        i.StartCoroutine(lostbattle());
    }

    public static IEnumerator LoadLevelAnimated(LevelScene scene, Vector2 playerSpanPoint, AnimPlus.Direction playerSpanDirection)
    {
        Time.timeScale = 0;
        yield return GameUI.ToggleLoadingScreen(true);
        LoadScene(scene, playerSpanPoint, playerSpanDirection);
        yield return GameUI.ToggleLoadingScreen(false);
        Time.timeScale = 1;
    }
    public static IEnumerator LoadLevelAnimated(LevelScene scene, string playerSpanDoor)
    {
        Time.timeScale = 0;
        yield return GameUI.ToggleLoadingScreen(true);
        LoadScene(scene, playerSpanDoor);
        yield return GameUI.ToggleLoadingScreen(false);
        Time.timeScale = 1;
    }
    public static int GetRandomIntId()
    {
        return Random.Range(int.MinValue, int.MaxValue);
    }
}
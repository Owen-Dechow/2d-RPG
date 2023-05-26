using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public List<GameObject> playerUnits;

    [SerializeField] GameObject battleSystemPrefab;
    [SerializeField] TextBox textBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Image loadingScreen;
    [SerializeField] GameObject gameUi;
    private static GameManager instance;

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
    public static Door.DoorTag playerDoorEnter;

    void Start()
    {
        DontDestroyOnLoad(this);

        id = SaveSystem.GetNewId();

        instance = this;
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
        playerDoorEnter = (Door.DoorTag)int.MaxValue;

        setPlayerLocationOnLoad = true;
        currentLevelScene = scene;
        SceneManager.LoadScene(scene.ToString());
    }
    public static void LoadScene(LevelScene scene, Door.DoorTag playerSpanDoor)
    {
        playerDoorEnter = playerSpanDoor;

        setPlayerLocationOnLoad = false;
        currentLevelScene = scene;
        SceneManager.LoadScene(scene.ToString());
    }

    public static void StartBattle(BattleUnit[] enemyUnits, GameObject enemyGameObject)
    {
        Time.timeScale = 0;
        BattleSystem battleSystem = instance.battleSystemPrefab.GetComponent<BattleSystem>();
        battleSystem.enemies = enemyUnits;
        battleSystem.players = player.GetBattleUnits();
        battleSystem.enemyGameObject = enemyGameObject;
        Instantiate(instance.battleSystemPrefab);
    }
    public static void CloseUI()
    {
        instance.textBox.Close();
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
            yield return ToggleLoadingScreen(true, instant: true);

            LoadFromSavePoint(id);

            yield return ToggleLoadingScreen(false);
            Time.timeScale = 1;
        }
        instance.StartCoroutine(lostbattle());
    }

    public static IEnumerator TypeOut(string text, bool close = true)
    {
        string newText = GetCleanedText(text);
        yield return instance.textBox.TypeOut(newText, close);
    }
    public static IEnumerator GetChoice(string prompt, string[] options, bool alowCancel = false)
    {
        string[] newOptions = options;
        for (int i = 0; i < options.Length; i++)
        {
            newOptions[i] = GetCleanedText(options[i]);
        }

        yield return instance.choiceBox.AskChoice(prompt, newOptions, alowCancel);
    }
    public static IEnumerator GetYesNo(string prompt, string thirdOption = null, bool alowCancel = false)
    {
        string[] options;
        if (thirdOption != null && thirdOption.Length > 0)
            options = new string[] { "Yes", "No", GetCleanedText(thirdOption) };
        else
            options = new string[] { "Yes", "No" };

        yield return instance.choiceBox.AskChoice(prompt, options, alowCancel);
    }
    public static IEnumerator ToggleLoadingScreen(bool onOff, bool instant = false)
    {
        instance.gameUi.SetActive(true);

        if (instant)
        {
            instance.loadingScreen.color = new Color(0, 0, 0, onOff ? 1 : 0);
        }

        if (onOff)
        {
            while (instance.loadingScreen.color.a < 1)
            {
                instance.loadingScreen.color = new Color(0, 0, 0, Mathf.Clamp(instance.loadingScreen.color.a + Time.unscaledDeltaTime, 0, 1));
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (instance.loadingScreen.color.a > 0)
            {
                instance.loadingScreen.color = new Color(0, 0, 0, Mathf.Clamp(instance.loadingScreen.color.a - Time.unscaledDeltaTime, 0, 1));
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public static IEnumerator LoadLevelAnimated(LevelScene scene, Vector2 playerSpanPoint, AnimPlus.Direction playerSpanDirection)
    {
        Time.timeScale = 0;
        yield return ToggleLoadingScreen(true);
        LoadScene(scene, playerSpanPoint, playerSpanDirection);
        yield return ToggleLoadingScreen(false);
        Time.timeScale = 1;
    }
    public static IEnumerator LoadLevelAnimated(LevelScene scene, Door.DoorTag playerSpanDoor)
    {
        Time.timeScale = 0;
        yield return ToggleLoadingScreen(true);
        LoadScene(scene, playerSpanDoor);
        yield return ToggleLoadingScreen(false);
        Time.timeScale = 1;
    }
    public static int GetRandomIntId()
    {
        return Random.Range(int.MinValue, int.MaxValue);
    }
}
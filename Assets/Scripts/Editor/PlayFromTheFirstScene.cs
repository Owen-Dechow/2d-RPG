using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlayFromTheFirstScene
{

    // https://github.com/AiFuYou/UnityDemo/blob/master/Assets/Plugins/Editor/PlayFromTheFirstScene.cs

    const string PlayFromFirstMenuStr = "Custom/Always Start From LoadScene &p";

    public static int currentScene;

    static bool PlayFromFirstScene
    {
        get => EditorPrefs.HasKey(PlayFromFirstMenuStr) && EditorPrefs.GetBool(PlayFromFirstMenuStr);
        set => EditorPrefs.SetBool(PlayFromFirstMenuStr, value);
    }

    [MenuItem(PlayFromFirstMenuStr, false, 150)]
    static void PlayFromFirstSceneCheckMenu()
    {
        PlayFromFirstScene = !PlayFromFirstScene;
        Menu.SetChecked(PlayFromFirstMenuStr, PlayFromFirstScene);

        ShowNotifyOrLog(PlayFromFirstScene ? "Play from LoadScene" : "Play from current scene");
    }

    // The menu won't be gray out, we use this validate method for update check state
    [MenuItem(PlayFromFirstMenuStr, true)]
    static bool PlayFromFirstSceneCheckMenuValidate()
    {
        Menu.SetChecked(PlayFromFirstMenuStr, PlayFromFirstScene);
        return true;
    }

    // This method is called before any Awake. It's the perfect callback for this feature
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void LoadFirstSceneAtGameBegins()
    {
        if (!PlayFromFirstScene)
            return;

        if (EditorBuildSettings.scenes.Length == 0)
        {
            Debug.LogWarning("The scene build list is empty. Can't play from first scene.");
            return;
        }

        foreach (GameObject go in Object.FindObjectsOfType<GameObject>())
            go.SetActive(false);

        currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(0);
    }

    static void ShowNotifyOrLog(string msg)
    {
        if (Resources.FindObjectsOfTypeAll<SceneView>().Length > 0)
            EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent(msg));
        else
            Debug.Log(msg); // When there's no scene view opened, we just print a log
    }
}
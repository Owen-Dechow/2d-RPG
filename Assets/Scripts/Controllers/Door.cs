using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorOpenDir
    {
        top = 0,
        bottom = 180,
        left = 90,
        right = -90,
    }

    public enum LoadType
    {
        DoorToDoor,
        DoorToSpanPoint,
        NoEnter,
    }

    public enum DoorTag
    {
        UnSetDoor,
        MainDoor,
        MainEntrance,
        MainExit,
        D1,
        D2,
        D3,
        D4,
        D5,
        D6,
    }

    public LevelScene toLevel;
    public DoorOpenDir doorOpening;

    [SerializeField] DoorTag doorTag;

    [SerializeField] LoadType loadType;
    [SerializeField] DoorTag connectedDoor;
    [SerializeField] Vector2 spanPosition;

    public string disallowEnterText;

    [HideInInspector] public static Door door;

    private void Awake()
    {
        if (GameManager.PlayerPlacementSettings.DoorTag == doorTag)
        {
            door = this;
        }
    }

    private void Start()
    {
        Destroy(GetComponent<SpriteRenderer>());

        if (GameManager.PlayerPlacementSettings.Relocation != PlayerController.PlacementSettings.RelocateType.Door)
            return;
    }

    private void OnValidate()
    {
        transform.rotation = Quaternion.Euler(0, 0, (int)doorOpening);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        string text = GameManager.GetCleanedText(disallowEnterText);

        if (loadType == LoadType.NoEnter)
        {
            StartCoroutine(CantEnter(text));
            return;
        }

        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        yield return GameManager.LoadLevelAnimated(toLevel, connectedDoor);
        Destroy(gameObject);
    }

    IEnumerator CantEnter(string text)
    {
        Time.timeScale = 0;
        yield return GameUI.TypeOut(text);
        Time.timeScale = 1;
    }
}

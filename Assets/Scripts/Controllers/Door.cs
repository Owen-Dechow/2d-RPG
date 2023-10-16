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

    [SerializeField] LevelScene toLevel;

    public DoorOpenDir doorOpening;

    [SerializeField] string doorTag;
    [SerializeField] string connectedDoor;

    [SerializeField] string disallowEnterText;

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
        if (text.Length > 0)
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

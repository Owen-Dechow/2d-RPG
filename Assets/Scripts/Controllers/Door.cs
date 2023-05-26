using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorTag
    {
        NoTag,
        Door1,
        Door2,
        Door3,
        Door4,
        Door5,
        Door6,
        Door7,
        Door8,
        Door9,
        Door10,
        Door11,
        Door12,
        Door13,
        Door14,
        Door15,
        Door16,
        Door17,
        Door18,
        Door19,
        Door20,
    }
    public enum DoorOpenDir
    {
        top = 0,
        bottom = 180,
        left = 90,
        right = -90,
    }

    [SerializeField] LevelScene toLevel;

    [SerializeField] DoorOpenDir doorOpening;
    [SerializeField] bool betweenTiles;

    [SerializeField] DoorTag doorTag;
    [SerializeField] DoorTag connectedDoor;

    [SerializeField] string disallowEnterText;

    private void OnValidate()
    {
        Position();
    }

    private void Start()
    {
        Position();
        if (GameManager.playerDoorEnter == doorTag)
        {
            var direction = doorOpening switch
            {
                DoorOpenDir.top => AnimPlus.Direction.up,
                DoorOpenDir.bottom => AnimPlus.Direction.down,
                DoorOpenDir.left => AnimPlus.Direction.left,
                DoorOpenDir.right => AnimPlus.Direction.right,
                _ => AnimPlus.Direction.down,
            };
            GameManager.player.playerObject.GetComponent<AnimPlus>().SetDirection(direction);

            GameManager.player.playerObject.transform.position = transform.position;
            Vector2 delta = new Vector2(
                Mathf.Cos((int)doorOpening),
                Mathf.Sin((int)doorOpening)
                ) * 0.16f;
            GameManager.player.playerObject.transform.Translate(delta.x + 0.08f, delta.y, 0);
            CameraController.SetUpCamera();
        }
    }

    private void Position()
    {
        transform.rotation = Quaternion.Euler(0, 0, (int)doorOpening);
        GameManager.SnapTransformToGrid(transform, betweenTiles);
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
        DontDestroyOnLoad(gameObject);
        yield return GameManager.LoadLevelAnimated(toLevel, connectedDoor);
        Destroy(gameObject);

    }
    IEnumerator CantEnter(string text)
    {
        Time.timeScale = 0;
        yield return GameManager.TypeOut(text);
        Time.timeScale = 1;

    }
}

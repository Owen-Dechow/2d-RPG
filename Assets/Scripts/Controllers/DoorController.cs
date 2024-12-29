using System.Collections;
using Data;
using Managers;
using Managers.CutScene;
using UnityEngine;

namespace Controllers
{
    public class DoorController : MonoBehaviour
    {
        public enum DoorOpenDir
        {
            Top = 0,
            Bottom = 180,
            Left = 90,
            Right = -90,
        }

        public enum LoadType
        {
            DoorToDoor,
            DoorToSpawnPoint,
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
            D7,
            D8,
            D9,
            D10,
        }

        public LevelScene toLevel;
        public DoorOpenDir doorOpening;

        [SerializeField] DoorTag doorTag;

        [SerializeField] LoadType loadType;
        [SerializeField] DoorTag connectedDoor;
        [SerializeField] Vector2 spanPosition;

        public string disallowEnterText;

        public static DoorController doorController;

        private void Awake()
        {
            if (GameManager.PlayerPlacementSettings.DoorTag == doorTag)
            {
                doorController = this;
            }
        }

        private void Start()
        {
            Destroy(GetComponent<SpriteRenderer>());
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
            using (new CutScene.Window())
            {
                yield return GameUI.TypeOut(text);
            }
        }
    }
}
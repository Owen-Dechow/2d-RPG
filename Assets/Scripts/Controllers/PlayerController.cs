using System;
using Managers;
using UnityEngine;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [HideInInspector] public bool moving;
        public bool CanAttack => inactiveSeconds >= 2;

        public static PlayerController playerController;

        public BoxCollider2D interactionCollider;

        [SerializeField] float normalSpeed;
        [SerializeField] float dashSpeed;

        private Rigidbody2D rb2D;
        private SpriteRenderer spr;
        private AnimPlus animPlus;
        private float inactiveSeconds;

        private Vector2 delta;
        private bool running;
        private bool openMenu;

        void Start()
        {
            playerController = this;
            rb2D = GetComponent<Rigidbody2D>();
            spr = GetComponent<SpriteRenderer>();
            animPlus = GetComponent<AnimPlus>();
            inactiveSeconds = 99;


            if (GameManager.PlayerPlacementSettings.Relocation == PlacementSettings.RelocateType.Position)
            {
                transform.position = GameManager.PlayerPlacementSettings.Position;
                animPlus.SetDirection(GameManager.PlayerPlacementSettings.Direction);
            }
            else if (GameManager.PlayerPlacementSettings.Relocation == PlacementSettings.RelocateType.Door)
            {
                MoveToDoor(DoorController.doorController.transform.position, DoorController.doorController.doorOpening);
            }

            CameraController.CenterCameraOnPlayer();
            CutScene.OnEnable += OnCutScene;
        }

        private void OnDestroy()
        {
            CutScene.OnEnable -= OnCutScene;
        }

        private void OnCutScene()
        {
            rb2D.velocity = Vector2.zero;
        }

        void Update()
        {
            if (!CutScene.Enabled)
            {
                // Input
                running = MyInput.Select == 1;
                delta = new()
                {
                    x = MyInput.MoveHorizontal,
                    y = MyInput.MoveVertical
                };
                openMenu = MyInput.OpenMenu;


                animPlus.speedAdded = running;

                // inactive
                if (!CanAttack)
                {
                    inactiveSeconds += Time.deltaTime;
                    if (CanAttack) spr.color = Color.white;
                    else
                        spr.color = Mathf.Floor(Time.time * 10) % 2 == 0
                            ? Color.white
                            : new Color(0.3f, 0.3f, 0.3f, 0.3f);
                }


                // rigid body
                delta = delta.normalized;

                if
                    (running) delta *= dashSpeed;
                else
                    delta *= normalSpeed;

                rb2D.velocity = normalSpeed * delta;

                // Moving
                moving = delta.magnitude > 0;

                // Handel Menu
                if (openMenu)
                {
                    StartCoroutine(MenuController.Menu());
                }
            }
        }

        void MoveToDoor(Vector3 position, DoorController.DoorOpenDir doorOpening)
        {
            AnimPlus.Direction direction;
            Vector2 delta;

            switch (doorOpening)
            {
                case DoorController.DoorOpenDir.Top:
                    delta = Vector2.up;
                    direction = AnimPlus.Direction.Up;
                    break;

                case DoorController.DoorOpenDir.Bottom:
                    delta = Vector2.down;
                    direction = AnimPlus.Direction.Down;
                    break;

                case DoorController.DoorOpenDir.Right:
                    delta = Vector2.right;
                    direction = AnimPlus.Direction.Right;
                    break;

                case DoorController.DoorOpenDir.Left:
                    delta = Vector2.left;
                    direction = AnimPlus.Direction.Left;
                    break;
                default:
                    throw new Exception("Unknown Direction");
            }

            GetComponent<AnimPlus>().SetDirection(direction);

            transform.position = position;

            if (Mathf.Abs(delta.x) == 1)
            {
                delta *= 0.17f;
                delta.y += 0.08f;
            }
            else
                delta *= 0.16f;

            transform.Translate(delta.x, delta.y, 0);
        }

        public void SetInactive()
        {
            inactiveSeconds = 0;
        }

        public class PlacementSettings
        {
            public enum RelocateType
            {
                Door,
                Position,
                None
            }

            public RelocateType Relocation { get; private set; }
            public DoorController.DoorTag DoorTag { get; private set; }
            public AnimPlus.Direction Direction { get; private set; }
            public Vector2 Position { get; private set; }

            public PlacementSettings(DoorController.DoorTag doorTag)
            {
                Relocation = RelocateType.Door;
                DoorTag = doorTag;
            }

            public PlacementSettings(Vector2 position, AnimPlus.Direction direction)
            {
                Relocation = RelocateType.Position;
                Position = position;
                Direction = direction;
            }

            public PlacementSettings()
            {
                Relocation = RelocateType.None;
                Direction = AnimPlus.Direction.Down;
            }
        }
    }
}
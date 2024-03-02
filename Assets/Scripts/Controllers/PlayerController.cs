using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Door;

public class PlayerController : MonoBehaviour
{

    [HideInInspector] public bool moving = false;
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
            MoveToDoor(Door.door.transform.position, door.doorOpening);
        }

        CameraController.CenterCameraOnPlayer();
    }

    void Update()
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
            else spr.color = Mathf.Floor(Time.time * 10) % 2 == 0 ? Color.white : new Color(0.3f, 0.3f, 0.3f, 0.3f);
        }

        // sprite renderer
        if (Time.timeScale > 0)
        {
            if (delta.x == 1) spr.flipX = false;
            else if (delta.x == -1) spr.flipX = true;
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
            if (Time.timeScale > 0)
                StartCoroutine(MenuController.Menu());
        }
    }

    void MoveToDoor(Vector3 position, Door.DoorOpenDir doorOpening)
    {
        AnimPlus.Direction direction;
        Vector2 delta;

        switch (doorOpening)
        {
            case DoorOpenDir.top:
                delta = Vector2.up;
                direction = AnimPlus.Direction.up;
                break;

            case DoorOpenDir.bottom:
                delta = Vector2.down;
                direction = AnimPlus.Direction.down;
                break;

            case DoorOpenDir.right:
                delta = Vector2.right;
                direction = AnimPlus.Direction.right;
                break;

            case DoorOpenDir.left:
                delta = Vector2.left;
                direction = AnimPlus.Direction.left;
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
        public Door.DoorTag DoorTag { get; private set; }
        public AnimPlus.Direction Direction { get; private set; }
        public Vector2 Position { get; private set; }

        public PlacementSettings(Door.DoorTag doorTag)
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
            Direction = AnimPlus.Direction.down;
        }
    }
}

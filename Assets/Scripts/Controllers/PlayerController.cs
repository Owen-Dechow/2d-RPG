using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [HideInInspector] public bool moving = false;
    public bool CanAttack { get => inactiveSeconds >= 2; }

    public BoxCollider2D interactionCollider;

    [SerializeField] float normalSpeed;
    [SerializeField] float dashSpeed;

    private Rigidbody2D rb2D;
    private SpriteRenderer spr;
    private AnimPlus animPlus;
    private float inactiveSeconds;


    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        animPlus = GetComponent<AnimPlus>();
        inactiveSeconds = 99;

        GameManager.player.playerObject = this;

        if (GameManager.PlayerPlacementSettings.Relocation == PlacementSettings.RelocateType.Position)
        {
            transform.position = GameManager.PlayerPlacementSettings.Position;
            animPlus.SetDirection(GameManager.PlayerPlacementSettings.Direction);
        }
    }

    public void Update()
    {
        bool running = MyInput.Select == 1;
        animPlus.speedAdded = running;
        Vector2 delta = new()
        {
            x = MyInput.MoveHorizontal,
            y = MyInput.MoveVertical
        };

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
        if (running) delta *= dashSpeed;
        else delta *= normalSpeed;
        rb2D.velocity = normalSpeed * (Vector3)delta;

        // Moving
        moving = delta.magnitude > 0;

        // Handel Menu
        MenuClick();
    }

    void MenuClick()
    {
        if (Time.timeScale <= 0) return;

        if (MyInput.OpenMenu)
        {
            StartCoroutine(Menu());
        }
    }

    IEnumerator Menu()
    {
        Time.timeScale = 0;
        yield return new WaitUntil(() => MyInput.Select == 0);

        List<string> options = new();
        
        foreach(BattleUnit unit in GameManager.playerBattleUnits)
        {
            string tag;

            tag = $"{unit.data.title}\\Items\\";
            foreach (Items.Options item in unit.data.items)
            {
                options.Add(tag + GameManager.GetCleanedText(item.ToString()));
            }

            tag = $"{unit.data.title}\\Magic\\";
            foreach (Magic.Options magic in unit.data.magicOptionsForUnit)
            {
                options.Add(tag + GameManager.GetCleanedText(magic.ToString()));
            }
        }

        yield return GameUI.FullMenu(options.ToArray(), true);

        yield return new WaitUntil(() => MyInput.Select == 0);
        Time.timeScale = 1;
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
        public string DoorTag { get; private set; }
        public AnimPlus.Direction Direction { get; private set; }
        public Vector2 Position { get; private set; }

        public PlacementSettings(string doorTag)
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

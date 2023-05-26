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

        if (GameManager.setPlayerLocationOnLoad)
        {
            transform.position = GameManager.playerLocationLoad;
            animPlus.SetDirection(GameManager.playerDirectionLoad);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        bool running = MyInput.GetSelect() == 1;
        animPlus.speedAdded = running;
        Vector2 delta = new()
        {
            x = MyInput.GetMoveHorizontal(),
            y = MyInput.GetMoveVertical()
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
    }

    public void SetInactive()
    {
        inactiveSeconds = 0;
    }
}

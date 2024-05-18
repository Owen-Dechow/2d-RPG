using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Npc : MonoBehaviour
{
    public enum MovementType
    {
        FaceDown,
        FaceUp,
        FaceLeft,
        FaceRight,
        FacePlayer,
        XOnly,
        YOnly,
        XAndY,
    }

    [SerializeField] MovementType movementType;
    public BehaviorTree behaviorTree;

    [SerializeField] CheckpointSystem.CheckpointFlag NPCEnabledCheckpointWindowOpen;
    [SerializeField] CheckpointSystem.CheckpointFlag NPCEnabledCheckpointWindowClose;

    private bool inPlayerInteractionZone = false;
    private AnimPlus animPlus;

    readonly float movementRadius = 1;
    readonly float moveSpeed = .4f;
    Vector2 origin;
    Vector2 moveDelta;
    Rigidbody2D rb2d;
    float changeDirInterval = 3;
    float dirChangeTime = 0;
    float collisionStaySeconds;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (!CheckpointSystem.GetWindow(NPCEnabledCheckpointWindowOpen, NPCEnabledCheckpointWindowClose))
            Destroy(gameObject);

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animPlus = GetComponent<AnimPlus>();
        origin = transform.position;
        rb2d = GetComponent<Rigidbody2D>();

        SetStartDirection();
    }

    void Update()
    {
        InteractWithPlayer();
        MoveNPC();
    }

    IEnumerator DestroySlow()
    {
        int iterations = 6;
        for (float i = 0; i < iterations; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, Mathf.Clamp(1 - (i / iterations), 0, 1));
            yield return new WaitForSecondsRealtime(0.1f);
            spriteRenderer.color = Color.clear;
            yield return new WaitForSecondsRealtime(0.05f);
        }

        Destroy(gameObject);
    }

    void InteractWithPlayer()
    {
        if (MyInput.SelectDown != 1) return;
        if (!inPlayerInteractionZone) return;
        if (Time.timeScale == 0) return;

        StartCoroutine(RunInteraction());
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player")) return;
        inPlayerInteractionZone = true;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player")) return;
        inPlayerInteractionZone = false;
    }

    IEnumerator RunInteraction()
    {
        Time.timeScale = 0;
        yield return new WaitForEndOfFrame();

        yield return behaviorTree.Run(this, new BehaviorTree.TreeData());

        yield return new WaitWhile(() => MyInput.Select == 1);

        if (!CheckpointSystem.GetWindow(NPCEnabledCheckpointWindowOpen, NPCEnabledCheckpointWindowClose))
        {
            yield return DestroySlow();
        }

        Time.timeScale = 1;
    }

    void SetStartDirection()
    {
        switch (movementType)
        {
            case MovementType.FaceDown:
                animPlus.SetDirection(AnimPlus.Direction.down);
                break;
            case MovementType.FaceUp:
                animPlus.SetDirection(AnimPlus.Direction.up);
                break;
            case MovementType.FaceLeft:
                animPlus.SetDirection(AnimPlus.Direction.left);
                break;
            case MovementType.FaceRight:
                animPlus.SetDirection(AnimPlus.Direction.right);
                break;
            case MovementType.FacePlayer:
                animPlus.SetDirection(AnimPlus.RandomDirection());
                break;
            case MovementType.XOnly:
                animPlus.SetDirection(AnimPlus.RandomDirection(includeY: false));
                break;
            case MovementType.YOnly:
                animPlus.SetDirection(AnimPlus.RandomDirection(includeX: false));
                break;
            case MovementType.XAndY:
                animPlus.SetDirection(AnimPlus.RandomDirection());
                break;
        }
    }

    void MoveNPC()
    {
        if (Time.timeScale == 0)
            return;

        switch (movementType)
        {
            case MovementType.FaceDown:
                {
                    animPlus.SetDirection(AnimPlus.Direction.down);
                    rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
                    break;
                }
            case MovementType.FaceUp:
                {
                    animPlus.SetDirection(AnimPlus.Direction.up);
                    rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
                    break;
                }
            case MovementType.FaceLeft:
                {
                    animPlus.SetDirection(AnimPlus.Direction.left);
                    rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
                    break;
                }
            case MovementType.FaceRight:
                {
                    animPlus.SetDirection(AnimPlus.Direction.right);
                    rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
                    break;
                }
            case MovementType.FacePlayer:
                {
                    Vector3 playerPosition = PlayerController.playerController.transform.position;
                    if (Vector3.Distance(playerPosition, transform.position) <= movementRadius)
                    {
                        Vector2 positionDif = playerPosition - transform.position;

                        if (Mathf.Abs(positionDif.x) > Mathf.Abs(positionDif.y))
                        {
                            if (positionDif.x < 0)
                                animPlus.SetDirection(AnimPlus.Direction.left);
                            else
                                animPlus.SetDirection(AnimPlus.Direction.right);
                        }
                        else
                        {
                            if (positionDif.y < 0)
                                animPlus.SetDirection(AnimPlus.Direction.down);
                            else
                                animPlus.SetDirection(AnimPlus.Direction.up);
                        }
                    }
                    rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
                    break;
                }
            case MovementType.XOnly:
                {
                    // Ensure valid direction
                    if (moveDelta != Vector2.left && moveDelta != Vector2.right)
                        moveDelta = Random.value <= .5f ? Vector2.left : Vector2.right;

                    if (Vector2.Distance(origin, transform.position) > movementRadius)
                    {
                        if (transform.position.x > origin.x)
                            moveDelta.x = -1;
                        else
                            moveDelta.x = 1;
                    }


                    rb2d.velocity = moveDelta * moveSpeed;
                    animPlus.SetUseRigidBody(true);

                    rb2d.bodyType = RigidbodyType2D.Dynamic;
                    rb2d.constraints = RigidbodyConstraints2D.FreezePositionY;
                    break;
                }
            case MovementType.YOnly:
                {
                    // Ensure valid direction
                    if (moveDelta != Vector2.up && moveDelta != Vector2.down)
                        moveDelta = Random.value <= .5f ? Vector2.up : Vector2.down;

                    if (Vector2.Distance(origin, transform.position) > movementRadius)
                    {
                        if (transform.position.y > origin.y)
                            moveDelta.y = -1;
                        else
                            moveDelta.y = 1;
                    }

                    rb2d.velocity = moveDelta * moveSpeed;
                    animPlus.SetUseRigidBody(true);

                    rb2d.bodyType = RigidbodyType2D.Dynamic;
                    rb2d.constraints = RigidbodyConstraints2D.None;
                    break;
                }
            case MovementType.XAndY:
                {
                    if (moveDelta == null || moveDelta == Vector2.zero)
                        moveDelta = new Vector2(
                            Random.Range(-1, 2),
                            Random.Range(-1, 2));

                    if (Vector2.Distance(origin, transform.position) > movementRadius)
                    {
                        if (transform.position.y > origin.y)
                            moveDelta.y = -1;
                        else
                            moveDelta.y = 1;

                        if (transform.position.x > origin.x)
                            moveDelta.x = -1;
                        else
                            moveDelta.x = 1;
                    }

                    dirChangeTime += Time.deltaTime;
                    if (dirChangeTime > changeDirInterval)
                    {
                        do
                        {
                            moveDelta = new Vector2(
                               Random.Range(-1, 2),
                               Random.Range(-1, 2));
                        } while (moveDelta == Vector2.zero);

                        changeDirInterval = Random.value * 5;
                        dirChangeTime = 0;
                    }

                    rb2d.bodyType = RigidbodyType2D.Dynamic;
                    rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
                    rb2d.velocity = moveDelta.normalized * moveSpeed;
                    break;
                }
        }

        rb2d.freezeRotation = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionStaySeconds = 0;
        moveDelta *= -1;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collisionStaySeconds = 0;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        collisionStaySeconds += Time.deltaTime;
        if (collisionStaySeconds >= 0.1f)
        {
            moveDelta *= -1;
            collisionStaySeconds = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        if (!NPCEnabledCheckpointWindowOpen.None)
        {
            Gizmos.DrawLine(transform.position + Vector3.left * 0.08f + Vector3.up * 0.08f, transform.position + Vector3.left * 0.08f + Vector3.down * 0.16f);
        }

        if (!NPCEnabledCheckpointWindowClose.None)
        {
            Gizmos.DrawLine(transform.position + Vector3.right * 0.08f + Vector3.up * 0.08f, transform.position + Vector3.right * 0.08f + Vector3.down * 0.16f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (Application.isPlaying)
            Gizmos.DrawWireSphere(origin, movementRadius);
        else
            Gizmos.DrawWireSphere(transform.position, movementRadius);
    }
}
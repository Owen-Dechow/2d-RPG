using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controllers
{
    public class Npc : MonoBehaviour
    {
        private enum MovementType
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

        [SerializeField] private MovementType movementType;
        public BehaviorTree behaviorTree;

        [FormerlySerializedAs("NPCEnabledCheckpointWindowOpen")] [SerializeField] private CheckpointSystem.CheckpointFlag npcEnabledCheckpointWindowOpen;
        [FormerlySerializedAs("NPCEnabledCheckpointWindowClose")] [SerializeField] private CheckpointSystem.CheckpointFlag npcEnabledCheckpointWindowClose;

        private bool inPlayerInteractionZone;
        private AnimPlus animPlus;

        private const float MovementRadius = 1;
        private const float MoveSpeed = .4f;
        private Vector2 origin;
        private Vector2 moveDelta;
        private Rigidbody2D rb2d;
        private float changeDirInterval = 3;
        private float dirChangeTime;
        private float collisionStaySeconds;

        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            if (!CheckpointSystem.GetWindow(npcEnabledCheckpointWindowOpen, npcEnabledCheckpointWindowClose))
                Destroy(gameObject);

            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            animPlus = GetComponent<AnimPlus>();
            origin = transform.position;
            rb2d = GetComponent<Rigidbody2D>();

            SetStartDirection();
        }

        private void Update()
        {
            InteractWithPlayer();
            MoveNpc();
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

        private void InteractWithPlayer()
        {
            if (MyInput.SelectDown != 1) return;
            if (!inPlayerInteractionZone) return;
            if (Time.timeScale == 0) return;

            StartCoroutine(RunInteraction());
        }

        private void OnTriggerEnter2D(Collider2D collider2d)
        {
            if (!collider2d.CompareTag("Player")) return;
            inPlayerInteractionZone = true;
        }

        private void OnTriggerExit2D(Collider2D collider2d)
        {
            if (!collider2d.CompareTag("Player")) return;
            inPlayerInteractionZone = false;
        }

        IEnumerator RunInteraction()
        {
            Time.timeScale = 0;
            yield return new WaitForEndOfFrame();

            yield return behaviorTree.Run(this, new BehaviorTree.TreeData());

            yield return new WaitWhile(() => MyInput.Select == 1);

            if (!CheckpointSystem.GetWindow(npcEnabledCheckpointWindowOpen, npcEnabledCheckpointWindowClose))
            {
                yield return DestroySlow();
            }

            Time.timeScale = 1;
        }

        private void SetStartDirection()
        {
            switch (movementType)
            {
                case MovementType.FaceUp:
                    animPlus.SetDirection(AnimPlus.Direction.Up);
                    break;
                case MovementType.FaceDown:
                    animPlus.SetDirection(AnimPlus.Direction.Down);
                    break;
                case MovementType.FaceLeft:
                    animPlus.SetDirection(AnimPlus.Direction.Left);
                    break;
                case MovementType.FaceRight:
                    animPlus.SetDirection(AnimPlus.Direction.Right);
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

        private void MoveNpc()
        {
            if (Time.timeScale == 0)
                return;

            switch (movementType)
            {
                case MovementType.FaceDown:
                {
                    animPlus.SetDirection(AnimPlus.Direction.Down);
                    rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
                    break;
                }
                case MovementType.FaceUp:
                {
                    animPlus.SetDirection(AnimPlus.Direction.Up);
                    rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
                    break;
                }
                case MovementType.FaceLeft:
                {
                    animPlus.SetDirection(AnimPlus.Direction.Left);
                    rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
                    break;
                }
                case MovementType.FaceRight:
                {
                    animPlus.SetDirection(AnimPlus.Direction.Right);
                    rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
                    break;
                }
                case MovementType.FacePlayer:
                {
                    Vector3 playerPosition = PlayerController.playerController.transform.position;
                    if (Vector3.Distance(playerPosition, transform.position) <= MovementRadius)
                    {
                        Vector2 positionDif = playerPosition - transform.position;

                        if (Mathf.Abs(positionDif.x) > Mathf.Abs(positionDif.y))
                        {
                            animPlus.SetDirection(
                                positionDif.x < 0 ? AnimPlus.Direction.Left : AnimPlus.Direction.Right);
                        }
                        else
                        {
                            animPlus.SetDirection(positionDif.y < 0 ? AnimPlus.Direction.Down : AnimPlus.Direction.Up);
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

                    if (Vector2.Distance(origin, transform.position) > MovementRadius)
                    {
                        if (transform.position.x > origin.x)
                            moveDelta.x = -1;
                        else
                            moveDelta.x = 1;
                    }


                    rb2d.velocity = moveDelta * MoveSpeed;
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

                    if (Vector2.Distance(origin, transform.position) > MovementRadius)
                    {
                        if (transform.position.y > origin.y)
                            moveDelta.y = -1;
                        else
                            moveDelta.y = 1;
                    }

                    rb2d.velocity = moveDelta * MoveSpeed;
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

                    if (Vector2.Distance(origin, transform.position) > MovementRadius)
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
                    rb2d.velocity = moveDelta.normalized * MoveSpeed;
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

            if (!npcEnabledCheckpointWindowOpen.None)
            {
                Gizmos.DrawLine(transform.position + Vector3.left * 0.08f + Vector3.up * 0.08f,
                    transform.position + Vector3.left * 0.08f + Vector3.down * 0.16f);
            }

            if (!npcEnabledCheckpointWindowClose.None)
            {
                Gizmos.DrawLine(transform.position + Vector3.right * 0.08f + Vector3.up * 0.08f,
                    transform.position + Vector3.right * 0.08f + Vector3.down * 0.16f);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            if (Application.isPlaying)
                Gizmos.DrawWireSphere(origin, MovementRadius);
            else
                Gizmos.DrawWireSphere(transform.position, MovementRadius);
        }
    }
}
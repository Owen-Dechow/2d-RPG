using Battle;
using Managers;
using Managers.CutScene;
using UnityEngine;

namespace Controllers
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] MovementOptions movementOptions;
        [SerializeField] BattleEnterOptions battleEnterOptions;

        private Rigidbody2D rb;
        private BattleUnit battleUnit;

        private bool turnPositive;
        private float timeInDirection;
        private bool turn = false;
        private byte numberOfTurns = 0;

        private enum MovementType
        {
            None,
            RegularPolygon,
            FigureEight,
            Random,
        }

        private enum MovementTrigger
        {
            Constant,
            WhenPlayerMoving
        }

        [System.Serializable]
        class MovementOptions
        {
            public MovementType movementType;
            public MovementTrigger movementTrigger;
            public float speed;
            public int numberOfTurnPoints;
            public float maxSecondsInDirection;
            public int direction;
            public float chaseRadius;

            public void SetDirection(int newDirection)
            {
                direction = newDirection % 360;
            }

            public void SetChaseRadius(float newChaseRadius)
            {
                chaseRadius = Mathf.Clamp(newChaseRadius, 0, 5);
            }

            public void SetMaxSecondsInDirection(float newMaxSecondsInDirection)
            {
                maxSecondsInDirection = Mathf.Clamp(newMaxSecondsInDirection, 0, 10);
            }

            public void SetNumberOfTurnPoints(int newNumberOfTurnPoints)
            {
                // numberOfTurnPoints = Mathf.Clamp(newNumberOfTurnPoints, 2, 10);
            }
        }

        [System.Serializable]
        private class BattleEnterOptions
        {
            public byte maxNumberOfCohorts;
            public RangeInt Range;
            public BattleUnit[] cohortOptions;
        }

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            battleUnit = GetComponent<BattleUnit>();

            turnPositive = Random.Range(0, 2) == 0;

            CutScene.OnEnable += OnCutScene;
        }

        private void OnDestroy()
        {
            CutScene.OnEnable -= OnCutScene;
        }

        private void OnCutScene()
        {
            rb.velocity = Vector2.zero;
        }

        void Update()
        {
            if (!PlayerController.playerController.CanAttack) return;
            if (CutScene.Enabled) return;

            Vector2 delta = GetDelta(transform.position, PlayerController.playerController.transform.position);
            delta = delta.normalized;
            rb.velocity = movementOptions.speed * (Vector3)delta;

            if (turn)
            {
                timeInDirection = 0;
                turn = false;
            }
            else
            {
                timeInDirection += Time.deltaTime;
                if (timeInDirection >= movementOptions.maxSecondsInDirection) turn = true;
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            turn = true;

            if (!PlayerController.playerController.CanAttack) return;
            if (!collision.collider.CompareTag("Player")) return;
            if (CutScene.Enabled) return;

            StartBattle();
        }

        private void StartBattle()
        {
            BattleUnit[] cohorts = new BattleUnit[Random.Range(0, battleEnterOptions.maxNumberOfCohorts + 1)];
            for (int i = 0; i < cohorts.Length; i++)
            {
                if (Random.Range(0, battleEnterOptions.cohortOptions.Length + 1) == 0) cohorts[i] = battleUnit;
                else
                    cohorts[i] =
                        battleEnterOptions.cohortOptions[Random.Range(0, battleEnterOptions.cohortOptions.Length)];
            }

            BattleUnit[] battleUnits = new BattleUnit[cohorts.Length + 1];
            cohorts.CopyTo(battleUnits, 1);
            battleUnits[0] = battleUnit;

            GameManager.StartBattle(battleUnits, gameObject);
        }

        private Vector2 GetDelta(Vector2 position, Vector2 target)
        {
            float distanceToPlayer = Vector2.Distance(target, position);

            if (movementOptions.movementTrigger == MovementTrigger.WhenPlayerMoving)
                if (!PlayerController.playerController.moving)
                    return Vector2.zero;

            if (distanceToPlayer <= movementOptions.chaseRadius)
            {
                Vector2 delta = target - position;
                movementOptions.direction = (int)(Mathf.Atan2(delta.x, delta.y) * Mathf.Rad2Deg);
                timeInDirection = 0;
            }
            else
            {
                if (movementOptions.movementType == MovementType.None) return Vector2.zero;

                if (turn)
                {
                    bool overrideToZero = false;
                    numberOfTurns += 1;
                    if (numberOfTurns >= movementOptions.numberOfTurnPoints)
                    {
                        if (movementOptions.movementType == MovementType.FigureEight)
                        {
                            turnPositive = !turnPositive;
                            overrideToZero = true;
                        }

                        numberOfTurns = 0;
                    }

                    int deg;
                    if (overrideToZero)
                    {
                        deg = 0;
                    }
                    else
                    {
                        if (movementOptions.movementType == MovementType.Random) deg = Random.Range(0, 360);
                        else deg = 360 / movementOptions.numberOfTurnPoints * (turnPositive ? 1 : -1);
                    }

                    movementOptions.SetDirection(movementOptions.direction + deg);
                }
            }

            return (Vector2)GetAngledVector();
        }

        private Vector3 GetAngledVector(bool trueForward = false)
        {
            Vector3 vector = Quaternion.Euler(0, 0, -movementOptions.direction) * Vector3.up;
            if (!trueForward)
                vector = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 10) * 40 * movementOptions.speed) * vector;
            vector.Normalize();
            return vector;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, movementOptions.chaseRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position,
                transform.position + GetAngledVector(true) * Mathf.Clamp(movementOptions.chaseRadius, 0.5f, 5));
            Gizmos.DrawLine(transform.position,
                transform.position + GetAngledVector() * Mathf.Clamp(movementOptions.chaseRadius / 2, 0.25f, 5));
        }

        private void OnValidate()
        {
            movementOptions.SetDirection(movementOptions.direction);
            movementOptions.SetChaseRadius(movementOptions.chaseRadius);
            movementOptions.SetMaxSecondsInDirection(movementOptions.maxSecondsInDirection);
            movementOptions.SetNumberOfTurnPoints(movementOptions.numberOfTurnPoints);
        }
    }
}
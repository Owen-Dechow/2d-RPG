using System.Collections.Generic;
using Managers.CutScene;
using UnityEngine;
using Data;

namespace Controllers
{
    public class AnimPlus : MonoBehaviour
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        private SpriteRenderer spr;

        private Rigidbody2D rb;

        private Direction currentDirection = Direction.Down;
        private Vector2 movementDelta;

        private float animationTime;

        public AnimationScriptable currentState;
        public AnimationScriptable moveUp;
        public AnimationScriptable moveDown;
        public AnimationScriptable moveHorizontal;

        public AnimationScriptable idleUp;
        public AnimationScriptable idleDown;
        public AnimationScriptable idleHorizontal;

        public bool speedAdded = false;
        public bool useRigidBody = true;

        private void Start()
        {
            spr = GetComponent<SpriteRenderer>();

            try { rb = GetComponent<Rigidbody2D>(); }
            catch { rb = null; }
        }

        private void LateUpdate()
        {
            spr.flipX = currentDirection == Direction.Left;
            float animSpeed = speedAdded ? 1.75f : 1;

            if (useRigidBody)
            {
                if (rb == null)
                {
                    Debug.LogError("There is no rigid body on this sprite");
                }
                else
                {
                    movementDelta = rb.velocity;
                }
            }

            bool idle = movementDelta.magnitude == 0;
            if (!idle)
                SetDirection();

            if (idle)
            {
                switch (currentDirection)
                {
                    case Direction.Up: SetAnimationTo(idleUp); break;
                    case Direction.Down: SetAnimationTo(idleDown); break;
                    case Direction.Left:
                    case Direction.Right:
                        SetAnimationTo(idleHorizontal); break;
                }
            }
            else
            {
                switch (currentDirection)
                {
                    case Direction.Up: SetAnimationTo(moveUp); break;
                    case Direction.Down: SetAnimationTo(moveDown); break;
                    case Direction.Left:
                    case Direction.Right:
                        SetAnimationTo(moveHorizontal); break;
                }
            }

            animationTime += Time.deltaTime * animSpeed;
            spr.sprite = currentState.GetSprite(animationTime);
        }

        private void SetDirection()
        {
            if (CutScene.Enabled) return;
            if (movementDelta.x > Mathf.Abs(movementDelta.y) * .5) currentDirection = Direction.Right;
            else if (movementDelta.x < Mathf.Abs(movementDelta.y) * -0.5) currentDirection = Direction.Left;
            else if (movementDelta.y > 0) currentDirection = Direction.Up;
            else if (movementDelta.y < 0) currentDirection = Direction.Down;
        }

        private void SetAnimationTo(AnimationScriptable animationClip)
        {
            if (currentState == animationClip) return;

            animationTime = 0;
            currentState = animationClip;
        }

        public static Direction RandomDirection(bool includeX = true, bool includeY = true)
        {
            List<Direction> directions = new();

            if (includeX)
            {
                directions.Add(Direction.Left);
                directions.Add(Direction.Right);
            }

            if (includeY)
            {
                directions.Add(Direction.Up);
                directions.Add(Direction.Down);
            }

            return directions[Random.Range(0, directions.Count)];
        }

        public void SetDelta(Vector2 delta)
        {
            movementDelta = delta;
        }
        public void SetDirection(Direction direction)
        {
            currentDirection = direction;
        }
        public void SetUseRigidBody(bool value)
        {
            useRigidBody = value;
        }
    }
}

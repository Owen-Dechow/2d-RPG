using UnityEngine;

public class AnimPlus : MonoBehaviour
{
    public enum Direction
    {
        up,
        down,
        left,
        right
    }

    private Animator anim;
    private SpriteRenderer spr;
    private AnimationClip currentState;
    private Rigidbody2D rb;

    private Direction currentDirection = Direction.down;
    private Vector2 movementDelta;

    public AnimationClip moveUp;
    public AnimationClip moveDown;
    public AnimationClip moveHorizontal;

    public AnimationClip idleUp;
    public AnimationClip idleDown;
    public AnimationClip idleHorizontal;

    public bool speedAdded = false;
    public bool useRigidBody = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();

        try { rb = GetComponent<Rigidbody2D>(); }
        catch { rb = null; }
    }

    private void LateUpdate()
    {
        spr.flipX = currentDirection == Direction.left;
        anim.speed = speedAdded ? 1.75f : 1;
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
        if (!idle) SetDirection();

        if (idle)
        {
            switch (currentDirection)
            {
                case Direction.up: SetAnimationTo(idleUp); break;
                case Direction.down: SetAnimationTo(idleDown); break;
                case Direction.left: SetAnimationTo(idleHorizontal); break;
                case Direction.right: SetAnimationTo(idleHorizontal); break;
            }
        }
        else
        {
            switch (currentDirection)
            {
                case Direction.up: SetAnimationTo(moveUp); break;
                case Direction.down: SetAnimationTo(moveDown); break;
                case Direction.left: SetAnimationTo(moveHorizontal); break;
                case Direction.right: SetAnimationTo(moveHorizontal); break;
            }
        }
    }

    private void SetDirection()
    {
        if (Time.timeScale == 0) return;
        if (movementDelta.x > 0) currentDirection = Direction.right;
        else if (movementDelta.x < 0) currentDirection = Direction.left;
        else if (movementDelta.y > 0) currentDirection = Direction.up;
        else if (movementDelta.y < 0) currentDirection = Direction.down;
    }

    private void SetAnimationTo(AnimationClip animation)
    {
        if (currentState == animation) return;

        anim.Play(animation.name);
        currentState = animation;
    }

    public void SetDelta(Vector2 delta)
    {
        movementDelta = delta;
    }
    public void SetDirection(Direction direction)
    {
        currentDirection = direction;
    }
}

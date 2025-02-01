using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    
    public Transform ballHoldPosition;
    private bool hasBall = false;

    private string lastDirection = "East";

    private bool isPassing = false;
    private Vector3 passDirection = Vector3.zero;

    private string currentAnimation = string.Empty;

    private Vector2 topLeft = new Vector2(-11.5f, 2.88f);
    private Vector2 bottomLeft = new Vector2(-14.44f, -2.88f);
    private Vector2 topRight = new Vector2(11.5f, 2.88f);
    private Vector2 bottomRight = new Vector2(14.44f, -2.88f);

    private bool isThrowing = false;
    private Vector3 throwDirection = Vector3.zero;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Only process input if this is the active player
        if (GameManager.Instance.activePlayer == this)
        {
            // Get player movement input (using the arrow keys for simplicity)
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
        }

        // Update animations based on direction
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        // Move player only if they have the ball
        // if (hasBall)
        // {
            // rb.linearVelocity = moveInput.normalized * moveSpeed;

            // // Calculate movement
            // Vector2 desiredPosition = rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
            // // Clamp the position to within the court boundaries
            // float clampedX = Mathf.Clamp(desiredPosition.x, bottomLeft.x, topRight.x);
            // float clampedY = Mathf.Clamp(desiredPosition.y, bottomLeft.y, topLeft.y);
            // // Apply the clamped position to the Rigidbody2D
            // rb.MovePosition(new Vector2(clampedX, clampedY));

        if (GameManager.Instance.activePlayer == this)
        {
            // Calculate movement
            Vector2 desiredPosition = rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime;

            // Check if the new position is inside the court polygon
            if (IsInsideCourt(desiredPosition))
            {
                rb.MovePosition(desiredPosition);
            }
        }
        // }
        // else
        // {
        //     rb.linearVelocity = Vector2.zero;  // Stop movement if the player doesn't have the ball
        // }
    }

    public void PassBall(Vector3 direction)
    {
        if (isPassing) return;

        isPassing = true;
        hasBall = false;

        StartCoroutine(PlayBallPassAnimation(direction));
    }

    private IEnumerator PlayBallPassAnimation(Vector3 direction)
    {
        passDirection = direction;

        string ballPassAnimationName = GetPassAnimationName(passDirection);

        if (!string.IsNullOrEmpty(ballPassAnimationName))
        {
            currentAnimation = ballPassAnimationName;
            animator.Play(currentAnimation);

            // Wait for the animation duration
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }

    private string GetPassAnimationName(Vector3 direction)
    {
        if (direction.y > 0.5f && Mathf.Abs(direction.x) <= 0.5f)
            return "PassReceiveNorth";
        else if (direction.y > 0.5f && direction.x > 0.5f)
            return "PassReceiveNorthEast";
        else if (direction.y > 0.5f && direction.x < -0.5f)
            return "PassReceiveNorthWest";
        else if (direction.y < -0.5f && Mathf.Abs(direction.x) <= 0.5f)
            return "PassReceiveSouth";
        else if (direction.y < -0.5f && direction.x > 0.5f)
            return "PassReceiveSouthEast";
        else if (direction.y < -0.5f && direction.x < -0.5f)
            return "PassReceiveSouthWest";
        else if (Mathf.Abs(direction.y) <= 0.5f && direction.x > 0.5f)
            return "PassReceiveEast";
        else if (Mathf.Abs(direction.y) <= 0.5f && direction.x < -0.5f)
            return "PassReceiveWest";

        return string.Empty;
    }

    public void TriggerNoBallAnimation()
    {
        string noBallPassAnimationName = GetNoBallPassAnimationName(passDirection);
        
        if (!string.IsNullOrEmpty(noBallPassAnimationName))
        {
            currentAnimation = noBallPassAnimationName;
            animator.Play(currentAnimation);
        }
    }

    private string GetNoBallPassAnimationName(Vector3 direction) {
        if (direction.y > 0.5f && Mathf.Abs(direction.x) <= 0.5f)
            return "NoBallPassReceiveNorth";
        else if (direction.y > 0.5f && direction.x > 0.5f)
            return "NoBallPassReceiveNorthEast";
        else if (direction.y > 0.5f && direction.x < -0.5f)
            return "NoBallPassReceiveNorthWest";
        else if (direction.y < -0.5f && Mathf.Abs(direction.x) <= 0.5f)
            return "NoBallPassReceiveSouth";
        else if (direction.y < -0.5f && direction.x > 0.5f)
            return "NoBallPassReceiveSouthEast";
        else if (direction.y < -0.5f && direction.x < -0.5f)
            return "NoBallPassReceiveSouthWest";
        else if (Mathf.Abs(direction.y) <= 0.5f && direction.x > 0.5f)
            return "NoBallPassReceiveEast";
        else if (Mathf.Abs(direction.y) <= 0.5f && direction.x < -0.5f)
            return "NoBallPassReceiveWest";

        return string.Empty;
    }

    public void TriggerThrowAnimation(Vector3 direction)
    {
        if (isThrowing) return;
        StartCoroutine(PlayThrowAnimation(direction));
    }

    private IEnumerator PlayThrowAnimation(Vector3 direction)
    {
        isThrowing = true;
        throwDirection = direction;

        string throwAnimationName = GetThrowAnimationName(throwDirection);

        if (!string.IsNullOrEmpty(throwAnimationName))
        {
            currentAnimation = throwAnimationName;
            animator.Play(currentAnimation);

            // Wait for the animation duration
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }

        isThrowing = false;
    }

    private string GetThrowAnimationName(Vector3 direction)
    {
        if (direction.y > 0.5f && Mathf.Abs(direction.x) <= 0.5f)
            return "JumpShootNorth"; // StandShootNorth
        else if (direction.y > 0.5f && direction.x > 0.5f)
            return "JumpShootNorthEast"; // StandShootNorthEast
        else if (direction.y > 0.5f && direction.x < -0.5f)
            return "JumpShootNorthWest"; // StandShootNorthWest
        else if (direction.y < -0.5f && Mathf.Abs(direction.x) <= 0.5f)
            return "JumpShootSouth"; // StandShootSouth
        else if (direction.y < -0.5f && direction.x > 0.5f)
            return "JumpShootSouthEast"; // StandShootSouthEast
        else if (direction.y < -0.5f && direction.x < -0.5f)
            return "JumpShootSouthWest"; // StandShootSouthWest
        else if (Mathf.Abs(direction.y) <= 0.5f && direction.x > 0.5f)
            return "JumpShootEast"; // StandShootEast
        else if (Mathf.Abs(direction.y) <= 0.5f && direction.x < -0.5f)
            return "JumpShootWest"; // StandShootWest

        return string.Empty;
    }

    private void UpdateAnimation()
    {
        if (isPassing || isThrowing)
        {
            return; // Avoid running other animation logic in this frame
        }

        // Determine the direction of movement
        string direction = GetMovementDirection();

        // Update the last direction if player is moving
        if (moveInput != Vector2.zero)
        {
            lastDirection = direction; // Store the direction when moving
        }

        // Handle animations based on whether the player has the ball or not
        string animationName = hasBall 
        ? (moveInput != Vector2.zero ? "Dribbling" + direction : "BallIdle" + lastDirection)
        : (moveInput != Vector2.zero ? "Running" + direction : "Idle" + lastDirection);
        
        // Debug.Log("Playing animation: " + animationName);
        animator.Play(animationName);
    }

    private string GetMovementDirection()
    {
        if (moveInput == Vector2.zero)
        {
            return lastDirection;
        }

        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        {
            return moveInput.x > 0 ? "East" : "West"; 
        }
        else if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
        {
            return moveInput.y > 0 ? "North" : "South"; 
        }
        else
        {
            if (moveInput.x > 0 && moveInput.y > 0)
                return "NorthEast";
            else if (moveInput.x < 0 && moveInput.y > 0)
                return "NorthWest";
            else if (moveInput.x > 0 && moveInput.y < 0)
                return "SouthEast";
            else if (moveInput.x < 0 && moveInput.y < 0)
                return "SouthWest";
        }

        return "East";
    }

    private bool IsInsideCourt(Vector2 point)
    {
        Vector2[] courtBoundary = { topLeft, bottomLeft, bottomRight, topRight };

        int crossings = 0;
        for (int i = 0; i < courtBoundary.Length; i++)
        {
            Vector2 a = courtBoundary[i];
            Vector2 b = courtBoundary[(i + 1) % courtBoundary.Length];

            // Ray-casting method: Count how many times a horizontal ray crosses the edges
            if (((a.y > point.y) != (b.y > point.y)) && 
                (point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y) + a.x))
            {
                crossings++;
            }
        }

        return (crossings % 2) == 1; // Odd crossings mean inside
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log($"Is Passing: {isPassing}");
        if (other.CompareTag("Ball") && !hasBall && !isPassing)
        {
            // Debug.Log("Player collided with Ball!");
            TakeBall();
            Destroy(other.gameObject);
        }
    }

    public bool HasBall()
    {
        return hasBall;
    }

    public void TakeBall()
    {
        hasBall = true;
    }

    public void DropBall()
    {
        hasBall = false;
    }

    public bool IsThrowing()
    {
        return isThrowing;
    }

    public bool IsPassing()
    {
        return isPassing;
    }

    public void Unfreeze()
    {
        isPassing = false;
    }

}

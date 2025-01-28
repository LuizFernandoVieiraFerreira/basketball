using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    
    public Transform ballHoldPosition; // Position where the ball is held
    private bool hasBall = true; // Whether the player has the ball

    private string lastDirection = "East";

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Only process input if this is the active player
        if (GameManager.Instance.activePlayer != this)
        {
            return;
        }
        // Get player movement input (using the arrow keys for simplicity)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Update animations based on direction
        UpdateAnimation();

        if (Input.GetKeyDown(KeyCode.Space) && hasBall)
        {
            PassBall();
        }
    }

    private void PassBall()
    {
        DropBall();
        StopMovement();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        // Move player only if they have the ball
        if (hasBall)
        {
            rb.linearVelocity = moveInput.normalized * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;  // Stop movement if the player doesn't have the ball
        }
    }

    private void UpdateAnimation()
    {
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
        
        Debug.Log("Playing animation: " + animationName);
        animator.Play(animationName);
    }

    private string GetMovementDirection()
    {
        if (moveInput == Vector2.zero)
        {
            return lastDirection; // Idle state
        }

        // Determine direction based on x and y inputs (adjust these as necessary)
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        {
            // Horizontal movement
            return moveInput.x > 0 ? "East" : "West"; 
        }
        else if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
        {
            // Vertical movement
            return moveInput.y > 0 ? "North" : "South"; 
        }
        else
        {
            // Diagonal movement
            if (moveInput.x > 0 && moveInput.y > 0)
                return "NorthEast";
            else if (moveInput.x < 0 && moveInput.y > 0)
                return "NorthWest";
            else if (moveInput.x > 0 && moveInput.y < 0)
                return "SouthEast";
            else if (moveInput.x < 0 && moveInput.y < 0)
                return "SouthWest";
        }

        return "East"; // Default to idle if nothing matches
    }

    public void SetAsActive()
    {
        // Enable input and special visuals for the active player
        Debug.Log($"{name} is now the active player!");
    }

    public void SetAsInactive()
    {
        // Disable input for inactive players
        Debug.Log($"{name} is now inactive.");
    }

    public bool HasBall()
    {
        return hasBall;
    }

    public void TakeBall()
    {
        hasBall = true;
        Debug.Log($"{name} now has the ball!");
    }

    public void DropBall()
    {
        hasBall = false;
        Debug.Log($"{name} dropped the ball!");
    }

    private void StopMovement()
    {
        moveInput = Vector2.zero; // Stop player movement when they drop or pass the ball
    }
}

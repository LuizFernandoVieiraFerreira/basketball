using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    // public GameObject ballPrefab;
    // public GameObject ball; // Reference to the ball GameObject
    public Transform ballHoldPosition; // Position where the ball is held
    private bool hasBall = true; // Whether the player has the ball

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

        // Handle ball dribbling or detachment
        if (hasBall)
        {
            DribbleBall();
        }
        else
        {
            // Ball is free-floating or passed; handle separately
        }

        if (Input.GetKeyDown(KeyCode.Space) && hasBall)
        {
            ShootBall();
        }
    }

    private void DribbleBall()
    {
        // Make the ball follow the player at the ballHoldPosition
        // ball.transform.position = ballHoldPosition.position;
    }

    private void ShootBall()
    {
        // Detach the ball and give it a force
        hasBall = false;

        // Instantiate the ball prefab
        // GameObject ball = Instantiate(ballPrefab, ballHoldPosition.position, Quaternion.identity);

        // Add force to the ball for shooting
        // Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        // ballRb.AddForce(Vector2.up * 500f); // Example force; adjust as needed

        // Optionally destroy the ball after some time
        // Destroy(ball, 3f);

        // ball.transform.parent = null;

        // Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        // ballRb.isKinematic = false;
        // ballRb.linearVelocity = new Vector2(0, 5f); // Example shoot velocity
    }

    private void FixedUpdate()
    {
        // Move player
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    private void UpdateAnimation()
    {
        // Determine the direction of movement
        string direction = GetMovementDirection();

        // string animationName = moveInput != Vector2.zero ? "Running" + direction : "Idle" + direction;
        Debug.Log("Has ball: " + hasBall);

        string animationName = hasBall 
        ? (moveInput != Vector2.zero ? "Dribbling" + direction : "BallIdle" + direction)
        : (moveInput != Vector2.zero ? "Running" + direction : "Idle" + direction);
        Debug.Log("Playing animation: " + animationName);

        animator.Play(animationName);
    }

    private string GetMovementDirection()
    {
        if (moveInput == Vector2.zero)
        {
            return "East"; // Idle state
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
}

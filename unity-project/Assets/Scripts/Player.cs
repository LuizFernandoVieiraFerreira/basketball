using UnityEngine;

public class Payer : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get player movement input (using the arrow keys for simplicity)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Update animations based on direction
        UpdateAnimation();
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

        string animationName = moveInput != Vector2.zero ? "Running" + direction : "Idle" + direction;
        Debug.Log("Playing animation: " + animationName);

        // Set the appropriate animation based on the direction and running state
        if (moveInput != Vector2.zero)
        {
            animator.Play("Running" + direction);
        }
        else
        {
            animator.Play("Idle" + direction);
        }
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
}

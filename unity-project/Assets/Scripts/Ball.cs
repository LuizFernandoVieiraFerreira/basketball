using UnityEngine;

public class Ball : MonoBehaviour
{

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Example: Handle collisions (e.g., with the hoop, ground, or players)
        // if (collision.gameObject.CompareTag("Hoop"))
        // {
        //     Debug.Log("Scored!");
        // }
    }
}
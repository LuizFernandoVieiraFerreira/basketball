using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The target to follow (active player)
    public Vector3 offset; // Offset from the target's position
    public float smoothSpeed = 5f; // Speed at which the camera follows the target

    public float minX = -11f;
    public float maxX = 11f;

    private void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired X position
            float targetX = target.position.x + offset.x;

            // Clamp the X position within the defined limits
            float clampedX = Mathf.Clamp(targetX, minX, maxX);

            // Create the new position, keeping Y and Z unchanged
            Vector3 desiredPosition = new Vector3(clampedX, transform.position.y, transform.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            transform.position = smoothedPosition;
        }
    }

    // Function to set a new target
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}

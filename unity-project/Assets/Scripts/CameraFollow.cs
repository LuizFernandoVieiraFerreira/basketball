using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The target to follow (active player)
    public Vector3 offset; // Offset from the target's position
    public float smoothSpeed = 5f; // Speed at which the camera follows the target

    private void LateUpdate()
    {
        if (target != null)
        {
            // Smoothly interpolate the camera's position to the target's position with offset
            Vector3 desiredPosition = target.position + offset;
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

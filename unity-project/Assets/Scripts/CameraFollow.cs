using UnityEngine;
using UnityEngine.U2D;

public class CameraFollow : MonoBehaviour
{
    // public Transform target;
    // public Vector3 offset;
    // public float minX = -11f;
    // public float maxX = 11f;

    // private PixelPerfectCamera pixelPerfectCamera;

    // private void Awake()
    // {
    //     pixelPerfectCamera = Camera.main?.GetComponent<PixelPerfectCamera>();
    //     if (pixelPerfectCamera == null)
    //     {
    //         Debug.LogError("PixelPerfectCamera component not found on Main Camera!");
    //     }
    // }

    // private void Start()
    // {
    //     Debug.Log($"Camera.main: {Camera.main}");
    //     if (Camera.main != null)
    //     {
    //         Debug.Log($"PixelPerfectCamera found? {Camera.main.GetComponent<PixelPerfectCamera>()}");
    //     }
    // }

    // private void LateUpdate()
    // {
    //     if (target == null) return;

    //     // Step 1: Get target position
    //     float targetX = Mathf.Clamp(target.position.x + offset.x, minX, maxX);
    //     Vector3 desiredPosition = new Vector3(targetX, transform.position.y, transform.position.z);

    //     // Step 2: Pixel snap the position
    //     transform.position = PixelSnap(desiredPosition);
    // }

    // private Vector3 PixelSnap(Vector3 position)
    // {
    //     float ppu = pixelPerfectCamera.assetsPPU;
    //     float snapX = Mathf.Round(position.x * ppu) / ppu;
    //     float snapY = Mathf.Round(position.y * ppu) / ppu;
    //     float snapZ = Mathf.Round(position.z * ppu) / ppu;

    //     return new Vector3(snapX, snapY, snapZ);
    // }

    // public void SetTarget(Transform newTarget)
    // {
    //     target = newTarget;
    // }


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

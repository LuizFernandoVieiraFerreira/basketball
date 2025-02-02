using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject playerPrefab;
    public GameObject ballPrefab;

    public Transform[] spawnPoints;
    public List<Player> teamPlayers;
    
    public Player activePlayer;
    private GameObject activeBall;

    public Hoop leftHoop;
    public Hoop rightHoop;

    public Transform leftHoopTransform;
    public Transform rightHoopTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SpawnTeamPlayers();

        if (teamPlayers.Count > 0)
        {
            SetActivePlayerWithBall(teamPlayers[0]);
        }
    }

    private void SpawnTeamPlayers()
    {
        // Example: Spawn players at predefined spawn points
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject playerObj = Instantiate(playerPrefab, spawnPoints[i].position, Quaternion.identity);
            Player player = playerObj.GetComponent<Player>();

            if (player != null)
            {
                teamPlayers.Add(player); // Add to the teamPlayers list
                player.name = $"Player {i + 1}"; // Optional: Assign a unique name
            }
        }
    }

    private void Update()
    {
        // Debug.Log($"Active player {activePlayer} - isPassing: {activePlayer.IsPassing()}");

        //  ||
        // (Input.GetButtonDown("Fire1") && activePlayer.HasBall())
        if ((Input.GetKeyDown(KeyCode.Space) && activePlayer.HasBall()) ||
        (Input.GetKeyDown(KeyCode.Z) && activePlayer.HasBall()))
        {
            PassBall();
        }
        //  ||
        // (Input.GetButtonDown("Fire2") && activePlayer.HasBall())
        else if ((Input.GetKeyDown(KeyCode.X) && activePlayer.HasBall()))
        {
            ThrowBall();
        }
    }

    public void SetActivePlayer(Player player)
    {
        activePlayer = player;

         // Update the camera to follow the new active player
        Camera.main.GetComponent<CameraFollow>().SetTarget(activePlayer.transform);
    }

    public void SetActivePlayerWithBall(Player player)
    {
        activePlayer = player;
        activePlayer.TakeBall();

         // Update the camera to follow the new active player
        Camera.main.GetComponent<CameraFollow>().SetTarget(activePlayer.transform);
    }

    private void PassBall()
    {
        Player targetPlayer = FindClosestTeammate();

        if (targetPlayer != null)
        {
            // Calculate the direction of the pass
            Vector3 direction = (targetPlayer.transform.position - activePlayer.transform.position).normalized;

            // Trigger the pass animation on the active player
            activePlayer.PassBall(direction);

            // Start the process of waiting for the animation to finish
            StartCoroutine(WaitForPassAnimationAndInstantiateBall(targetPlayer));
            // Instantiate the ball and move it to the target player
            // activeBall = Instantiate(ballPrefab, activePlayer.ballHoldPosition.position, Quaternion.identity);
            // StartCoroutine(MoveBallToTarget(activeBall, targetPlayer));
        }
    }

    private IEnumerator WaitForPassAnimationAndInstantiateBall(Player targetPlayer)
    {
        // // Wait until the pass animation is completed in the Player
        // while (activePlayer.IsPassing())
        // {
        //     yield return null; // Wait until the passing state is no longer true
        // }

        // Assuming you have an Animator attached to your player and you're using it to control the animation
        Animator animator = activePlayer.GetComponent<Animator>();

        // Wait until the animation has reached the last frame (or is done playing)
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) // normalizedTime reaches 1 when the animation finishes
        {
            yield return null; // Wait until the animation reaches the last frame
        }

        float yOffset = 12f / 16f;
        Vector3 spawnPosition = activePlayer.ballHoldPosition.position + new Vector3(0, yOffset, 0);

        // Now instantiate the ball and move it to the target player
        activeBall = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        activePlayer.TriggerNoBallAnimation();        
        StartCoroutine(MoveBallToTarget(activeBall, targetPlayer));

        while (!targetPlayer.HasBall())
        {
            yield return null; // Wait until the ball is received by the target player
        }

        activePlayer.Unfreeze();

        SetActivePlayerWithBall(targetPlayer);
        // activePlayer.SetIsPassing(false);
    }

    private Player FindClosestTeammate()
    {
        Player closestTeammate = null;
        float closestDistance = float.MaxValue;

        foreach (Player teammate in teamPlayers)
        {
            if (teammate == activePlayer) continue; // Skip the current player

            float distance = Vector3.Distance(activePlayer.transform.position, teammate.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTeammate = teammate;
            }
        }

        return closestTeammate;
    }

    private System.Collections.IEnumerator MoveBallToTarget(GameObject ball, Player targetPlayer)
    {
        Vector3 startPosition = ball.transform.position;
        Vector3 targetPosition = targetPlayer.ballHoldPosition.position;

        float travelTime = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < travelTime)
        {
            ball.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / travelTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ball.transform.position = targetPosition;

        // Destroy the ball and assign possession to the target player
        Destroy(ball);
        targetPlayer.TakeBall();
    }

    private void ThrowBall()
    {
        Transform targetHoop = DetermineTargetHoop();
        
        if (targetHoop != null)
        {
            // Calculate the direction of the pass
            Vector3 direction = (targetHoop.transform.position - activePlayer.transform.position).normalized;

            activePlayer.TriggerThrowAnimation(direction);

            activePlayer.DropBall();

            StartCoroutine(WaitForThrowAnimationAndInstantiateBall(targetHoop));
            // activeBall = Instantiate(ballPrefab, activePlayer.ballHoldPosition.position, Quaternion.identity);
            // StartCoroutine(MoveBallToHoop(activeBall, targetHoop));
        }
    }

    private Transform DetermineTargetHoop()
    {
        // Logic to decide which hoop to target (example: closest hoop)
        Transform closestHoop = null;
        float distanceToLeftHoop = Vector3.Distance(activePlayer.transform.position, leftHoopTransform.position);
        float distanceToRightHoop = Vector3.Distance(activePlayer.transform.position, rightHoopTransform.position);

        if (distanceToLeftHoop < distanceToRightHoop)
        {
            closestHoop = leftHoopTransform;
        }
        else
        {
            closestHoop = rightHoopTransform;
        }

        return closestHoop;
    }

    private IEnumerator WaitForThrowAnimationAndInstantiateBall(Transform targetHoop)
    {
        // Wait until the throw animation is completed in the Player
        while (activePlayer.IsThrowing())
        {
            yield return null; // Wait until the throwing state is no longer true
        }

        float yOffset = 27f / 16f;
        Vector3 spawnPosition = activePlayer.ballHoldPosition.position + new Vector3(0, yOffset, 0);

        // Now instantiate the ball and move it to the hoop
        activeBall = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        StartCoroutine(MoveBallToHoop(activeBall, targetHoop));
    }

    private System.Collections.IEnumerator MoveBallToHoop(GameObject ball, Transform hoop)
    {
        Vector3 startPosition = ball.transform.position;
        Vector3 targetPosition = hoop.position;

        float travelTime = 1f; // Adjust for speed
        float elapsedTime = 0f;

        // Control how high the ball goes depending on the distance
        float arcHeight = Mathf.Clamp(Vector3.Distance(startPosition, targetPosition) / 2, 1f, 3f);

        while (elapsedTime < travelTime)
        {
            float t = elapsedTime / travelTime; // 0 to 1

            // X and Z move linearly
            float x = Mathf.Lerp(startPosition.x, targetPosition.x, t);
            float z = Mathf.Lerp(startPosition.z, targetPosition.z, t);

            // Y follows a parabola
            float y = Mathf.Lerp(startPosition.y, targetPosition.y, t) + arcHeight * Mathf.Sin(t * Mathf.PI);

            ball.transform.position = new Vector3(x, y, z);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Introduce a small random variation to simulate human error
        float missOffset = Random.Range(-0.7f, 0.7f); // Adjust for difficulty
        ball.transform.position = targetPosition + new Vector3(missOffset, 0, missOffset);

        // Check if the ball successfully scores
        // CheckIfScored(ball, hoop);

        // Determine if the shot is successful
        float distance = Vector3.Distance(ball.transform.position, hoop.position);
        if (distance <= 0.5f) // Successful shot
        {
            Debug.Log($"Scored in {hoop.name}!");
            Destroy(ball);
            leftHoop.TriggerScoreAnimation();

            if (teamPlayers.Count > 0)
            {
                Player firstPlayer = teamPlayers[0];
                SetActivePlayerWithBall(firstPlayer);
            }
        }
        else
        {
            Debug.Log("Missed the shot!");

            StartCoroutine(HandleMiss(ball, hoop));
        }
    }

    private IEnumerator HandleMiss(GameObject ball, Transform hoop)
    {
        Vector2 startPosition = ball.transform.position;
        Vector2 missTarget;

        // // Choose one of three predefined miss zones
        // int missType = Random.Range(0, 3);

        // if (missType == 0) // Short Front Rim
        // {
        //     missTarget = new Vector2(hoop.position.x, hoop.position.y - 1f); // Lands near the hoop
        // }
        // else if (missType == 1) // Back Rim Bounce
        // {
        //     missTarget = new Vector2(hoop.position.x, hoop.position.y - 3f); // Lands near free-throw line
        // }
        // else // Side Rim Miss
        // {
        //     missTarget = new Vector2(hoop.position.x + Random.Range(-3f, 3f), hoop.position.y - 2f); // Lands left or right
        // }

        // Manually define two possible miss zones (e.g., near the rim or at a different angle)
        int missType = Random.Range(0, 2); // Only two options for deterministic behavior

        if (missType == 0) // Near the front rim
        {
            missTarget = new Vector3(hoop.position.x + 1f, hoop.position.y - 2f, hoop.position.z); // Slight miss near the hoop
        }
        else // Back or side miss
        {
            missTarget = new Vector3(hoop.position.x + Random.Range(2f, 3f), hoop.position.y - 2f, hoop.position.z); // Lands off-center
        }

        float travelTime = 0.7f;
        float elapsedTime = 0f;
        float arcHeight = 2f;

        while (elapsedTime < travelTime)
        {
            float t = elapsedTime / travelTime;

            // X and Y move linearly
            float x = Mathf.Lerp(startPosition.x, missTarget.x, t);
            float y = Mathf.Lerp(startPosition.y, missTarget.y, t) + arcHeight * Mathf.Sin(t * Mathf.PI);

            ball.transform.position = new Vector2(x, y);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        HandleRebound(ball);
    }

    private void HandleRebound(GameObject ball)
    {
        Player nearestPlayer = FindNearestPlayer(ball.transform.position);

        if (nearestPlayer != null)
        {
            // Debug.Log($"{nearestPlayer.name} grabbed the rebound!");
            Debug.Log($"Nearest player {nearestPlayer.name} activated!");
            SetActivePlayer(nearestPlayer);
            // nearestPlayer.TakeBall();
            // Destroy(ball);
        }
        else
        {
            Debug.Log("No one got the rebound!");
            // Destroy(ball, 2f);
            // Add a small random movement to keep the ball moving around
            StartCoroutine(MoveBallContinuing(ball));
        }
    }

    private IEnumerator MoveBallContinuing(GameObject ball)
    {
        Vector3 lastDirection = ball.GetComponent<Rigidbody>().linearVelocity.normalized;
        float maxDistance = 5f; // Max distance the ball can move in one step

        float travelTime = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < travelTime)
        {
            // Move the ball in the last direction or in a random direction
            Vector3 randomDirection = new Vector3(
                lastDirection.x + Random.Range(-0.1f, 0.1f),
                lastDirection.y + Random.Range(-0.1f, 0.1f),
                lastDirection.z + Random.Range(-0.1f, 0.1f)
            ).normalized;

            ball.transform.position += randomDirection * maxDistance * Time.deltaTime;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // After this, recheck if the closest player can get the ball
        Player nearestPlayer = FindNearestPlayer(ball.transform.position);
        if (nearestPlayer != null)
        {
            Debug.Log($"{nearestPlayer.name} is now close to the ball!");
            SetActivePlayer(nearestPlayer);
            // nearestPlayer.TakeBall();
            // Destroy(ball);
        }
    }

    private Player FindNearestPlayer(Vector3 ballPosition)
    {
        Player closestPlayer = null;
        float minDistance = float.MaxValue;

        foreach (Player player in teamPlayers)
        {
            float distance = Vector3.Distance(player.transform.position, ballPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }
}

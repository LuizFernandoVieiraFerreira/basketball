using UnityEngine;
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

    public Transform leftHoop;
    public Transform rightHoop;

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
            SetActivePlayer(teamPlayers[0]);
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
        if (Input.GetKeyDown(KeyCode.Space) && activePlayer.HasBall())
        {
            PassBall();
        }
        else if (Input.GetKeyDown(KeyCode.Z) && activePlayer.HasBall())
        {
            ThrowBall();
        }
    }

    public void SetActivePlayer(Player player)
    {
        if (activePlayer != null)
        {
            activePlayer.SetAsInactive();
        }

        activePlayer = player;
        activePlayer.SetAsActive();

         // Update the camera to follow the new active player
        Camera.main.GetComponent<CameraFollow>().SetTarget(activePlayer.transform);
    }

    private void PassBall()
    {
        Player targetPlayer = FindClosestTeammate();

        if (targetPlayer != null)
        {
            // Detach the ball from the active player
            activePlayer.DropBall();

            // Instantiate the ball and move it to the target player
            activeBall = Instantiate(ballPrefab, activePlayer.ballHoldPosition.position, Quaternion.identity);
            StartCoroutine(MoveBallToTarget(activeBall, targetPlayer));
        }
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
        SetActivePlayer(targetPlayer);
    }

    private void ThrowBall()
    {
        Transform targetHoop = DetermineTargetHoop();
        
        if (targetHoop != null)
        {
            activePlayer.DropBall();

            activeBall = Instantiate(ballPrefab, activePlayer.ballHoldPosition.position, Quaternion.identity);
            StartCoroutine(MoveBallToHoop(activeBall, targetHoop));
        }
    }

    private Transform DetermineTargetHoop()
    {
        // Logic to decide which hoop to target (example: closest hoop)
        Transform closestHoop = null;
        float distanceToLeftHoop = Vector3.Distance(activePlayer.transform.position, leftHoop.position);
        float distanceToRightHoop = Vector3.Distance(activePlayer.transform.position, rightHoop.position);

        if (distanceToLeftHoop < distanceToRightHoop)
        {
            closestHoop = leftHoop;
        }
        else
        {
            closestHoop = rightHoop;
        }

        return closestHoop;
    }

    private System.Collections.IEnumerator MoveBallToHoop(GameObject ball, Transform hoop)
    {
        Vector3 startPosition = ball.transform.position;
        Vector3 targetPosition = hoop.position;

        float travelTime = 1f; // Adjust time to make it feel realistic
        float elapsedTime = 0f;

        while (elapsedTime < travelTime)
        {
            ball.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / travelTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ball.transform.position = targetPosition;

        // Check if the ball successfully scores
        CheckIfScored(ball, hoop);

        Destroy(ball); // Clean up the ball after the throw
    }

    private void CheckIfScored(GameObject ball, Transform hoop)
    {
        float distance = Vector3.Distance(ball.transform.position, hoop.position);

        if (distance <= 0.5f) // Adjust threshold for scoring
        {
            Debug.Log($"Scored in {hoop.name}!");
        }
        else
        {
            Debug.Log("Missed the shot!");
        }
    }
}

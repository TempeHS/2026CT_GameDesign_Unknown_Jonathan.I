using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance;

    [Header("Lives")]
    public int livesPerRun = 3;
    public int currentLives;

    [Header("Respawn")]
    public Transform spawnPoint;
    public GameObject playerPrefab;
    private GameObject currentPlayer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartNewRun();
    }

    public void StartNewRun()
    {
        currentLives = livesPerRun;
        SpawnPlayer();
    }

    public void OnPlayerDeath()
    {
        currentLives--;

        if (currentLives <= 0)
        {
            Debug.Log("Run over — restarting run");
            StartNewRun();
        }
        else
        {
            Debug.Log("Player died — respawning");
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        if (currentPlayer != null)
            Destroy(currentPlayer);

        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
    }
}

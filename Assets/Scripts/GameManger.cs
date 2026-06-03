using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform startPoint;
    public GameObject playerPrefab;
    public CameraFollow cameraFollow;

    private GameObject playerInstance;

    void Start()
    {
        // Spawn player at Start
        playerInstance = Instantiate(playerPrefab, startPoint.position, Quaternion.identity);

        // Assign camera target
        cameraFollow.target = playerInstance.transform;
    }
}

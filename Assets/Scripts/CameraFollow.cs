using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Transform target
    {
        get => player;
        set => player = value;
    }
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private void ResolvePlayer()
    {
        if (RunManager.Instance != null)
        {
            Transform runPlayer = RunManager.Instance.CurrentPlayerTransform;
            if (runPlayer != null)
            {
                player = runPlayer;
                return;
            }
        }

        if (player == null)
        {
            PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
            if (playerMovement != null)
            {
                player = playerMovement.transform;
                return;
            }
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    void Awake()
    {
        ResolvePlayer();

        if (player != null && offset == Vector3.zero)
        {
            // Keep the camera's starting distance from the player if no offset was set.
            offset = transform.position - player.position;
        }
    }

    void LateUpdate()
    {
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            ResolvePlayer();
        }

        if (player == null)
        {
            return;
        }

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}

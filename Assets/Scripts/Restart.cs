using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPopup : MonoBehaviour
{
    public GameObject deathPanel;

    private void Start()
    {
        deathPanel.SetActive(false);
    }

    public void ShowDeathPopup()
    {
        deathPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

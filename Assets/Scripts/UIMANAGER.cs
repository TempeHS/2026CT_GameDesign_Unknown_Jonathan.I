using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Reference to the popup panel
    public GameObject panel;

    // Reference to the text inside the popup
    public TMP_Text popupText;

    private void Start()
    {
        // Hide the popup when the game starts
        panel.SetActive(false);
    }

    // Show the popup with a custom message
    public void ShowPopup(string message)
    {
        popupText.text = message;
        panel.SetActive(true);
    }

    // Restart the current scene
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

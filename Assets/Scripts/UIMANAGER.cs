using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text popupText;

    private void Start()
    {
        panel.SetActive(false);
    }

    public void ShowPopup(string message)
    {
        popupText.text = message;
        panel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

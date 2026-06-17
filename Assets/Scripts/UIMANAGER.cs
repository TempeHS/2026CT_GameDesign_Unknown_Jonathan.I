using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Popup Elements")]
    public GameObject panel;
    public TMP_Text popupText;

    [Header("Buttons")]
    public GameObject restartButton;
    public GameObject nextLevelButton;

    private void Start()
    {
        panel.SetActive(false);
    }

    // LEVEL COMPLETE POPUP
    public void ShowFinishOptions(float finalTime, bool allowNext)
    {
        panel.SetActive(true);

        popupText.text = "Level Complete!\nTime: " + finalTime.ToString("F2");

        restartButton.SetActive(true);
        nextLevelButton.SetActive(allowNext);
    }

    // DEATH POPUP
    public void ShowDeathPopup()
    {
        panel.SetActive(true);

        popupText.text = "You Died!";

        restartButton.SetActive(true);
        nextLevelButton.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }
}

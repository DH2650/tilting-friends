using UnityEngine;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    // Drag references in Inspector
    public GameObject mainMenuParent;
    public GameObject lobbyNewParentSMode;
    public GameObject lobbyNewParentVsMode;
    public GameObject youWonPanel;

    void Start()
    {
        // Check if player won in the last session
        if (PlayerPrefs.GetInt("PlayerWon", 0) == 1)
        {
            ShowYouWonUI();
            PlayerPrefs.SetInt("PlayerWon", 0); // Reset for next time
        }
        else
        {
            youWonPanel.SetActive(false);
        }

        int result = PlayerPrefs.GetInt("PlayerWon", 0);

    }

    void ShowYouWonUI()
    {
        mainMenuParent.SetActive(false);  // Optional: hide main menu if needed
        lobbyNewParentVsMode.SetActive(false);  // Optional: hide lobby if needed
        lobbyNewParentSMode.SetActive(false);
        youWonPanel.SetActive(true);      // Activate "You Won" UI
    }
    public void OnSurvivalModeClicked()
    {
        mainMenuParent.SetActive(false);
        lobbyNewParentSMode.SetActive(true);
    }

    public void OnVSModeClicked()
    {
        mainMenuParent.SetActive(false);
        lobbyNewParentVsMode.SetActive(true);
    }

    // Add this for the START button in lobby
    public void OnStartGameClicked()
    {
        // Add your game start logic here
        Debug.Log("Game Started!");
    }
}
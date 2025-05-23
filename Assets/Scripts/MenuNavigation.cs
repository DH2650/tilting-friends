using UnityEngine;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    // Drag references in Inspector
    public GameObject mainMenuParent;
    public GameObject lobbyNewParent;

    public void OnSurvivalModeClicked()
    {
        mainMenuParent.SetActive(false);
        lobbyNewParent.SetActive(true);
    }

    public void OnVSModeClicked()
    {
        mainMenuParent.SetActive(false);
        lobbyNewParent.SetActive(true);
    }

    // Add this for the START button in lobby
    public void OnStartGameClicked()
    {
        // Add your game start logic here
        Debug.Log("Game Started!");
    }
}
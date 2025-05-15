using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayGame()
    {
        SceneManager.LoadScene("SurvivalMode");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting...");
    }

    public void VSQuit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayVS()
    {
        SceneManager.LoadScene("VSMode");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject networkManagerObject;
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
        networkManagerObject = GameObject.FindGameObjectWithTag("NetworkManager");
        NetworkManager networkManager = networkManagerObject.GetComponent<NetworkManager>();
        networkManager.players.Clear();

        SceneManager.LoadScene("MainMenu");
    }

    public void PlayVS()
    {
        SceneManager.LoadScene("VSMode");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject networkManagerObject;
    [SerializeField] VSBoard board1;
    [SerializeField] VSBoard board2;
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

    public void StartVS()
    {
        Debug.Log("Start");
        board1.clickStart();
        board2.clickStart();

    }
}

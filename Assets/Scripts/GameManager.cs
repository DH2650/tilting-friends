using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Boards & Balls")]
    public GameObject[] boards;         // Array of board GameObjects
    public GameObject[] balls;          // Array of corresponding balls
    public Transform boardParent;       // Parent of boards, if needed for grouping

    [Header("Camera Settings")]
    public Camera mainCamera;
    public Vector3 cameraOffset = new Vector3(0, 10, -10); // Position offset for camera

    [Header("Transition Settings")]
    public float transitionTime = 1f;

    [Header("Environment Settings")]
    public Material cloudySkybox;
    public Material nightSkybox;
    public Material snowySkybox;
    public Material sunnySkybox;

    private bool VSMode;


    private int currentLevel = 0;
    private int nextLevel = 0;
    private bool transitioning = false;
    private GameObject currentBall;
    private Vector3 ballPos = new Vector3(0f, 2f, 0f);
    private void Start()
    {
        // Only activate the first board and ball at the start
        for (int i = 0; i < boards.Length; i++)
        {
            balls[i].transform.position = boards[i].transform.position + ballPos;
            boards[i].SetActive(i == 0);
            balls[i].SetActive(i == 0);
        }

        //         mainCamera.transform.position = new Vector3(35.35f, 11.68f, 0f);



    }

    private void Update()
    {
        
        currentBall = balls[currentLevel];
        if (currentBall.transform.position.y < -100)
        {
            Debug.Log("level lost");
            boards[currentLevel].SetActive(false);
            balls[currentLevel].SetActive(false);
            currentLevel = 0;
            LoadLevel(0);
            Start();
            
        }

    }

    public void LoadLevel(int level)
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "VSMode")
        {
            return;
        }

        // Switch skybox
            switch (level)
            {
                case 0:
                    RenderSettings.skybox = sunnySkybox;
                    break;
                case 1:
                    RenderSettings.skybox = cloudySkybox;
                    break;
                case 2:
                    RenderSettings.skybox = nightSkybox;
                    break;
                case 3:
                    RenderSettings.skybox = snowySkybox;
                    break;
                default:
                    Debug.LogWarning("No skybox defined for this level.");
                    break;
            }

        // Update lighting
        DynamicGI.UpdateEnvironment();
    }
    public void LevelUp()
    {
        if (transitioning)
            return;
        else if (currentLevel >= boards.Length - 1)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "VSMode")
            {
                if (boards[currentLevel].tag == "Board1")
                {
                    Debug.Log("Left team won!");
                }
                else
                {
                    Debug.Log("Right team won!");
                }
            }
            else
            {
                Debug.Log("Game won!");
            }
            
            
            
            //ADD GAME WON HERE
        }
        else
        {
            transitioning = true;
            nextLevel = currentLevel + 1;

            // Activate next board and ball before transition
            boards[currentLevel].SetActive(false);
            balls[currentLevel].SetActive(false);

            LoadLevel(nextLevel);
            boards[nextLevel].SetActive(true);
            balls[nextLevel].SetActive(true);
            balls[nextLevel].transform.position += new Vector3(3.0f, 0f, 0f); ;
            currentLevel = nextLevel;
            transitioning = false;
        }
        



//         StartCoroutine(MoveToNextBoard(nextLevel));
    }

//     private IEnumerator MoveToNextBoard(int nextLevel)
//     {
//         Vector3 boardStart = boardParent.position;
//         Vector3 boardEnd = boards[nextLevel].transform.position;
//
//         Vector3 cameraStart = mainCamera.transform.position;
//         Vector3 cameraEnd = new Vector3(
//             cameraStart.x,
//             cameraStart.y,
//             cameraStart.z
//             );
//
//         float elapsed = 0f;
//
//         while (elapsed < transitionTime)
//         {
//             float t = elapsed / transitionTime;
//             mainCamera.transform.position = Vector3.Lerp(cameraStart, cameraEnd, t);
//
//             elapsed += Time.deltaTime;
//             yield return null;
//         }
//
//         // Snap to exact final position
//         mainCamera.transform.position = cameraEnd;
//
//         // Deactivate previous board and ball
//         boards[currentLevel].SetActive(false);
//         balls[currentLevel].SetActive(false);
//
//         currentLevel = nextLevel;
//         transitioning = false;
//     }

    private void UpdateCameraPosition()
    {
        Vector3 targetPosition = boards[currentLevel].transform.position + cameraOffset;
        mainCamera.transform.position = targetPosition;
    }
}

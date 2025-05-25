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
    private bool shouldResetLevel = false;
    private bool levelLostTriggered = false;

    [Header("Level  Settings")]
    [SerializeField] public GameObject levelUpEffectPrefab; // Assign in inspector
    public Transform effectSpawnPoint;     // Where the effect spawns
    public GameObject levelUpText;         // Assign the "Level Up" UI text
    public float levelUpDelay = 2f;        // Seconds to wait before showing next level
    public GameObject levelLost;

    [Header("VSMode Settings")]
    public GameObject youWonPanel;
    
    private void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "VSMode")
        {
            youWonPanel.SetActive(false);
        }
        Debug.Log("LevelUpEffectPrefab: " + (levelUpEffectPrefab == null ? "NULL" : levelUpEffectPrefab.name));
        Debug.Log("GameObject running this: " + gameObject.name);
        Debug.Log("GameManager Instance ID: " + GetInstanceID());
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


        if (!levelLostTriggered && currentBall.transform.position.y < -100)
        {
            Debug.Log("level lost");
            boards[currentLevel].SetActive(false);
            balls[currentLevel].SetActive(false);
            levelLost.SetActive(true);

            levelLostTriggered = true;
            StartCoroutine(DelayBeforeReset(2f)); // 2-second delay
        }

        if (shouldResetLevel)
        {
            levelLost.SetActive(false);
            currentLevel = 0;
            LoadLevel(0);
            Start();

            // Reset flags
            shouldResetLevel = false;
            levelLostTriggered = false;
        }
        //if (currentBall.transform.position.y < -100)
        //{
        //    Debug.Log("level lost");
        //    boards[currentLevel].SetActive(false);
        //    balls[currentLevel].SetActive(false);
        //    levelLost.SetActive(true);
        //    currentLevel = 0;
        //    LoadLevel(0);
        //    Start();
            
        //}

    }

    private IEnumerator DelayBeforeReset(float delay)
    {
        yield return new WaitForSeconds(delay);
        shouldResetLevel = true;
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



    private IEnumerator ShowLevelUpEffectAndText(System.Action callback)
    {
        Debug.Log("Effect prefab: " + levelUpEffectPrefab);
        Scene scene = SceneManager.GetActiveScene();
        if (levelUpEffectPrefab != null)
        { 
            if (scene.name == "VSMode")
            {
                Instantiate(levelUpEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
                Debug.Log("Level up effect instantiated!");
            }

            else
            {
                //Instantiate(levelUpEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
                // Spawn at origin (or any specific world position)
                Instantiate(levelUpEffectPrefab, Vector3.zero, Quaternion.identity);
                Debug.Log("Level up effect instantiated!");

            }
        }
        // Show text
        if (levelUpText != null)
        {
            Debug.Log("Enabling Level Up Text!");
            levelUpText.SetActive(true);
        }
        // Wait
        yield return new WaitForSeconds(levelUpDelay);

        // Hide text
        if (levelUpText != null)
            levelUpText.SetActive(false);

        // Continue with the rest of the level transition
        callback?.Invoke();
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
                    youWonPanel.SetActive(true);

                }
                else
                {
                    Debug.Log("Right team won!");
                    youWonPanel.SetActive(true);
                }
            }
            else
            {
                Debug.Log("Game won!");
                PlayerPrefs.SetInt("PlayerWon", 1); // 1 = true
                // Load MainMenu
                SceneManager.LoadScene("MainMenu");
            }

            

            //ADD GAME WON HERE
        }
        else
        {
            Debug.Log("Effect prefab before transitioning: " + levelUpEffectPrefab);
            transitioning = true;
            nextLevel = currentLevel + 1;

            // Activate next board and ball before transition
            boards[currentLevel].SetActive(false);
            balls[currentLevel].SetActive(false);

            // Delay and effect before switching to next level
            StartCoroutine(ShowLevelUpEffectAndText(() => {
                LoadLevel(nextLevel);
                boards[nextLevel].SetActive(true);
                balls[nextLevel].SetActive(true);
                balls[nextLevel].transform.position += new Vector3(3.0f, 0f, 0f);
                currentLevel = nextLevel;
                transitioning = false;
            }));



            //LoadLevel(nextLevel);
            //boards[nextLevel].SetActive(true);
            //balls[nextLevel].SetActive(true);
            //balls[nextLevel].transform.position += new Vector3(3.0f, 0f, 0f); ;
            //currentLevel = nextLevel;
            //transitioning = false;
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

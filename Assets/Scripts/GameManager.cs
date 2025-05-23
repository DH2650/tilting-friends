using System.Collections;
using UnityEngine;

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


    private int currentLevel = 0;
    private int nextLevel = 0;
    private bool transitioning = false;

    private void Start()
    {
        // Only activate the first board and ball at the start
        for (int i = 0; i < boards.Length; i++)
        {
            boards[i].SetActive(i == 0);
            balls[i].SetActive(i == 0);
        }

//         mainCamera.transform.position = new Vector3(35.35f, 11.68f, 0f);
        


    }

    public void LoadLevel(int level)
    {


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
        Debug.Log("entered levelup");
        if (transitioning || currentLevel >= boards.Length - 1)
            return;

        transitioning = true;
        Debug.Log("current level: " + currentLevel);
        nextLevel = currentLevel + 1;

        // Activate next board and ball before transition
        boards[currentLevel].SetActive(false);
        balls[currentLevel].SetActive(false);

        LoadLevel(nextLevel);
        boards[nextLevel].SetActive(true);
        balls[nextLevel].SetActive(true);

        currentLevel = nextLevel;
        Debug.Log("after");
        Debug.Log("next level: " + nextLevel);
        Debug.Log("current level: " + currentLevel);
        transitioning = false;



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

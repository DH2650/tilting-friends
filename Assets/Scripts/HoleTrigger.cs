using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered collision");
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Tag was correct");
            gameManager.LevelUp();
        }
    }
}
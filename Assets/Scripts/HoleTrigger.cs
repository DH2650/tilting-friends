using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger entered");
        if (other.CompareTag("Ball"))
        {
            Debug.Log("correct tag");
            gameManager.LevelUp();
        }
    }
}
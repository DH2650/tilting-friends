using UnityEngine;

public class SurvivalGame : MonoBehaviour
{
    public GameObject board1;
    [SerializeField] HoleSpawner holeSpawner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        holeSpawner.SpawnHole(board1);
    }


}

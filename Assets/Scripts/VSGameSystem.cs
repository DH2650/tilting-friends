using UnityEngine;

public class VSGameSystem : MonoBehaviour
{
    public GameObject board1;
    public GameObject board2;
    [SerializeField] HoleSpawner holeSpawner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        holeSpawner.SpawnHole(board1);
        holeSpawner.SpawnHole(board2);

    }

}

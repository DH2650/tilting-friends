using System;
using System.Collections.Generic;
using UnityEngine;

internal class MainThread : MonoBehaviour
{
    internal static MainThread wkr;
    Queue<Action> jobs = new Queue<Action>();

    void Awake()
    {
        if (wkr == null)
        {
            wkr = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // avoid duplicates if MainThread exists already
        }

        Debug.Log("MainThread initialized in scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

    }


    void Update() {
        while (jobs.Count > 0)
            jobs.Dequeue().Invoke();
    }

    internal void AddJob(Action newJob) {
        jobs.Enqueue(newJob);
    }
}


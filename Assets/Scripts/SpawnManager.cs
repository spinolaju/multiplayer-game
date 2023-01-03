using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    private void Awake()
    {
        instance = this;
    }
    
    public Transform[] spawnPoints;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform sp in spawnPoints)
        {
            sp.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetSpawnPoint(int actorNo)
    {
        if (actorNo >= spawnPoints.Length) actorNo = spawnPoints.Length - 1;
        
        return spawnPoints[actorNo];
    }
}

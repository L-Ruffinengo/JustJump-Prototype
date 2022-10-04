using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorPlatform : MonoBehaviour
{
    [SerializeField]
    GameObject prefabPlatform;
    //-----Invoke Variables
    [SerializeField]
    float delaySpawnTime = 3f;
    [SerializeField]
    float intervalSpawnTime = 5f;
    //-----Spawn Variables
    [SerializeField]
    float randomRango = 5f;

    void Start()
    {
        InvokeRepeating("SpawnPlatform", delaySpawnTime, intervalSpawnTime);
    }

    
    void Update()
    {
       
    }

    void SpawnPlatform(){
        float x = Random.Range(-randomRango, randomRango);
        Vector3 randomTransform = transform.position + new Vector3(x, 0, 0);
        Instantiate(prefabPlatform, randomTransform , transform.rotation);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatforms : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed = 3.0f;
    
    void FixedUpdate()
    {
        RotatePlatforms();
    }

    void RotatePlatforms(){
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCylinder : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed = 5f;


    // Update is called once per frame
    void FixedUpdate()
    {
        RotateCylinder();
    }

    void RotateCylinder()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }
}

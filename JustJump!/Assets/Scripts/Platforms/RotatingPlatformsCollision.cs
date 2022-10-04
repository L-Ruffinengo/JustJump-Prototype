using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatformsCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            
            other.transform.SetParent(transform.parent);//se lo emparenta al transform del padre de las plataformas para que no haya problemas con la escala del Player
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            other.transform.SetParent(null);
            
        }
    }

}

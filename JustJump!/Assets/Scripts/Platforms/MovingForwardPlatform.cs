using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingForwardPlatform : MonoBehaviour
{
   [SerializeField]
   float movementSpeed = 5f; 
    
    void FixedUpdate()
    {
        Move(); //el movimiento va en FixedUpdate para no tener problemas con la actualiazación del movimiento del player
    }

    void Move(){
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            other.transform.SetParent(transform); //cuando Player se para sobre la plataforma, lo convertimos en hijo para que se mueva con ella
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            other.transform.SetParent(null); //cuando Player sale de la plataforma lo desemparentamos
        }
    }

}

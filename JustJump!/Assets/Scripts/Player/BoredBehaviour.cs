using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoredBehaviour : StateMachineBehaviour
{
    //ESTE SCRIPT CONTROLA LAS IDLE ANIMATIONS. SE ENCUENTRA EN LA FSM DEL ANIMATOR

    [SerializeField]
    private float timeUntilBored; //el tiempo de espera hasta que se active la animación.
    [SerializeField]
    private int animationCount; //contador de animaciones
    bool isBored;
    private float idleTime; //indica cuanto tiempo estuvo en la animación estándar
    private int boredAnimation;
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isBored)
        { //si no está aburrido
            idleTime += Time.deltaTime; //contramos la cantidad de tiempo de idle
            if (idleTime > timeUntilBored && stateInfo.normalizedTime % 1 < 0.02f) //si el tiempo de idle supera el tiempo definido de aburrimiento y estamos al principio de la animacion de idle
            {
                isBored = true; //esta aburrido
                boredAnimation = Random.Range(1, (animationCount + 1)); //creamos una variable donde se guardará un número random para indicar la siguiente animación aburrida
                boredAnimation = (boredAnimation * 2) - 1;

                animator.SetFloat("BoredAnimation", boredAnimation - 1);
            }
        }
        else if (stateInfo.normalizedTime % 1 > 0.98f) //si la animacion aburrida se esta por completar (llegar a 1)
        {
            ResetIdle(); //reseteamos las animaciones
        }

        animator.SetFloat("BoredAnimation", boredAnimation, 0.2f, Time.deltaTime); //enviamos el numero random de animación de la variable anterior al animator
    }
    private void ResetIdle()
    {

        if (isBored)
        {
            boredAnimation--; //si esta aburrido volvemos a la animacion idle
        }
        //reseteamos variabels
        isBored = false;
        idleTime = 0;
        boredAnimation = 0;
    }
}

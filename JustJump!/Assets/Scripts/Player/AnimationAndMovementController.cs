using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    //---Variables que guardan REFERENCIAS---
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    //---Variables de INPUT---
    Vector2 actualMovementInput; //variable que guarda el input de movimiento
    Vector3 actualMovement; //variable que guarda el movimiento en sí
    bool isMovementPressed;

    //---Variables de MOVIMIENTO Y ROTACION---
    [SerializeField]
    float rotationSpeed = 1.0f;

    [SerializeField]
    float movementSpeed = 1.0f;
    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        playerInput.CharacterControls.Move.started += onMovementInput; //cuando se comience a apretar los botones se llama al método
        playerInput.CharacterControls.Move.performed += onMovementInput; //cuando se mantengan apretados los botones se llama al metodo
        playerInput.CharacterControls.Move.canceled += onMovementInput; //cuando se dejen de apretar los botones se llama al metodo

    }



    void Update()
    {
        CharacterRotation();
        ControlAnimation();
        characterController.Move(actualMovement * movementSpeed * Time.deltaTime);
    }

    void ControlAnimation()
    {
        bool isRunning = animator.GetBool("isRunning"); //obtenemos el valor booleano del animator

        if (isMovementPressed && !isRunning)
        {
            animator.SetBool("isRunning", true);
        }
        else if (!isMovementPressed && isRunning)
        {
            animator.SetBool("isRunning", false);
        }
    }

    void CharacterRotation()
    {
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(actualMovement);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }


    void onMovementInput(InputAction.CallbackContext context)
    {
        actualMovementInput = context.ReadValue<Vector2>();
        //Guardamos los valores de Input (Vector2) en una variable de tipo Vector3
        actualMovement.x = actualMovementInput.x;
        actualMovement.z = actualMovementInput.y;
        // El booleano será true cuando haya movimiento en ambos ejes
        isMovementPressed = actualMovementInput.x != 0 || actualMovementInput.y != 0;

    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}

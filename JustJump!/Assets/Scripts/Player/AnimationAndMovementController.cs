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



    //---Variables de SALTO---
    bool isJumpPressed = false;
    float initialJumpVelocity;
    [SerializeField]
    float maxJumpHeight = 1.0f; //controla la altura del salto
    [SerializeField]
    float maxJumpHeightModifier2 = 2.0f; //altura 2do salto
    [SerializeField]
    float maxJumpHeightModifier3 = 4.0f; //altura 3er salto
    [SerializeField]
    float maxJumpTime = 0.5f; //Controla la velocidad del salto
    [SerializeField]
    float maxJumpTimeModifier2 = 1.25f; //duracion segundo salto
    [SerializeField]
    float maxJumpTimeModifier3 = 1.5f; //duracion tercer salto
    bool isJumping = false;
    int isJumpingHash;
    int jumpCountHash;
    bool isJumpAnimating;
    int jumpCount = 0;
    Dictionary<int, float> initialJumpVelocities = new Dictionary<int, float>();
    Dictionary<int, float> jumpGravities = new Dictionary<int, float>();
    Coroutine currentJumpResetRoutine = null;

    //---Variables CONSTANTES---
    float gravity = -9.8f;
    float groundedGravity = -0.05f; //Variable necesaria para controlar que el Player no clipee con el suelo
    [SerializeField]
    float fallMultiplier = 2.0f; //variable que sirve como multiplicador de la fuerza de gravedad cuando el personaje está en caida
    float velocityLimiter = -20.0f; //se utiliza para limitar la velocidad en Y cuando actúa el fallMultiplier

    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isJumpingHash = Animator.StringToHash("isJumping");
        jumpCountHash = Animator.StringToHash("jumpCount");

        playerInput.CharacterControls.Move.started += OnMovementInput; //cuando se comience a apretar los botones se llama al método
        playerInput.CharacterControls.Move.performed += OnMovementInput; //cuando se mantengan apretados los botones se llama al metodo
        playerInput.CharacterControls.Move.canceled += OnMovementInput; //cuando se dejen de apretar los botones se llama al metodo
        playerInput.CharacterControls.Jump.started += OnJump;
        playerInput.CharacterControls.Jump.canceled += OnJump;

        ControlJumpVariables();

    }

    void Update()
    {
        CharacterRotation();
        ControlAnimation();
        characterController.Move(actualMovement * Time.deltaTime);

        ControlGravity();
        Jump();
    }

    //---METODOS DE CONTROL-----------------------------------------------

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

    void ControlGravity()
    {
        bool isFalling = actualMovement.y <= 0.0f || !isJumpPressed; //Cuando el personaje cae el valor de movimiento en Y es 0

        if (characterController.isGrounded)
        {
            if (isJumpAnimating)
            {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
                currentJumpResetRoutine = StartCoroutine(JumpResetRoutine());
                if (jumpCount == 3){
                    jumpCount = 0;
                    animator.SetInteger(jumpCountHash, jumpCount);
                }
            }

            actualMovement.y = groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = actualMovement.y;
            float newYVelocity = actualMovement.y + (jumpGravities[jumpCount] * fallMultiplier * Time.deltaTime);
            float nextVelocity = Mathf.Max((previousYVelocity + newYVelocity) * 0.5f, velocityLimiter);
            actualMovement.y = nextVelocity;
        }
        else
        { //velocity Verlet Integration. Hace frame independent la influencia de la gravedad
            float previousYVelocity = actualMovement.y;
            float newYVelocity = actualMovement.y + (jumpGravities[jumpCount] * Time.deltaTime);
            float nextVelocity = (previousYVelocity + newYVelocity) * 0.5f;
            actualMovement.y = nextVelocity;
        }
    }

    void ControlJumpVariables()
    { //calculos para establecer la fisica del salto
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        float secondJumpGravity = (-2 * (maxJumpHeight + maxJumpHeightModifier2)) / Mathf.Pow((timeToApex * maxJumpTimeModifier2), 2);
        float secondJumpInitialVelocity = (2 * (maxJumpHeight + maxJumpHeightModifier2)) / (timeToApex * maxJumpTimeModifier2);
        float thirdJumpGravity = (-2 * (maxJumpHeight + maxJumpHeightModifier3)) / Mathf.Pow((timeToApex * maxJumpTimeModifier3), 2);
        float thirdJumpInitialVelocity = (2 * (maxJumpHeight + maxJumpHeightModifier3)) / (timeToApex * maxJumpTimeModifier3);

        //Diccionarios---
        initialJumpVelocities.Add(1, initialJumpVelocity);
        initialJumpVelocities.Add(2, secondJumpInitialVelocity);
        initialJumpVelocities.Add(3, thirdJumpInitialVelocity);

        jumpGravities.Add(0, gravity); //el index 0 sirve para resetear el conteo
        jumpGravities.Add(1, gravity);
        jumpGravities.Add(2, secondJumpGravity);
        jumpGravities.Add(3, thirdJumpGravity);

    }

    //---METODOS DE MOVIMIENTO Y SALTO------------------------------------------------------------------------------------
    void CharacterRotation()
    {
        Vector3 positionToLookAt = new Vector3(actualMovement.x, 0f, actualMovement.z);


        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    void Jump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            if (jumpCount < 3 && currentJumpResetRoutine != null)
            {
                StopCoroutine(currentJumpResetRoutine);
            }
            animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;
            jumpCount += 1;
            animator.SetInteger(jumpCountHash, jumpCount);
            Debug.Log(jumpCount);
            actualMovement.y = initialJumpVelocities[jumpCount] * 0.5f;
        }
        else if (isJumping && characterController.isGrounded && !isJumpPressed)
        {
            isJumping = false;
        }
    }

    IEnumerator JumpResetRoutine()
    {
        yield return new WaitForSeconds(0.5f); //cuando se llame a la corutin, la función se frena por 0.5 segundos
        jumpCount = 0; //cuando se espera ese tiempo, reseteamos el contado de saltos
    }
    void OnMovementInput(InputAction.CallbackContext context)
    {
        actualMovementInput = context.ReadValue<Vector2>();
        //Guardamos los valores de Input (Vector2) en una variable de tipo Vector3
        actualMovement.x = actualMovementInput.x * movementSpeed;
        actualMovement.z = actualMovementInput.y * movementSpeed;
        // El booleano será true cuando haya movimiento en ambos ejes
        isMovementPressed = actualMovementInput.x != 0 || actualMovementInput.y != 0;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }
    //--------------------------------------------------------------------------------------------------
    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}

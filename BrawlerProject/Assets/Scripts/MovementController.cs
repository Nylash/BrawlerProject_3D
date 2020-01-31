using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    public static MovementController instance;
    public Vector2 movementDirection;
    public float movementSpeed;
    public float rotationSpeed = 1;

    CharacterController controller;
    Animator animController;
    InputMap inputMap;
    Transform rotationTarget;
    Camera cam;

    Vector3 forward;
    Vector3 right;
    private void OnEnable() => inputMap.InGame.Enable();
    private void OnDisable() => inputMap.InGame.Disable();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        inputMap = new InputMap();

        inputMap.InGame.Movement.performed += ctx => movementDirection = ctx.ReadValue<Vector2>();
        inputMap.InGame.Movement.canceled += ctx => movementDirection = Vector2.zero;

        animController = GetComponent<Animator>();
        rotationTarget = new GameObject("RotationTargetObject").transform;
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    private void Update()
    {
        AnimationHandler();
        PlayerMovement();
        PlayerRotation();
    }

    void PlayerRotation()
    {
        if(movementDirection != Vector2.zero)
        {
            rotationTarget.position = transform.position + (movementDirection.x * right + forward * movementDirection.y);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotationTarget.position - transform.position), Time.deltaTime * rotationSpeed);
        }
    }

    void PlayerMovement()
    {
        forward = cam.transform.forward;
        right = cam.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        Vector3 movement = (movementDirection.x * right + forward * movementDirection.y) * (movementSpeed * movementDirection.magnitude) * Time.deltaTime;
        controller.Move(movement);
    }

    void AnimationHandler()
    {
        animController.SetFloat("SpeedMagnitude", movementDirection.magnitude);
    }
}
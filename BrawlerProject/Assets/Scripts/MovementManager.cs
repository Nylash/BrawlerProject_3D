using UnityEngine;
using UnityEngine.InputSystem;

public class MovementManager : MonoBehaviour
{
    public static MovementManager instance;
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
        if (CameraManager.instance.lockOn)
        {
            AnimationHandlerLock();
            PlayerRotationLock();
            PlayerMovementLock();
        }
        else
        {
            AnimationHandlerFree();
            PlayerRotationFree();
            PlayerMovementFree();
        }
    }

    public void SetLock(bool lockState)
    {
        animController.SetBool("LockOn", lockState);
    }

    void PlayerRotationLock()
    {
        if (CameraManager.instance.lockOnTarget)
        {
            //transform.LookAt(new Vector3(CameraManager.instance.lockOnTarget.transform.position.x,0, CameraManager.instance.lockOnTarget.transform.position.z));
            rotationTarget.position = CameraManager.instance.lockOnTarget.transform.position;
            Vector3 rot = rotationTarget.position - transform.position;
            rot.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rot), Time.deltaTime * rotationSpeed);
        }
    }

    void PlayerMovementLock()
    {
        Vector3 movement = (movementDirection.x * transform.right + transform.forward * movementDirection.y) * (movementSpeed * movementDirection.magnitude) * Time.deltaTime;
        movement.y = 0;
        controller.Move(movement);
    }

    void AnimationHandlerLock()
    {
        animController.SetFloat("SpeedX", movementDirection.x);
        animController.SetFloat("SpeedY", movementDirection.y);
    }

    void PlayerRotationFree()
    {
        forward = cam.transform.forward;
        right = cam.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        if (movementDirection != Vector2.zero)
        {
            rotationTarget.position = transform.position + (movementDirection.x * right + forward * movementDirection.y);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotationTarget.position - transform.position), Time.deltaTime * rotationSpeed);
        }
    }

    void PlayerMovementFree()
    {
        Vector3 movement = (movementDirection.x * right + forward * movementDirection.y) * (movementSpeed * movementDirection.magnitude) * Time.deltaTime;
        controller.Move(movement);
    }

    void AnimationHandlerFree()
    {
        animController.SetFloat("SpeedMagnitude", movementDirection.magnitude);
    }
}
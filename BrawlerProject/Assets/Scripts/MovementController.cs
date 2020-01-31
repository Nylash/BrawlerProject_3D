using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    public static MovementController instance;
    public Vector2 movementDirection;
    public float movementSpeed;

    Rigidbody rigid;
    InputMap inputMap;
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

        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rigid.AddForce(new Vector3(movementDirection.x,0,movementDirection.y) * movementSpeed * Time.deltaTime);
    }
}
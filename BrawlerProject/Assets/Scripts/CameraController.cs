using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public Transform target;
    public Vector3 offsetCamera;
#pragma warning disable 0649
    [SerializeField] float smooth;
#pragma warning restore 0649

    Transform player;
    Vector3 velocity = Vector3.zero;
    Vector2 cameraDirection;

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

        inputMap.InGame.CameraMovement.performed += ctx => cameraDirection = ctx.ReadValue<Vector2>();
        inputMap.InGame.CameraMovement.canceled += ctx => cameraDirection = Vector2.zero;
    }

    private void Start()
    {
        //temp to remove unity's debug updater which crashes with new input
        GameObject go = GameObject.Find("[Debug Updater]");
        if (go != null) DestroyImmediate(go);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        offsetCamera = transform.position - player.position;
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offsetCamera;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smooth * Time.deltaTime);
        transform.LookAt((target != null ? target : player));
    }
}

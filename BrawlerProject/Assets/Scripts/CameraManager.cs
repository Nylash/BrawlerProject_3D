using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    [Header("Serialize Objects")]
#pragma warning disable 0649
    [SerializeField] CinemachineTargetGroup lockOnGroup;
    [SerializeField] GameObject tmpObject;
    [SerializeField] CinemachineFreeLook freeLookCam;
    [SerializeField] CinemachineVirtualCamera lockOnCam;
    [Header("Parameters")]
    [SerializeField] float lockDetectionRadius;
    [SerializeField] LayerMask lockDetectionMask;
#pragma warning restore 0649
    [Header("Script Informations")]
    public bool lockOn;
    public GameObject lockOnTarget;

    Vector2 LookDelta;
    InputMap inputMap;
    GameObject player;
    

    private void OnEnable() => inputMap.InGame.Enable();
    private void OnDisable() => inputMap.InGame.Disable();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        inputMap = new InputMap();

        inputMap.InGame.Lock.performed += ctx => TryLock();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (!lockOn)
        {
            CinemachineCore.GetInputAxis = GetAxisCustom;
        }
    }

    void TryLock()
    {
        Collider[] inRangeTargets = Physics.OverlapSphere(player.transform.position, lockDetectionRadius, lockDetectionMask);
    }

    void Lock(GameObject target)
    {
        if (lockOn)
        {
            lockOn = false;
            lockOnCam.enabled = false;
            freeLookCam.m_YAxis.Value = .5f;
            freeLookCam.enabled = true;
            lockOnGroup.RemoveMember(lockOnTarget.transform);
            lockOnTarget = null;
        }
        else
        {
            lockOn = true;
            lockOnTarget = target;
            lockOnGroup.AddMember(target.transform, 2, 2);
            freeLookCam.enabled = false;
            lockOnCam.enabled = true;
        }
        MovementManager.instance.SetLock(lockOn);
    }

    void Recenter()
    {
        //Only work if recentring is enabled
        freeLookCam.m_RecenterToTargetHeading.RecenterNow();
        freeLookCam.m_YAxisRecentering.RecenterNow();
    }

    public float GetAxisCustom(string axisName)
    {
        LookDelta = inputMap.InGame.CameraMovement.ReadValue<Vector2>();
        LookDelta.Normalize();

        if (axisName == "Mouse X")
        {
            return LookDelta.x;
        }
        else if (axisName == "Mouse Y")
        {
            return LookDelta.y;
        }
        return 0;
    }
}

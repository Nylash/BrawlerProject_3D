using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    private InputMap inputMap; //default controls is just the Csharp code you generate from the action maps asset
    private Vector2 LookDelta;
    public bool Lockon; //this would be used to switch to a virtual camera for a lockon system

    private void Awake() => inputMap = new InputMap();

    private void OnEnable() => inputMap.InGame.Enable();
    private void OnDisable() => inputMap.InGame.Disable();

    private void Update()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;
    }

    public float GetAxisCustom(string axisName)
    {
        LookDelta = inputMap.InGame.CameraMovement.ReadValue<Vector2>(); // reads theavailable camera values and uses them.
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

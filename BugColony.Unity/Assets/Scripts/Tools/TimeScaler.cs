using UnityEngine;
using UnityEngine.InputSystem;

public class TimeScaler : MonoBehaviour
{
    [SerializeField] private float fastTimeScale = 10f;

    private InputAction _speedUpAction;

    private void Awake()
    {
        // Declare the action inline — Space key pressed/held accelerates time
        _speedUpAction = new InputAction(
            name: "SpeedUp",
            type: InputActionType.Button,
            binding: "<Keyboard>/space");
    }

    private void OnEnable()
    {
        _speedUpAction.started  += OnSpeedUpStarted;
        _speedUpAction.canceled += OnSpeedUpCanceled;
        _speedUpAction.Enable();
    }

    private void OnDisable()
    {
        _speedUpAction.started  -= OnSpeedUpStarted;
        _speedUpAction.canceled -= OnSpeedUpCanceled;
        _speedUpAction.Disable();
    }

    private void OnDestroy()
    {
        _speedUpAction.Dispose();
    }

    private void OnSpeedUpStarted(InputAction.CallbackContext _)
    {
        Time.timeScale = fastTimeScale;
    }

    private void OnSpeedUpCanceled(InputAction.CallbackContext _)
    {
        Time.timeScale = 1f;
    }
}
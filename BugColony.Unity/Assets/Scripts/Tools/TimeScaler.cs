using UnityEngine;
            using UnityEngine.InputSystem;
            
            public class TimeScaler : MonoBehaviour
            {
                [SerializeField] private float fastTimeScale = 10f;
            
                private InputAction _speedUpAction;
            
                private void Awake()
                {
                    // One action with multiple bindings:
                    // - Keyboard Space (desktop)
                    // - Primary touch press (mobile)
                    _speedUpAction = new InputAction(
                        name: "SpeedUp",
                        type: InputActionType.Button);
            
                    _speedUpAction.AddBinding("<Keyboard>/space");
                    _speedUpAction.AddBinding("<Touchscreen>/primaryTouch/press");
                }
            
                private void OnEnable()
                {
                    _speedUpAction.started += OnSpeedUpStarted;
                    _speedUpAction.canceled += OnSpeedUpCanceled;
                    _speedUpAction.Enable();
                }
            
                private void OnDisable()
                {
                    _speedUpAction.started -= OnSpeedUpStarted;
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
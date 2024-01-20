using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Run { get; private set; }
    public bool Jump { get; private set; }
    public bool Crouch { get; private set; }
    public bool PowerSlow { get; private set; }
    public bool PowerStop { get; private set; }
    public bool Menu { get; private set; }
    
    public PlayerInput playerInput;
    
    private InputActionMap _playerMap;
    private InputActionMap _menuMap;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _runAction;
    private InputAction _jumpAction;
    private InputAction _crouchAction;
    private InputAction _powerActionSlow;
    private InputAction _powerActionStop;
    private InputAction _menu;

// ------------------------------------------------------- BASE FTC ----------------------------------------------------
    private void Awake()
    {
        HideCursor();

        _playerMap = playerInput.actions.FindActionMap("Player");
        _menuMap = playerInput.actions.FindActionMap("Menu");
        
        _moveAction = _playerMap.FindAction("Move");
        _lookAction = _playerMap.FindAction("Look");
        _runAction = _playerMap.FindAction("Run");
        _jumpAction = _playerMap.FindAction("Jump");
        _crouchAction = _playerMap.FindAction("Crouch");
        _powerActionSlow = _playerMap.FindAction("Power_Slow");
        _powerActionStop = _playerMap.FindAction("Power_Stop");
        _menu = _menuMap.FindAction("Menu");


        _moveAction.performed += onMove;
        _lookAction.performed += onLook;
        _runAction.performed += onRun;
        _jumpAction.performed += onJump;
        _crouchAction.performed += onCrouch;
        _powerActionSlow.performed += onPowerSlow;
        _powerActionStop.performed += onPowerStop;
        _menu.performed += onMenu;

        _moveAction.canceled += onMove;
        _lookAction.canceled += onLook;
        _runAction.canceled += onRun;
        _jumpAction.canceled += onJump;
        _crouchAction.canceled += onCrouch;
        _powerActionSlow.canceled += onPowerSlow;
        _powerActionStop.canceled += onPowerStop;
        _menu.canceled += onMenu;
    }
    
    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void OnEnable()
    {
        _playerMap.Enable();
        _menuMap.Enable();
    }

    private void OnDisable()
    {
        _playerMap.Disable();
        _menuMap.Disable();
    }
    
// -------------------------------------------------------- PLAYER -----------------------------------------------------
    private void onMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    private void onLook(InputAction.CallbackContext context)
    {
        Look = context.ReadValue<Vector2>();
    }

    private void onRun(InputAction.CallbackContext context)
    {
        Run = context.ReadValueAsButton();
    }

    private void onJump(InputAction.CallbackContext context)
    {
        Jump = context.ReadValueAsButton();
    }

    private void onCrouch(InputAction.CallbackContext context)
    {
        Crouch = context.ReadValueAsButton();
    }

    private void onPowerSlow(InputAction.CallbackContext context)
    {
        PowerSlow = context.ReadValueAsButton();
    }

    private void onPowerStop(InputAction.CallbackContext context)
    {
        PowerStop = context.ReadValueAsButton();
    }
    
// --------------------------------------------------------- MENUS -----------------------------------------------------
    private void onMenu(InputAction.CallbackContext context)
    {
        Menu = context.ReadValueAsButton();
    }
}

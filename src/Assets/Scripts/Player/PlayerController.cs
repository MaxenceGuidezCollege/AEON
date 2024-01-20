using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
// ----------------------------------------------------- VARIABLES -----------------------------------------------------
    [Header("Camera")]
    public Camera playerCamera;
    public Transform cameraRoot;
    public float cameraTopLimit = -40f;
    public float cameraBottomLimit = 70f;
    [Space]
    public float mouseSensitivity;
    public float baseMouseSensitivity = 30f;

    [Header("Speeds")]
    public float baseWalkSpeed = 2f;
    public float baseRunSpeed = 6f;
    public float baseCrouchSpeed = 1.5f;
    public float walkSpeed;
    public float runSpeed;
    public float crouchSpeed;

    [Header("Jump")]
    [Range(10, 500)]
    public float jumpFactor = 300f;
    public float airResistance = 0.8f;
    public float dis2Ground = 1f;
    public LayerMask groundCheck;
    
    [Header("Sounds")]
    public AudioClip[] footstepSound;
    public float timeBetweenFootsteps = 0.5f;
    public AudioClip jumpSound;
    public AudioClip landingSound;

    private Rigidbody _playerRigidbody;
    private InputManager _inputManager;
    private Animator _animator;
    private bool _hasAnimator, _isGrounded, _isBlocked, _inSlowMo, _isJumpSoundPlaying;
    private int _xVelHash, _yVelHash, _zVelHash, _jumpHash, _groundHash, _fallingHash, _crouchHash, _speedFactor = 10;
    private float _xRotation, _animBlendSpeed = 10f, _lastFootstepTime;
    private Vector2 _currentVelocity;
    
// ------------------------------------------------------ BASE FCT -----------------------------------------------------
    public static PlayerController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
    
        instance = this;
    }

    void Start()
    {
        _hasAnimator = TryGetComponent<Animator>(out _animator);
        _playerRigidbody = GetComponent<Rigidbody>();
        _inputManager = GetComponent<InputManager>();

        _xVelHash = Animator.StringToHash("X_Velocity");
        _yVelHash = Animator.StringToHash("Y_Velocity");
        _zVelHash = Animator.StringToHash("Z_Velocity");
        _jumpHash = Animator.StringToHash("Jump");
        _groundHash = Animator.StringToHash("Grounded");
        _fallingHash = Animator.StringToHash("Falling");
        _crouchHash = Animator.StringToHash("Crouch");
        
        UnblockPlayer();
    }

    private void FixedUpdate()
    {
        if (Time.timeScale < 1)
        {
            SpeedUpPLayer();
        }
        else
        {
            SlowDownPLayer();
        }
        SampleGround();
        Move();
        HandleJump();
        HandleCrouch();
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private void LateUpdate()
    {
        CameraMovements();
    }
    
// ----------------------------------------------------- MOVEMENTS -----------------------------------------------------
    private void Move()
    {
        if (!_hasAnimator) return;
        if (_isBlocked) return;

        float targetSpeed = _inputManager.Run ? runSpeed : walkSpeed;
        if (_inputManager.Crouch) targetSpeed = crouchSpeed;
        if (_inputManager.Move == Vector2.zero) targetSpeed = 0;

        if (!_isGrounded)
        {
            targetSpeed *= 0.4f;

            _playerRigidbody.AddForce(transform.TransformVector(new Vector3(
            _currentVelocity.x * airResistance,
            0,
            _currentVelocity.y * airResistance
            )), ForceMode.VelocityChange);
        }

        _currentVelocity.x = Mathf.Lerp(
            _currentVelocity.x,
            _inputManager.Move.x * targetSpeed,
            _animBlendSpeed * Time.unscaledDeltaTime
        );

        _currentVelocity.y = Mathf.Lerp(
            _currentVelocity.y,
            _inputManager.Move.y * targetSpeed,
            _animBlendSpeed * Time.unscaledDeltaTime
        );
        
        if (_currentVelocity.magnitude > 0.1f && _isGrounded)
        {
            float adjustedTimeBetweenFootsteps = Mathf.Lerp(timeBetweenFootsteps, timeBetweenFootsteps / 2f, _inputManager.Run ? 1f : 0f);

            if (Time.time - _lastFootstepTime > adjustedTimeBetweenFootsteps)
            {
                PlayFootstepSound();
                _lastFootstepTime = Time.time;
            }
        }

        var xVelDifference = _currentVelocity.x - _playerRigidbody.velocity.x;
        var zVelDifference = _currentVelocity.y - _playerRigidbody.velocity.z;

        _playerRigidbody.AddForce(
            transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)),
            ForceMode.VelocityChange
        );

        if (_inSlowMo)
        {
            _animator.SetFloat(_xVelHash, _currentVelocity.x / _speedFactor);
            _animator.SetFloat(_yVelHash, _currentVelocity.y / _speedFactor);
        }
        else
        {
            _animator.SetFloat(_xVelHash, _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
        }

    }

    private void CameraMovements()
    {
        if (!_hasAnimator) return;
        if (_isBlocked) return;

        var mouseX = _inputManager.Look.x;
        var mouseY = _inputManager.Look.y;

        playerCamera.transform.position = cameraRoot.position;

        _xRotation -= mouseY * mouseSensitivity * Time.smoothDeltaTime;
        _xRotation = Mathf.Clamp(_xRotation, cameraTopLimit, cameraBottomLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        _playerRigidbody.MoveRotation(_playerRigidbody.rotation * Quaternion.Euler(
            0,
            mouseX * mouseSensitivity * Time.smoothDeltaTime,
            0));
    }

    private void HandleCrouch()
    {
        if (_isBlocked) return;
        
        _animator.SetBool(_crouchHash, _inputManager.Crouch);
    }

    private void HandleJump()
    {
        if (!_hasAnimator) return;
        if (!_inputManager.Jump) return;
        if (!_isGrounded) return;
        if (_isBlocked) return;

        _animator.SetTrigger(_jumpHash);
        PlayJumpSound();
        
        StartCoroutine(ResetJumpTrig());
    }

    // Do not rename (Event fct)
    public void JumpAddForce()
    {
        _playerRigidbody.AddForce(Vector3.up * _playerRigidbody.velocity.y, ForceMode.VelocityChange);
        _playerRigidbody.AddForce(Vector3.up * jumpFactor, ForceMode.Impulse);
    }

    private void SampleGround()
    {
        if (!_hasAnimator) return;

        RaycastHit hitInfo;
        bool wasFalling = !_isGrounded;
        
        if (Physics.Raycast(_playerRigidbody.worldCenterOfMass, Vector3.down, out hitInfo, dis2Ground + 0.1f, groundCheck))
        {
            _isGrounded = true;
            
            SetAnimationGrounding();
            if (wasFalling) PlayLandingSound();
            
            return;
        }

        _isGrounded = false;
        _animator.SetFloat(_zVelHash, _playerRigidbody.velocity.y);
        SetAnimationGrounding();
    }

    private void SetAnimationGrounding()
    {
        _animator.SetBool(_fallingHash, !_isGrounded);
        _animator.SetBool(_groundHash, _isGrounded);
    }
    
// -------------------------------------------------------- TIME -------------------------------------------------------
    private void SpeedUpPLayer()
    {
        if (!_inSlowMo)
        {
            _currentVelocity.x = _currentVelocity.x * _speedFactor;
            _currentVelocity.y = _currentVelocity.y * _speedFactor;
        }
        _inSlowMo = true;
        walkSpeed = baseWalkSpeed * _speedFactor;
        runSpeed = baseRunSpeed * _speedFactor;
        crouchSpeed = baseCrouchSpeed * _speedFactor;
        mouseSensitivity = baseMouseSensitivity * _speedFactor;
    }

    private void SlowDownPLayer()
    {
        if (_inSlowMo)
        {
            _currentVelocity.x = _currentVelocity.x / _speedFactor;
            _currentVelocity.y = _currentVelocity.y / _speedFactor;
        }
        _inSlowMo = false;
        walkSpeed = baseWalkSpeed;
        runSpeed = baseRunSpeed;
        crouchSpeed = baseCrouchSpeed;
        mouseSensitivity = baseMouseSensitivity;
    }
    
// ------------------------------------------------------- SOUNDS ------------------------------------------------------
    private void PlayFootstepSound()
    {
        if (footstepSound != null && AudioManager.instance != null)
        {
            int n = Random.Range(0, footstepSound.Length);
            AudioManager.instance.PlayClipAt(footstepSound[n], transform.position);
        }
    }

    private void PlayJumpSound()
    {
        if (jumpSound != null && AudioManager.instance != null && !_isJumpSoundPlaying)
        {
            AudioManager.instance.PlayClipAt(jumpSound, transform.position);
            _isJumpSoundPlaying = true;

            StartCoroutine(ResetJumpSoundFlag(jumpSound.length));
        }
    }

    private void PlayLandingSound()
    {
        AudioManager.instance.PlayClipAt(landingSound, transform.position);
    }

    private IEnumerator ResetJumpSoundFlag(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isJumpSoundPlaying = false;
    }
    
// ------------------------------------------------------ ANIMATION ----------------------------------------------------
    private void BlockPlayer()
    {
        _isBlocked = true;
        _animator.SetFloat(_xVelHash, 0f);
        _animator.SetFloat(_yVelHash, 0f);
    }
    public void UnblockPlayer()
    {
        _isBlocked = false;
    }
    public void CatchByKronos()
    {
        BlockPlayer();
        RotateToTarget(KronosController.instance.transform);
    }
    
    private void RotateToTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    
// -------------------------------------------------------- UTILS ------------------------------------------------------
    private IEnumerator ResetJumpTrig()
    {
        yield return new WaitForSeconds(0.5f);
        _animator.ResetTrigger(_jumpHash);
    }
}
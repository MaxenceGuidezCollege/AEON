using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class KronosController : MonoBehaviour
{
// ----------------------------------------------------- VARIABLES -----------------------------------------------------
    public float farSpeed = 6f;
    public float nearSpeed = 2f;
    public float distanceThreshold = 10f;
    public AudioClip soundOnCatch;
    
    private bool _hasAnimator, _isTouching;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private GameObject _player;
    private int _yVelHash, _touchHash;
    
// ------------------------------------------------------ BASE FCT -----------------------------------------------------
    public static KronosController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }
    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _player = PlayerController.instance.gameObject;
        
        _yVelHash = Animator.StringToHash("Y_Velocity");
        _touchHash = Animator.StringToHash("Touch");

        _navMeshAgent.acceleration = 10f;
        _navMeshAgent.isStopped = false;
    }
    
    private void FixedUpdate()
    {
        if (_isTouching) _navMeshAgent.isStopped = true;
        else _navMeshAgent.isStopped = false;
    
        MoveToTarget(_player.transform);
        HandleTouch();
    }
    private void LateUpdate()
    {
        RotateToTarget(_player.transform);
    }
    
// ------------------------------------------------------ MOVEMENTS ----------------------------------------------------
    private void MoveToTarget(Transform target)
    {
        if (_player is null) return;
        if (!_hasAnimator) return;
    
        _navMeshAgent.SetDestination(target.position);

        float distanceToTarget = _navMeshAgent.remainingDistance;
        float speed = Mathf.Lerp(nearSpeed, farSpeed, Mathf.InverseLerp(0, distanceThreshold, distanceToTarget));

        _navMeshAgent.speed = speed;

        _animator.SetFloat(_yVelHash, Mathf.InverseLerp(nearSpeed, farSpeed, speed));
    }
    private void RotateToTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
    }
    
// -------------------------------------------------------- TOUCH ------------------------------------------------------
    private void HandleTouch()
    {
        if (_navMeshAgent.hasPath &&
            _navMeshAgent.pathStatus != NavMeshPathStatus.PathInvalid &&
            _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            _isTouching = true;
            _navMeshAgent.velocity = Vector3.zero;
            
            _animator.SetTrigger(_touchHash);
            StartCoroutine(ResetTrig(_touchHash));
            
            AudioManager.instance.StopAllSounds(false);
            AudioManager.instance.PlayClipAt(soundOnCatch, transform.position);
            PlayerController.instance.CatchByKronos();
        }
    }
    
    // Do not rename (Event fct)
    public void TouchAnimationFinished()
    {
        _isTouching = false;
        PlayerController.instance.UnblockPlayer();
        SceneManager.LoadScene("MainScene");
    }
    
// ------------------------------------------------------ APPEARANCE ---------------------------------------------------
    public void Appear()
    {
        gameObject.SetActive(true);
    }
    public void Disappear()
    {
        gameObject.SetActive(false);
    }
    
    
// --------------------------------------------------------- UTILS -----------------------------------------------------
    IEnumerator ResetTrig(int trigger)
    {
        yield return new WaitForSeconds(0.5f);
        _animator.ResetTrigger(trigger);
    }
}
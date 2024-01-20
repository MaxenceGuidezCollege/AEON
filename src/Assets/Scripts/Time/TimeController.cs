using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
// ----------------------------------------------------- VARIABLES -----------------------------------------------------
    [Header("Sounds")]
    public AudioClip slowMoInSound;
    public AudioClip slowMoThenSound;
    public AudioClip slowMoOutSound;

    [Header("Icons")]
    public Image iconSlow;
    public Image iconStop;
    public Sprite iconSlowDisabled;
    public Sprite iconStopDisabled;
    public Sprite iconSlowEnabled;
    public Sprite iconStopEnabled;
    
    private Dictionary<GameObject, Vector3> _originalVelocities = new Dictionary<GameObject, Vector3>();
    private InputManager _inputManager;
    private bool _isStopped, _lastSlowState, _lastStopState, _isSlowMoThenPlaying;
    
// ------------------------------------------------------ BASE FCT -----------------------------------------------------
    public static TimeController instance;

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
        iconSlow.sprite = iconSlowDisabled;
        iconStop.sprite = iconStopDisabled;
        
        _inputManager = GetComponent<InputManager>();
        
        _lastSlowState = false;
        _lastStopState = false;
        _isSlowMoThenPlaying = false;
        _isStopped = false;
        AudioManager.instance.StopAllSounds(false);
    }

    void FixedUpdate()
    {
        HandlePowerSlow();
        HandlePowerStop();
    }
    
// ------------------------------------------------------ POWER FCT ----------------------------------------------------
    private void HandlePowerSlow()
    {
        bool isSlowed = _inputManager.PowerSlow;

        if (isSlowed) Time.timeScale = 0.1f;
        else Time.timeScale = 1.0f;
        
        if (_lastSlowState != isSlowed && isSlowed)
        {
            iconSlow.sprite = iconSlowEnabled;
            AudioManager.instance.SetSpeedMusics(0.5f);
            PlaySlowMoInSound();
            PlaySlowMoThenSound();
        }
        else if (_lastSlowState != isSlowed && !isSlowed){
            iconSlow.sprite = iconSlowDisabled;
            PlaySlowMoOutSound();
            AudioManager.instance.SetSpeedMusics(1f);
        }

        _lastSlowState = isSlowed;
    }

    private void HandlePowerStop()
    {
        bool isStopped = _inputManager.PowerStop;

        if (isStopped && !_lastStopState)
        {
            _isStopped = !_isStopped;
            
            if (_isStopped)
            {
                iconStop.sprite = iconStopEnabled;
                AudioManager.instance.StopAllSounds(true);
                StoreOriginalVelocities();
                ChangeKinematicState();
            }
            else
            {
                iconStop.sprite = iconStopDisabled;
                AudioManager.instance.StopAllSounds(false);
                RestoreOriginalVelocities();
                ChangeKinematicState();
            }
        }

        _lastStopState = isStopped;
    }
    
// ------------------------------------------------------- STOP FCT ----------------------------------------------------
    private void ChangeKinematicState()
    {
        GameObject[] stoppables = FindAllStoppableObjects();

        foreach (GameObject stoppable in stoppables)
        {
            Rigidbody rigidbody = stoppable.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = !rigidbody.isKinematic;
            }
        }
    }

    private void StoreOriginalVelocities()
    {
        _originalVelocities.Clear();

        GameObject[] stoppables = FindAllStoppableObjects();

        foreach (GameObject stoppable in stoppables)
        {
            Rigidbody rigidbody = stoppable.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                _originalVelocities.Add(stoppable, rigidbody.velocity);
            }
        }
    }

    private void RestoreOriginalVelocities()
    {
        foreach (var entry in _originalVelocities)
        {
            GameObject stoppable = entry.Key;
            Rigidbody rigidbody = stoppable.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.velocity = entry.Value;
            }
        }
    }
    
// -------------------------------------------------------- SOUNDS -----------------------------------------------------
    private void PlaySlowMoInSound()
    {
        if (slowMoInSound != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlayClipAt(slowMoInSound, transform.position);
        }
    }

    private void PlaySlowMoThenSound()
    {
        if (slowMoThenSound != null && AudioManager.instance != null && !_isSlowMoThenPlaying)
        {
            AudioManager.instance.PlayClipAt(slowMoThenSound, transform.position, true);
            _isSlowMoThenPlaying = true;
        }
    }

    private void StopSlowMoThenSound()
    {
        if (slowMoThenSound != null && AudioManager.instance != null && _isSlowMoThenPlaying)
        {
            AudioManager.instance.StopSound(slowMoThenSound);
            _isSlowMoThenPlaying = false;
        }
    }

    private void PlaySlowMoOutSound()
    {
        if (slowMoOutSound != null && AudioManager.instance != null)
        {
            StopSlowMoThenSound();
            AudioManager.instance.PlayClipAt(slowMoOutSound, transform.position);
        }
    }
    
// --------------------------------------------------------- UTILS -----------------------------------------------------
    private GameObject[] FindAllStoppableObjects()
    {
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        GameObject[] goWithoutPlayer = allGameObjects.Where(go => !go.CompareTag("Player")).ToArray();
        GameObject[] goWithoutFan = goWithoutPlayer.Where(go => go.name != "Fan").ToArray();
        return goWithoutFan.Where(go => go.name != "MovingPlateform").ToArray();
    }
    
    public bool GetStopState()
    {
        return _isStopped;
    }
    public bool GetSlowState()
    {
        return _lastSlowState;
    }
}

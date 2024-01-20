using UnityEngine;

public class StartElevator : ElevatorManager
{
    public AudioClip soundOnDoorMoving;
    
    private bool _isAlreadyClosed;
    
    void Start()
    {
        AudioManager.instance.PlayClipAt(soundOnDoorMoving, transform.position);
        StartCoroutine(MoveDoors(true, 2f, 1f));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isAlreadyClosed)
        {
            StartCoroutine(MoveDoors(false, 0f, 1f));
            _isAlreadyClosed = true;
        }
    }
}

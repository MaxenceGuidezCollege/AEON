using UnityEngine;

public class EndElevator : ElevatorManager
{
    private Coroutine _doorsCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (_doorsCoroutine != null)
        {
            StopCoroutine(_doorsCoroutine);
        }

        areDoorsMoving = true;
        _doorsCoroutine = StartCoroutine(MoveDoors(true, 0f, 1f));
    }

    private void OnTriggerExit(Collider other)
    {
        if (_doorsCoroutine != null)
        {
            StopCoroutine(_doorsCoroutine);
        }

        areDoorsMoving = true;
        _doorsCoroutine = StartCoroutine(MoveDoors(false, 0f, 1f));
    }

    public void AnimateLastClosing()
    {
        StartCoroutine(MoveDoors(false, 0f, 2f));
    }
}
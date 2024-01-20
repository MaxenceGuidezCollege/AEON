using System.Collections;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    public GameObject doorRight;
    public GameObject doorLeft;

    public Vector3 initialPositionRight;
    public Vector3 initialPositionLeft;
    protected bool areDoorsMoving;

    protected IEnumerator MoveDoors(bool haveToOpen, float afterSeconds, float duration)
    {
        yield return new WaitForSeconds(afterSeconds);

        float startTime = Time.time;
        float deltaDoor = 0.85f;
        Vector3 localPositionRight = doorRight.transform.localPosition;
        Vector3 localPositionLeft = doorLeft.transform.localPosition;
        Vector3 finalPositionRight;
        Vector3 finalPositionLeft;
        if (haveToOpen)
        {
            finalPositionRight = new Vector3(initialPositionRight.x - deltaDoor, initialPositionRight.y, initialPositionRight.z);
            finalPositionLeft = new Vector3(initialPositionLeft.x + deltaDoor, initialPositionLeft.y, initialPositionLeft.z);
        }
        else
        {
            finalPositionRight = initialPositionRight;
            finalPositionLeft = initialPositionLeft;
        }

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;

            doorRight.transform.localPosition = Vector3.Lerp(localPositionRight, finalPositionRight, t);
            doorLeft.transform.localPosition = Vector3.Lerp(localPositionLeft, finalPositionLeft, t);

            yield return null;
        }

        doorRight.transform.localPosition = finalPositionRight;
        doorLeft.transform.localPosition = finalPositionLeft;
        areDoorsMoving = false;
    }
}

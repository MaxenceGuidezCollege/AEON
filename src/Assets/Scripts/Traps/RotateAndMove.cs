using UnityEngine;
using UnityEngine.Serialization;


public class RotateAndMove : MonoBehaviour
{
    public float rotationSpeed = 5f;
    
    private BoxCollider _boxCollider;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        float ajustedRotationSpeed;
        if (TimeController.instance.GetStopState())
        {
            RemoveCollider();
            ajustedRotationSpeed = 0;
        }
        else if (TimeController.instance.GetSlowState())
        {
            RemoveCollider();
            ajustedRotationSpeed = rotationSpeed / 10;
        }
        else
        {
            RestoreCollider();
            ajustedRotationSpeed = rotationSpeed;
        }

        transform.Rotate(Vector3.back * ajustedRotationSpeed);
    }

    private void RemoveCollider()
    {
        if (_boxCollider != null)
        {
            Destroy(_boxCollider);
            _boxCollider = null;
        }
    }

    private void RestoreCollider()
    {
        if (_boxCollider == null)
        {
            _boxCollider = gameObject.AddComponent<BoxCollider>();
            _boxCollider.size = new Vector3(15f, 15f, 2f); 
        }
    }
}

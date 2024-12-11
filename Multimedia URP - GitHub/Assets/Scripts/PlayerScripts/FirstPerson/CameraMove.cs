using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;

    void LateUpdate()
    {
        transform.position = cameraPosition.position;
    }
}
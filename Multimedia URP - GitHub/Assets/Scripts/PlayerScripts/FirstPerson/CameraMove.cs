using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;

    void Awake(){
        transform.position = cameraPosition.position;
    }

    void LateUpdate()
    {
        transform.position = cameraPosition.position;
    }
}
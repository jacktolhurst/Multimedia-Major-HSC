using UnityEngine;

public class SunMovement : MonoBehaviour
{
    [SerializeField] private float movementAmount;
    
    void Update(){
        transform.Rotate(movementAmount * Time.deltaTime, 0, 0);
    }
}

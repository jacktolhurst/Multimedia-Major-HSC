using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    Rigidbody rb;

    private Vector3 moveDirection;


    [SerializeField] private float speed;
    [SerializeField] private float drag;

    void Awake(){
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        moveDirection = Vector3.zero;
    }

    void Update(){
        GetInput();
        ControlDrag();
    }

    private void GetInput(){
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
    }

    private void ControlDrag(){ 
        rb.linearDamping = drag;
    }

    void FixedUpdate(){
        Movement();
    }

    private void Movement(){
        rb.AddForce(moveDirection.normalized * speed, ForceMode.Acceleration);
    }
}

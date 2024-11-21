using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    private Rigidbody rb;

    private Renderer playerRenderer;

    [SerializeField] private Transform orientation;

    private Vector3 moveDirection;

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    private float movementMultiplier = 10;
    [SerializeField] private float jumpMultiplier = 0.4f;
    private float groundDrag = 6f;
    private float airBag = 2f;

    private bool isGrounded;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        playerRenderer = GetComponentInChildren<Renderer>();

        rb.freezeRotation = true;

        moveDirection = Vector3.zero;
    }

    void Update(){
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerRenderer.bounds.extents.y + 0.1f);

        GetInput();
        ControlDrag();

        if(Input.GetKeyDown("space") && isGrounded){
            Jump();
        }
    }

    private void GetInput(){
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
    }

    private void Jump(){
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ControlDrag(){ 
        if(isGrounded){
            rb.linearDamping = groundDrag;
        } 
        else{
            rb.linearDamping = airBag;
        }
    }

    void FixedUpdate(){
        Movement();
    }

    private void Movement(){
        if(isGrounded){
            rb.AddForce(moveDirection.normalized * speed * movementMultiplier, ForceMode.  Acceleration);
        }
        else{
            rb.AddForce(moveDirection.normalized * speed * movementMultiplier * jumpMultiplier, ForceMode.  Acceleration);

        }
    }
}

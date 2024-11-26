using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{   
    [Header("Attributes")]
    [SerializeField] private Renderer playerRenderer;

    private Rigidbody rb;

// ----------------------------------------------------------------

    [Header("Movement")]
    [SerializeField] private Transform orientation;

    private Vector3 moveDirection;

    [SerializeField] private float speed;
    [SerializeField] private float sprintSpeed;
    private float movementMultiplier = 10;
    [SerializeField] private float groundDrag = 6f;

// ----------------------------------------------------------------

    [Header("Jumping")]
    [SerializeField] private LayerMask groundMask;

    private Vector3 playerBoundsSize;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpMultiplier = 0.4f;
    [SerializeField] private float airDrag = 2f;

    [Header("Using CheckBox")]
    [SerializeField] private float checkBoxSize;
    [Range(0f, 1f)]
    [SerializeField] private float boxPadding;

    [SerializeField] private bool useCheckBox;

    [Header("Using Raycasts")]
    [SerializeField] private float raycastDistance;

    private Vector3[] raycastPositions = {new Vector3(0,0,0)};

    [SerializeField] private bool useRaycast;

    private bool isGrounded;

// ----------------------------------------------------------------


    void Awake(){
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        moveDirection = Vector3.zero;

        playerBoundsSize = playerRenderer.bounds.size;

        Vector3[] positions = {
            new Vector3(0, -(playerBoundsSize.y/2), 0),
            new Vector3(playerBoundsSize.x/2, -playerBoundsSize.y/2, -playerBoundsSize.z/2),
            new Vector3(playerBoundsSize.x/2, -playerBoundsSize.y/2, playerBoundsSize.z/2),
            new Vector3(-playerBoundsSize.x/2, -playerBoundsSize.y/2, -playerBoundsSize.z/2),
            new Vector3(-playerBoundsSize.x/2, -playerBoundsSize.y/2, playerBoundsSize.z/2)
        };

        raycastPositions = positions;
    }

    void Update(){
        if(useCheckBox){
            isGrounded = Physics.CheckBox(transform.position - new  Vector3(0, playerBoundsSize.y/2 + checkBoxSize/2, 0), new Vector3(playerBoundsSize.x, checkBoxSize, playerBoundsSize.z) - new Vector3(boxPadding, 0, boxPadding), orientation.rotation, groundMask);

        }
        if(useRaycast){
            isGrounded = false;
            foreach (var position in raycastPositions){
                Vector3 worldPosition = transform.TransformPoint(position);
                if(Physics.Raycast(worldPosition, Vector3.down, raycastDistance, groundMask)){
                    Debug.Log("hi");
                    isGrounded = true;
                    break;
                }
                
            }
        }

        GetInput();
        ControlDrag();

        if(Input.GetButtonDown("Jump") && isGrounded){
            Jump();
        }
    }

    private void GetInput(){
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
    }

    private void Jump(){
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ControlDrag(){ 
        if(isGrounded){
            rb.linearDamping = groundDrag;
        } 
        else{
            rb.linearDamping = airDrag;
        }
    }

    void FixedUpdate(){
        Movement();
    }

    private void Movement(){
        if(isGrounded){
            rb.AddForce(moveDirection.normalized * (speed * movementMultiplier + (Input.GetAxisRaw("Sprint") * sprintSpeed)), ForceMode.  Acceleration);
        }
        else{
            rb.AddForce(moveDirection.normalized * speed * movementMultiplier * jumpMultiplier, ForceMode.  Acceleration);

        }
    }


    void OnDrawGizmos(){
        if(isGrounded){
            Gizmos.color = Color.green;
        }
        else{
            Gizmos.color = Color.red;
        }

        Gizmos.matrix = Matrix4x4.TRS(transform.position, orientation.rotation, Vector3.one);

        if(useCheckBox){
            Gizmos.DrawCube(Vector3.zero - new  Vector3(0, playerBoundsSize.y/2 + checkBoxSize/2, 0), new Vector3(playerBoundsSize.x, checkBoxSize,playerBoundsSize.z)  - new Vector3(boxPadding, 0, boxPadding));
        }

        if(useRaycast){
            foreach (var position in raycastPositions){
                Gizmos.DrawLine(position, position + Vector3.down * raycastDistance);
            }
        }
    }
}
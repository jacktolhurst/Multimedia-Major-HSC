using UnityEngine;
using System.Collections.Generic;
using FMOD.Studio;

[SelectionBase]
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
    [SerializeField]private CapsuleCollider playerCollider;

    [SerializeField] private LayerMask groundMask;

    private Vector3 playerBoundsExtents;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpMultiplier = 0.4f;
    [SerializeField] private float airDrag = 2f;
    private float lastJumpTime;
    [SerializeField] private float jumpIntervalTime;
    [SerializeField] private float heightDivision;
    private float standardHeight;

// ----------------------------------------------------------------

    [Header("Raycasting")]
    [SerializeField] private Vector2 raycastPadding;

    private List<Vector3> cachedPositions = new List<Vector3>();

    [Range(0,360)]
    [SerializeField] private int innerIterations;
    [Range(0,360)]
    [SerializeField] private int iterations;

    private float raycastSize;
    private float raycastHeightSubtract;

    private bool isGrounded;

    [HideInInspector] public Vector3 generatedStartPos;
    [Header("StartPositioning")]
    [SerializeField] private Vector3 subtractedPosition;

    [SerializeField] private bool useStartPos;

    [Header("Audio")]
    [SerializeField] private string footStepsName;
    private FMODEvents.SoundEventClass playerFootstepsSound;

    void Awake(){
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        moveDirection = Vector3.zero;

        playerBoundsExtents = playerRenderer.bounds.extents;

        raycastHeightSubtract = playerBoundsExtents.y / 2;
        raycastSize = raycastHeightSubtract + 0.01f;

        cachedPositions.Add(new Vector3(0, -playerBoundsExtents.y + raycastHeightSubtract,0));
        for(int i = 0; i < innerIterations; i++){
            float angle = (360 / innerIterations) * i;

            cachedPositions.Add(GetPosAtAngle(new Vector3(0, -playerBoundsExtents.y + raycastHeightSubtract, 0), new Vector2(playerBoundsExtents.x/2, playerBoundsExtents.z/2), angle));
        }
        for(int i = 0; i < iterations; i++){
            float angle = (360 / iterations) * i;
            
            cachedPositions.Add(GetPosAtAngle(new Vector3(0, -playerBoundsExtents.y + raycastHeightSubtract, 0), new Vector2(playerBoundsExtents.x, playerBoundsExtents.z), angle));
        }

        standardHeight = playerCollider.height;
    }

    void Start(){
        if(useStartPos){
            rb.isKinematic = true;
            if(generatedStartPos != Vector3.zero){
                generatedStartPos.y = 1;
                generatedStartPos -= subtractedPosition;
                transform.position = generatedStartPos;
            }
        }

        playerFootstepsSound = AudioManager.instance.GetSoundEventClass(footStepsName);
    }

    void Update(){
        GetInput();

        isGrounded = CheckGround();
        if(Input.GetButtonDown("Jump")){
            if(isGrounded && Time.time - lastJumpTime > jumpIntervalTime){
                Jump();
            }
        }

        if(useStartPos){
            rb.isKinematic = false;
        }
    }

    void LateUpdate(){
        ControlDragAndSize();
    }

    private bool CheckGround(){
        foreach(Vector3 position in cachedPositions){
            if(Physics.Raycast(position + transform.position, Vector3.down, raycastSize, groundMask)){
                return true;
            }
        }
        return false;
    }

    private Vector3 GetPosAtAngle(Vector3 centre, Vector2 distances, float angle){
        float radians = Mathf.Deg2Rad * angle;

        float xOffset = Mathf.Cos(radians) * (distances.x - raycastPadding.x);
        float zOffset = Mathf.Sin(radians) * (distances.y - raycastPadding.y);

        return new Vector3(centre.x + xOffset, centre.y, centre.z + zOffset);
    }

    private void GetInput(){
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
    }

    private void Jump(){
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        lastJumpTime = Time.time;
    }

    private void ControlDragAndSize(){ 
        if(isGrounded){
            rb.linearDamping = groundDrag;

            playerCollider.height = standardHeight;
        } 
        else{
            rb.linearDamping = airDrag;

            playerCollider.height = standardHeight/heightDivision;
        }
    }

    void FixedUpdate(){
        Movement();
    }

    private void Movement(){
        if(isGrounded){
            rb.AddForce(moveDirection.normalized * (speed * movementMultiplier + (Input.GetAxisRaw("Sprint") * sprintSpeed)), ForceMode.Acceleration);
            if(!playerFootstepsSound.playNow){
                if(Input.GetAxisRaw("Sprint") == 0){
                    playerFootstepsSound.ChangeBPM(playerFootstepsSound.GetOriginalBPM());
                }
                else{ 
                    playerFootstepsSound.ChangeBPM(playerFootstepsSound.GetOriginalBPM()+40);
                }

                if(moveDirection.normalized != Vector3.zero){
                    playerFootstepsSound.PlaySound(transform.position);
                }
            }
        }
        else{
            rb.AddForce(moveDirection.normalized * speed * movementMultiplier * jumpMultiplier, ForceMode.  Acceleration);
        }
    }

    void OnDrawGizmos(){
        foreach(Vector3 position in cachedPositions){
            if(isGrounded){
                Debug.DrawRay(position + transform.position, Vector3.down * raycastSize, Color.green);
            }
            else{ 
                Debug.DrawRay(position + transform.position, Vector3.down * raycastSize, Color.red);
            }
        }
    }
}
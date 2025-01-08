using UnityEngine;
using System.Collections.Generic;
using FMOD.Studio;

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

    private Vector3 playerBoundsExtents;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpMultiplier = 0.4f;
    [SerializeField] private float airDrag = 2f;
    private float lastJumpTime;
    [SerializeField] private float jumpIntervalTime;

// ----------------------------------------------------------------

    [Header("Using Raycasts")]
    [SerializeField] private Vector2 raycastPadding;

    private List<Vector3> chachedPositions = new List<Vector3>();

    [Range(0,360)]
    [SerializeField] private int innerIterations;
    [Range(0,360)]
    [SerializeField] private int iterations;

    [Range(0.1f,1f)]
    [SerializeField] private float raycastSize;

// ----------------------------------------------------------------

    [Header("isGrounded")]
    private bool isGrounded;

// ----------------------------------------------------------------

    // [Header("Sound")]
    // private EventInstance playerFootSteps;

// ----------------------------------------------------------------


    void Awake(){
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        moveDirection = Vector3.zero;

        playerBoundsExtents = playerRenderer.bounds.extents;

        chachedPositions.Add(new Vector3(0, -playerBoundsExtents.y + 0.1f,0));
        for(int i = 0; i < innerIterations; i++){
            float angle = (360 / innerIterations) * i;

            chachedPositions.Add(GetPosAtAngle(new Vector3(0, -playerBoundsExtents.y + 0.1f, 0), new Vector2(playerBoundsExtents.x/2, playerBoundsExtents.z/2), angle));
        }
        for(int i = 0; i < iterations; i++){
            float angle = (360 / iterations) * i;
            
            chachedPositions.Add(GetPosAtAngle(new Vector3(0, -playerBoundsExtents.y + 0.1f, 0), new Vector2(playerBoundsExtents.x, playerBoundsExtents.z), angle));
        }
    }

    // void Start(){
    //     playerFootSteps = AudioManager.instance.CreatEventInstance(FMODEvents.instance.playerFootSteps);
    // }

    void Update(){
        GetInput();
        ControlDrag();

        isGrounded = CheckGround();
        if(Input.GetButtonDown("Jump")){
            if(isGrounded && Time.time - lastJumpTime > jumpIntervalTime){
                Jump();
            }
        }
    }

    private bool CheckGround(){
        foreach(Vector3 position in chachedPositions){
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

        // UpdateSound();
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
        foreach(Vector3 position in chachedPositions){
            if(isGrounded){
                Debug.DrawRay(position + transform.position, Vector3.down * raycastSize, Color.green);
            }
            else{ 
                Debug.DrawRay(position + transform.position, Vector3.down * raycastSize, Color.red);
            }
        }
    }

    // private void UpdateSound(){
    //     if(rb.linearVelocity.x != 0 && isGrounded){
    //         PLAYBACK_STATE playbackState;
    //         playerFootSteps.getPlaybackState(out playbackState);
    //         if(playbackState.Equals(PLAYBACK_STATE.STOPPED)){
    //             playerFootSteps.start();
    //         }
    //     }
    //     else{ 
    //         playerFootSteps.stop(STOP_MODE.ALLOWFADEOUT);
    //     }
    // }
}
using UnityEngine;

public class GrabObjects : MonoBehaviour
{
    [Header("CubicleGeneratorV2")] // used specifically to call the keepObejct function, to update all lists to keep this object
    [SerializeField] private CubicleGeneratorV2 cubicleGeneratorV2;

    [Header("Cursor")] // changing the colours of the cursor to match if the player can grab and to tell when an object is too far
    [SerializeField] private Material cursorMat; 

    [SerializeField] private Color canGrabColor;
    [SerializeField] private Color cantGrabColor;

    [SerializeField] private float colorLerpAmount;

    [Header("Raycasting")] // variables used in the raycast needed for grabbing the object and holding it there
    [SerializeField] private GameObject cam;

    private Ray mainRay;

    [SerializeField] private LayerMask playerMask;

    [SerializeField] private float grabDist;

    private bool canGrab = false;

    [Header("Grabbed Object Values")] // all variables to do with the setting and retreiving values with the grabbedObj (NOT MOVEMENT)
    private GameObject grabbedObj;

    private Rigidbody grabbedObjrb;

    [SerializeField] private string grabTag;

    private float beforeAngleDrag;

    [SerializeField] private int objRotDrag;

    private bool isGrabbing = false;

    [Header("Grabbed Object Movement")] // all values with movement inside the grabbed object

    [SerializeField] private float baseMoveSpeed;
    private float moveSpeed;
    [SerializeField] private float objectMaxDistance;

    [SerializeField] private int holdDist;

    private bool tooFar = false;

    void Update(){
        mainRay = new Ray(cam.transform.position, cam.transform.forward);
        if(!isGrabbing){
            if(Physics.Raycast(mainRay, out RaycastHit hit, grabDist, playerMask)){
                if(hit.collider.GetComponent<Rigidbody>()){
                    canGrab = true;
                    if(Input.GetKeyDown(KeyCode.Mouse0)){ 
                        grabbedObj = hit.transform.gameObject;
                        isGrabbing = true;
                        GrabObjectValues();
                    }
                }
                else{
                    canGrab = false;
                }
            }
            else{
                canGrab = false;
            }
        }
        else if(Input.GetKeyDown(KeyCode.Mouse0) && isGrabbing || tooFar){
            isGrabbing = false;
            tooFar = false;
            LetGoObjectValues();
            grabbedObj = null;
        }

        if(canGrab){
            cursorMat.color = Color.Lerp(cursorMat.color, canGrabColor, Time.deltaTime * colorLerpAmount);
        }
        else{ 
            cursorMat.color = Color.Lerp(cursorMat.color, cantGrabColor, Time.deltaTime * (colorLerpAmount / 3));
        }

        if(isGrabbing){
            if(Vector3.Distance(grabbedObj.transform.position, transform.position) > objectMaxDistance){
                cursorMat.color = Color.red;
                tooFar = true;
            }
        }
    }

    void FixedUpdate(){
        if(isGrabbing){ 
            MoveObject();
        }
    }

    private void MoveObject(){
        Vector3 targetPosition = mainRay.origin + mainRay.direction * holdDist;

        Vector3 direction = (targetPosition - grabbedObjrb.position).normalized;
        float distance = Vector3.Distance(targetPosition, grabbedObj.transform.position);
        
        grabbedObjrb.linearVelocity = direction * moveSpeed * distance;
    }

    private void GrabObjectValues(){
        grabbedObjrb = grabbedObj.GetComponent<Rigidbody>();

        grabbedObjrb.useGravity = false;
        grabbedObjrb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        beforeAngleDrag = grabbedObjrb.angularDamping;
        grabbedObjrb.angularDamping = objRotDrag;

        grabbedObj.tag = "StayInScene";

        cubicleGeneratorV2.KeepObject(grabbedObj);

        moveSpeed = baseMoveSpeed/grabbedObjrb.mass;
    }

    private void LetGoObjectValues(){
        grabbedObjrb.useGravity = true;

        grabbedObjrb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        grabbedObjrb.angularDamping = beforeAngleDrag;
    }

    void OnDrawGizmos(){
        if(isGrabbing){
            Debug.DrawRay(mainRay.origin, mainRay.direction * holdDist, Color.cyan);

            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, objectMaxDistance);
        }
        else{
            Debug.DrawRay(mainRay.origin, mainRay.direction * grabDist, Color.blue);
        }

    }
}

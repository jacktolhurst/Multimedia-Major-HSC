using UnityEngine;

public class GrabObjects : MonoBehaviour
{
    [SerializeField] private CubicleGeneratorV2 cubicleGeneratorV2;

    [SerializeField] private Material cursorMat;

    [SerializeField] private Color canGrabColor;
    [SerializeField] private Color cantGrabColor;

    [SerializeField] private GameObject cam;
    private GameObject grabbedObj;

    private Rigidbody grabbedObjrb;

    private Ray mainRay;

    [SerializeField] private LayerMask playerMask;

    [SerializeField] private string grabTag;

    private Vector3 targetPosition;

    [SerializeField] private float colorLerpAmount;
    [SerializeField] private float closeAmountSmoothing;
    private float beforeAngleDrag;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float objectMaxDistance;
    [SerializeField] private float power;

    [SerializeField] private int grabDist;
    [SerializeField] private int holdDist;
    [SerializeField] private int objRotDrag;

    private bool isGrabbing = false;
    private bool canGrab = false;
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
            cursorMat.color = Color.Lerp(cursorMat.color, cantGrabColor, Time.deltaTime * colorLerpAmount);
        }

        if(isGrabbing && Vector3.Distance(grabbedObj.transform.position, transform.position) > objectMaxDistance){
            Debug.Log("too far");
            cursorMat.color = Color.red;
            tooFar = true;
        }
    }

    void FixedUpdate(){
        if(isGrabbing){ 
            MoveObject();
        }
    }

    private void MoveObject(){
        targetPosition = mainRay.origin + mainRay.direction * holdDist;

        Vector3 direction = (targetPosition - grabbedObjrb.position).normalized;
        float distance = Vector3.Distance(grabbedObj.transform.position, transform.position);
        grabbedObjrb.linearVelocity = direction * moveSpeed * Mathf.Pow(distance, power);


        Debug.Log(grabbedObjrb.linearVelocity);
    }

    private void GrabObjectValues(){
        grabbedObjrb = grabbedObj.GetComponent<Rigidbody>();

        grabbedObjrb.useGravity = false;
        grabbedObjrb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        beforeAngleDrag = grabbedObjrb.angularDamping;
        grabbedObjrb.angularDamping = objRotDrag;

        grabbedObj.tag = "StayInScene";

        cubicleGeneratorV2.KeepObject(grabbedObj);
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

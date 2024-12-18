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

    private float lerpAmount;
    [SerializeField] private float colorLerpAmount;
    private float maxMass = 0;
    [SerializeField] private float closeAmountSmoothing;
    private float beforeAngleDrag;
    [SerializeField] private float lerpAmountTimes;

    [SerializeField] private int grabDist;
    [SerializeField] private int holdDist;
    [SerializeField] private int objRotDrag;

    private bool isGrabbing = false;
    private bool canGrab = false;

    void Start(){
        Rigidbody[] allObjects = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
        foreach(Rigidbody objRb in allObjects){
            if(objRb.mass > maxMass){
                maxMass = objRb.mass;
            }
        }
        maxMass /= 50;
    }

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
        else if(Input.GetKeyDown(KeyCode.Mouse0) && isGrabbing){
            isGrabbing = false;
            LetGoObjectValues();
            grabbedObj = null;
        }
        
        if(isGrabbing){ 
            MoveObject();
        }

        if(canGrab){
            cursorMat.color = Color.Lerp(cursorMat.color, canGrabColor, Time.deltaTime * colorLerpAmount);
        }
        else{ 
            cursorMat.color = Color.Lerp(cursorMat.color, cantGrabColor, Time.deltaTime * colorLerpAmount);
        }
    }

    private void MoveObject(){
        if(Physics.Raycast(mainRay, out RaycastHit hit, holdDist, playerMask) && hit.transform.gameObject != grabbedObj){
            Vector3 lerpedPos = Vector3.Lerp(grabbedObj.transform.position, hit.point, lerpAmount * Time.deltaTime);
            if(!isClose(grabbedObj.transform.position, lerpedPos, closeAmountSmoothing)){
                grabbedObj.transform.position = lerpedPos;
            }
        }
        else{
            Vector3 lerpedPos = Vector3.Lerp(grabbedObj.transform.position, mainRay.origin + mainRay.direction * holdDist, lerpAmount * Time.deltaTime);
            if(!isClose(grabbedObj.transform.position, lerpedPos, closeAmountSmoothing)){
                grabbedObj.transform.position = lerpedPos;
            }
        }

        grabbedObjrb.linearVelocity = Vector3.zero;
    }

    private void GrabObjectValues(){
        grabbedObjrb = grabbedObj.GetComponent<Rigidbody>();

        grabbedObjrb.useGravity = false;
        grabbedObjrb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        beforeAngleDrag = grabbedObjrb.angularDamping;
        grabbedObjrb.angularDamping = objRotDrag;

        lerpAmount = (maxMass/grabbedObjrb.mass) * lerpAmountTimes;

        grabbedObj.tag = "StayInScene";

        cubicleGeneratorV2.KeepObject(grabbedObj);
    }

    private void LetGoObjectValues(){
        grabbedObjrb.useGravity = true;

        grabbedObjrb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        grabbedObjrb.angularDamping = beforeAngleDrag;
    }

    void OnDrawGizmos(){
        Debug.DrawRay(mainRay.origin, mainRay.direction * grabDist, Color.magenta);
    }

    private bool isClose(Vector3 APos, Vector3 BPos, float dist){
        return Vector3.Distance(APos, BPos) < dist;
    }



}

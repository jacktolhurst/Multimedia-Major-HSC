using UnityEngine;
using System.Collections;

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
    [SerializeField] private float massDivision;
    [SerializeField] private float throwSpeed;

    [SerializeField] private int holdDist;

    private bool tooFar = false;



    private GameObject childObj;

    [Header("Motion Effects")] // everything to do with effects done while in play
    [SerializeField] private Material blurMat;

    private Vector3 lastPosition;

    [SerializeField] private float shaderMovementSmoothing;
    [SerializeField] private float speedDivision;
    [SerializeField] private float alphaMultiplier;

    private bool doShow = false;

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

            ManageChildMaterial();

            lastPosition = Vector3.Lerp(lastPosition, grabbedObj.transform.position, Time.deltaTime * shaderMovementSmoothing);
            childObj.transform.position = lastPosition;

        }
    }

    void FixedUpdate(){
        if(isGrabbing){ 
            MoveObject();
        }
    }

    private void ManageChildMaterial(){
        float magnitude = grabbedObjrb.linearVelocity.magnitude / speedDivision;
        Vector3 normalizedDirection = grabbedObjrb.linearVelocity.normalized;
        blurMat.SetVector("_ObjectSizeVector", magnitude * normalizedDirection);

        float rounded = (float)Mathf.Round(Vector3.Distance(childObj.transform.position, grabbedObj.transform.position) * 10) / 10;
        blurMat.SetFloat("_Alpha", rounded * alphaMultiplier);

        blurMat.SetColor("_Color", grabbedObjrb.GetComponent<Renderer>().material.color);
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
        grabbedObjrb.useGravity = true;
        grabbedObjrb.isKinematic = false;

        grabbedObj.tag = "StayInScene";

        cubicleGeneratorV2.KeepObject(grabbedObj);
        foreach(Transform child in grabbedObj.transform){
            cubicleGeneratorV2.KeepObject(child.gameObject);
        }
        moveSpeed = Mathf.Clamp(baseMoveSpeed/(grabbedObjrb.mass / massDivision),baseMoveSpeed/2, baseMoveSpeed);

        childObj = Instantiate(grabbedObj);
        foreach(Component component in childObj.GetComponents<Component>()){
            if(!(component is MeshRenderer) && !(component is MeshFilter) && !(component is Transform)){
                Destroy(component);
            }
        }
        childObj.GetComponent<MeshRenderer>().materials = new Material[0];
        childObj.GetComponent<MeshRenderer>().material = blurMat;

        childObj.layer = 0;
        childObj.transform.parent = grabbedObj.transform;

        doShow = true;
        StartCoroutine(ShaderInterval());
    }

    private void LetGoObjectValues(){
        grabbedObjrb.linearVelocity = mainRay.direction * throwSpeed/grabbedObjrb.mass;

        grabbedObjrb.useGravity = true;

        grabbedObjrb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        grabbedObjrb.angularDamping = beforeAngleDrag;

        doShow = false;
        StopCoroutine(ShaderInterval());
        Destroy(childObj);
    }

    private IEnumerator ShaderInterval(){
        while(doShow){
            lastPosition = grabbedObj.transform.position;
            yield return new WaitForSeconds(1);
        }
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

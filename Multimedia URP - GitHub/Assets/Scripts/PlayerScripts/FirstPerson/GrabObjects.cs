using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    [HideInInspector] public bool canGrab = false;

    [Header("Grabbed Object Values")] // all variables to do with the setting and retreiving values with the grabbedObj (NOT MOVEMENT)
    private GameObject grabbedObj;
    private GameObject childObj;

    private Collider selfCollider;

    private Rigidbody grabbedObjrb;

    private MeshRenderer childObjMeshRenderer;

    private float beforeAngleDrag;

    [SerializeField] private int objRotDrag;

    private bool isGrabbing = false;

    [Header("Grabbed Object Movement")] // all values with movement inside the grabbed object
    [SerializeField] private float baseMoveSpeed;
    private float moveSpeed;
    [SerializeField] private float objectMaxDistance;
    [SerializeField] private float massDivision;
    [SerializeField] private float throwSpeed;
    [SerializeField] private float baseHoldDist;
    private float holdDist;
    [SerializeField] private float minGrabbedObjDist;
    [SerializeField] private float maxGrabbedObjDist;
    private float prevMass;

    private bool tooFar = false;
    private bool inactiveObj = false;

    [Header("Effects")]
    [SerializeField] private int effectMask;
    private int defaultMask;

    [Header("Audio")]
    [SerializeField] private AudioManager.AudioReferenceClass grabbingObjectSound;

    void Awake(){
        selfCollider = transform.GetChild(0).GetComponent<Collider>();
    }

    void Update(){
        mainRay = new Ray(cam.transform.position, cam.transform.forward);
        if(!isGrabbing){
            if(Physics.Raycast(mainRay, out RaycastHit hit, grabDist, playerMask)){
                if(hit.collider.GetComponent<Rigidbody>()){
                    canGrab = true;
                    if(Input.GetKeyDown(KeyCode.Mouse0)){ 
                        grabbedObj = hit.transform.gameObject;
                        grabbingObjectSound.PlaySoundObject(hit.transform.gameObject);
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
        else if(Input.GetKeyDown(KeyCode.Mouse0) && isGrabbing || tooFar || inactiveObj){
            isGrabbing = false;
            tooFar = false;
            inactiveObj = false;
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
            if(grabbedObj == null || !grabbedObj.activeSelf){
                cursorMat.color = Color.red;
                inactiveObj = true;
            }
            else{
                if(Vector3.Distance(grabbedObj.transform.position, transform.position) > objectMaxDistance){
                    cursorMat.color = Color.red;
                    tooFar = true;
                }
            }
        }
    }

    void FixedUpdate(){
        if(isGrabbing &&  grabbedObjrb != null){ 
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
        grabbedObjrb.useGravity = true;
        grabbedObjrb.isKinematic = false;
        grabbedObjrb.interpolation = RigidbodyInterpolation.Interpolate;

        if(grabbedObj.transform.parent != null && grabbedObj.transform.parent.transform.parent != null){
            if(grabbedObj.transform.parent.transform.parent.name.ToLower().Contains("parent")){
                grabbedObj.tag = "StayInScene";

                cubicleGeneratorV2.KeepObject(grabbedObj);
            }
        }

        CheckChildren(effectMask, grabbedObj.transform);
        
        moveSpeed = Mathf.Clamp(baseMoveSpeed/(grabbedObjrb.mass / massDivision),baseMoveSpeed/2, baseMoveSpeed);
        if(grabbedObj.GetComponent<Renderer>() != null){
            holdDist = Mathf.Clamp((grabbedObjrb.mass * grabbedObj.GetComponent<Renderer>().bounds.extents.magnitude) + baseHoldDist, minGrabbedObjDist, maxGrabbedObjDist);
        }
        else{
            holdDist = Mathf.Clamp(grabbedObjrb.mass + baseHoldDist, minGrabbedObjDist, maxGrabbedObjDist);
        }

        if(grabbedObj.layer != 6){
            foreach(Collider collider in grabbedObj.GetComponents<Collider>()){
                Physics.IgnoreCollision(collider, selfCollider, true);
            }
        }

        defaultMask = grabbedObj.layer;
        grabbedObj.layer = effectMask;

        prevMass = grabbedObjrb.mass;
        grabbedObjrb.mass = 20 * grabbedObjrb.mass;
    }

    private void LetGoObjectValues(){
        if (grabbedObj == null || grabbedObjrb == null) return;


        grabbedObjrb.linearVelocity = grabbedObjrb.linearVelocity + mainRay.direction * throwSpeed/prevMass;

        grabbedObjrb.useGravity = true;

        grabbedObjrb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        grabbedObjrb.angularDamping = beforeAngleDrag;

        grabbedObjrb.mass = prevMass;

        foreach(Collider collider in grabbedObj.GetComponents<Collider>()){
            Physics.IgnoreCollision(collider, selfCollider, false);
        }

        CheckChildren(defaultMask, grabbedObj.transform);
        grabbedObj.layer = defaultMask;
    }

    private void CheckChildren(int mask, Transform objTrans){
        foreach(Transform child in objTrans){
            child.gameObject.layer = mask;
            CheckChildren(mask, child);
        }
    }

    void OnDrawGizmos(){
        if(isGrabbing){
            Debug.DrawRay(mainRay.origin, mainRay.direction * holdDist, Color.cyan);
        }
        else{
            Debug.DrawRay(mainRay.origin, mainRay.direction * grabDist, Color.blue);
        }
    }
}

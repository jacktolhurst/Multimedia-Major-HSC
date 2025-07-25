using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 
using DG.Tweening;

[SelectionBase]
public class OrbManager : MonoBehaviour
{
    private List<GameObject> deletedObjs = new List<GameObject>();

    private Bounds selfBounds;

    private Vector3 baseSize;
    private Vector3 projectedScale;

    [SerializeField] private float baseForceSize;
    [SerializeField] private float orbForce;
    [SerializeField] private float sizeSpeed;

    private int lastDeletedObjsSize;

    void Awake(){
        selfBounds = GetBoundsFromObj(transform.gameObject);
        baseSize = selfBounds.size;
    }

    void Update(){
        ManageRigidbodies();
        ManageScaling();
    }

    private void ManageRigidbodies(){
        ApplyForceToRigidbodies(GetRigidbodiesInRange(GetCheckSize(selfBounds.size, baseForceSize)), orbForce);
        ApplyDeletionToRigidbodies(GetRigidbodiesInRange(GetCheckSize(selfBounds.size, -1)));
    }

    private void ManageScaling(){
        if(lastDeletedObjsSize != deletedObjs.Count){
            DOTween.Kill(transform);

            projectedScale = ListToScale(deletedObjs, baseSize);
            transform.DOScale(projectedScale, sizeSpeed);

            lastDeletedObjsSize = deletedObjs.Count;
        }
    } 

    private void ApplyForceToRigidbodies(List<Rigidbody> rigidbodies, float baseForce){
        foreach(Rigidbody body in rigidbodies){
            Vector3 direction = (transform.position - body.position).normalized;
            float distance = Vector3.Distance(transform.position, body.position);

            body.AddForce(direction * (baseForce/distance), ForceMode.Acceleration);
        }
    }

    private void ApplyDeletionToRigidbodies(List<Rigidbody> rigidbodies){
        foreach(Rigidbody body in rigidbodies){
            GameObject obj = body.gameObject;
            if(!deletedObjs.Contains(obj)){
                StartCoroutine(TrackDestroyObject(obj, 0.5f));
                deletedObjs.Add(obj);
            }
        }
    }

    private float GetCheckSize(Vector3 givenScale, float baseSize){
        float average = (givenScale.x + givenScale.y + givenScale.z)/3;
        return average + baseSize;
    }

    private Vector3 ListToScale(List<GameObject> objs, Vector3 baseSize, float multiplier=1){
        Vector3 countInVector3 = new Vector3(1,1,1) * objs.Count * multiplier;
        return baseSize + countInVector3;
    }

    private Bounds GetBoundsFromObj(GameObject obj){
        return obj.GetComponent<Collider>().bounds;
    }

    private List<Rigidbody> GetRigidbodiesInRange(float range){
        List<GameObject> alreadyCheckedObjects = new List<GameObject>();
        List<Rigidbody> rigidbodiesInRange = new List<Rigidbody>();
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, range);

        foreach(Collider collider in collidersInRange){
            GameObject obj = collider.gameObject;
            if(obj.GetComponent<Rigidbody>() && !alreadyCheckedObjects.Contains(obj)){
                rigidbodiesInRange.Add(obj.GetComponent<Rigidbody>());
                alreadyCheckedObjects.Add(obj);
            }
        }

        return rigidbodiesInRange;
    }

    private IEnumerator TrackDestroyObject(GameObject obj, float scaleDuration){
        float deletionTime = Time.time + scaleDuration;

        Transform objTransform = obj.transform;
        Rigidbody objRigidbody = obj.GetComponent<Rigidbody>();
        objRigidbody.linearDamping = 100;

        objTransform.DOScale(Vector3.zero, scaleDuration);
        
        while(Time.time < deletionTime){
            objTransform.position = Vector3.Lerp(objTransform.position, transform.position, 5f*Time.deltaTime);
            yield return null;
        }

        DOTween.Kill(objTransform);
        Destroy(obj);
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, GetCheckSize(selfBounds.size, baseForceSize));
        Gizmos.DrawWireSphere(transform.position, GetCheckSize(selfBounds.size, -1));
    }

    // private List<GameObject> objects = new List<GameObject>();
    // [SerializeField] private GameObject wire;
    
    // [SerializeField] private ManagerScript manager;
    // [SerializeField] private CardReaderDoor cardReaderDoor;

    // [SerializeField] private ParticleSystem childParticleSystem;

    // [SerializeField] private Light mainLight;

    // Collider[] forceColliders;
    // Collider[] sceneColliders;

    // Material[] objMaterials;

    // private Vector3 baseSize;
    // private Vector3 projectedScale;
    // [SerializeField] private Vector3 speakerPosition;

    // [SerializeField] private float baseForceRadius;
    // private float forceRadius;
    // [SerializeField] private float baseForce;
    // private float sceneRadius;
    // [SerializeField] private float orbSpeed;
    // private float initialIntensity;
    // [SerializeField] private float maxSize;

    // private bool statedSceneCheck;
    // private bool statedChangeTransform;
    // private bool statedForceCheck;

    // void Awake(){
    //     baseSize = transform.localScale;

    //     projectedScale = transform.localScale;

    //     initialIntensity = mainLight.intensity;
    // }

    // void Start(){
    //     objMaterials = GetComponent<Renderer>().materials;
    // }

    // void Update(){
    //     if(cardReaderDoor.unlocked && !statedChangeTransform && !statedSceneCheck){
    //         StartCoroutine(SceneCheck());
    //         StartCoroutine(ChangeTransform());
    //     }

    // }

    // void FixedUpdate(){
    //     if(cardReaderDoor.unlocked && !statedForceCheck){
    //         StartCoroutine(ForceCheck());
    //     }
    // }

    // private IEnumerator ChangeTransform(){
    //     statedChangeTransform = true;
    //     while(true){
    //         transform.localScale = Vector3.Lerp(transform.localScale, projectedScale, orbSpeed * Time.deltaTime);

    //         Vector3 localScale = transform.localScale;

    //         sceneRadius = (localScale.x/2) - 0.4f;
    //         forceRadius = baseForceRadius * sceneRadius;

    //         var shape = childParticleSystem.shape;
    //         shape.scale = localScale;

    //         mainLight.intensity = initialIntensity + ((localScale.magnitude - baseSize.magnitude) * 3000);
    //         mainLight.range = forceRadius

    //         wire.SetActive(true);

    //         if(maxSize <= localScale.magnitude){
    //             cardReaderDoor.unlocked = false;

    //             objMaterials[0].SetFloat("_Speed", 0.01f);

    //             maxSize = 1000000;
    //         }

    //         objMaterials[0].SetFloat("_MovementIntensity", Mathf.Min(0.3f, localScale.magnitude/100));
    //         objMaterials[1].SetFloat("_Outline", Mathf.Min(0.3f, (localScale.magnitude/100) + 0.05f));

    //         yield return null;
    //     }
    // }

    // private IEnumerator SceneCheck(){
    //     statedSceneCheck = true;
    //     while(true){
    //         sceneColliders = Physics.OverlapSphere(transform.position, sceneRadius);

    //         foreach(Collider collider in sceneColliders){
    //             GameObject obj = collider.transform.gameObject;
    //             if(obj.layer == 9){
    //                 // TODO: change scene to the next
    //                 manager.RestartScene();
    //             }
    //             else if(obj.layer != 7 && obj.layer != 8 && obj.layer != 0){
    //                 objects.Add(obj);
    //                 if(obj.GetComponent<Renderer>() != null){
    //                     projectedScale += new Vector3(1,1,1) * obj.GetComponent<Renderer>().bounds.extents.magnitude;
    //                 }
    //                 foreach(Transform child in transform){
    //                     if(child.GetComponent<Renderer>() != null){
    //                         projectedScale += new Vector3(1,1,1) * child.GetComponent<Renderer>().bounds.extents.magnitude;
    //                     }
    //                 }
    //                 obj.SetActive(false);
    //             }
    //         }

    //         yield return null;
    //     }
    // }

    // private IEnumerator ForceCheck(){
    //     statedForceCheck = true;
    //     while(true){
    //         forceColliders = Physics.OverlapSphere(transform.position, forceRadius);

    //         foreach(Collider collider in forceColliders){
    //             Rigidbody rb = collider.GetComponent<Rigidbody>();
    //             if(collider.transform.parent != null){
    //                 if(collider.transform.parent.GetComponent<Rigidbody>() != null){
    //                     rb = collider.transform.parent.GetComponent<Rigidbody>();
    //                 }
    //             }
    //             if(rb != null && collider.transform.gameObject.layer != 8 && collider.transform.gameObject.layer != 7 && collider.transform.gameObject.layer != 0){
    //                 rb.isKinematic = false;

    //                 Vector3 direction = (transform.position - rb.position).normalized;
    //                 float distance = Vector3.Distance(transform.position, rb.position);
    //                 float force = (baseForce * 2/distance) * (Vector3.Distance(baseSize, transform.localScale) + 1);

    //                 rb.AddForce(direction * force, ForceMode.Acceleration);

    //                 Debug.DrawRay(rb.position, direction * distance, Color.red);
    //             }
    //         }

    //         yield return null;
    //     }
    // }

    // void OnDrawGizmos(){
    //     Gizmos.color = Color.magenta;
    //     Gizmos.DrawWireSphere(transform.position, forceRadius);

    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawWireSphere(transform.position, sceneRadius);

    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireSphere(speakerPosition, 2);
    // }

}

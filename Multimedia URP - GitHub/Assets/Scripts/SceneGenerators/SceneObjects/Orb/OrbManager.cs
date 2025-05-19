using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

[SelectionBase]
public class OrbManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> deletedObjs = new List<GameObject>();

    [SerializeField] private ManagerScript manager;

    private Bounds selfBounds;

    private float sceneRadius; 
    [SerializeField] private float forceRadius; 
    [SerializeField] private float attractorMass;

    void Start(){
        selfBounds = GetComponent<Collider>().bounds;
    }

    void Update(){
        ForceAddition();
        SceneCheck();
    }

    private void ForceAddition(){
        Collider[] objColliders = Physics.OverlapSphere(transform.position, forceRadius);
        foreach(Collider collider in objColliders){
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if(rb == null) continue;

            Vector3 direction = (transform.position - rb.position).normalized;
            float distance = Mathf.Max(Vector3.Distance(transform.position, rb.position), 0.1f);
            float force = attractorMass;

            rb.AddForce(direction * force, ForceMode.Acceleration);
            Debug.DrawRay(rb.position, direction * distance, Color.red);
        }
    }

    private void SceneCheck(){
        sceneRadius = transform.localScale.magnitude/3;

        Collider[] objColliders = Physics.OverlapSphere(transform.position, sceneRadius);
        foreach(Collider collider in objColliders){
            GameObject obj = collider.transform.gameObject;
            Bounds objBounds = obj.GetComponent<Collider>().bounds;
            int layer = obj.layer;

            if(layer == 11 || layer == 12){
                if(selfBounds.Contains(objBounds.max) && selfBounds.Contains(objBounds.min)){
                    deletedObjs.Add(obj);
                    obj.SetActive(false);
                }
            }
            if(layer == 9){
                manager.RestartScene();
            }
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, sceneRadius);
        Gizmos.DrawWireSphere(transform.position, forceRadius);
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

    // private Vector3 baseScale;
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
    //     baseScale = transform.localScale;

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

    //         mainLight.intensity = initialIntensity + ((localScale.magnitude - baseScale.magnitude) * 3000);
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
    //                 float force = (baseForce * 2/distance) * (Vector3.Distance(baseScale, transform.localScale) + 1);

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

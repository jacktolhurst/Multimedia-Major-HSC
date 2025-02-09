using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

public class OrbManager : MonoBehaviour
{
    private List<GameObject> objects = new List<GameObject>();
    [SerializeField] private GameObject wire;
    
    [SerializeField] private ManagerScript manager;
    [SerializeField] private CardReaderDoor cardReaderDoor;

    [SerializeField] private ParticleSystem childParticleSystem;

    [SerializeField] private Light mainLight;

    Collider[] forceColliders;
    Collider[] sceneColliders;

    private Vector3 baseScale;
    private Vector3 projectedScale;

    [SerializeField] private float baseForceRadius;
    private float forceRadius;
    [SerializeField] private float baseForce;
    private float sceneRadius;
    [SerializeField] private float orbSpeed;
    private float initialIntensity;
    [SerializeField] private float maxSize;

    private bool statedSceneCheck;
    private bool statedChangeTransform;
    private bool statedForceCheck;

    void Awake(){
        baseScale = transform.localScale;

        projectedScale = transform.localScale;

        initialIntensity = mainLight.intensity;
    }

    void Update(){
        if(cardReaderDoor.unlocked && !statedChangeTransform && !statedSceneCheck){
            StartCoroutine(SceneCheck());
            StartCoroutine(ChangeTransform());
        }
    }

    void FixedUpdate(){
        if(cardReaderDoor.unlocked && !statedForceCheck){
            StartCoroutine(ForceCheck());
        }
    }
    private IEnumerator ChangeTransform(){
        statedChangeTransform = true;
        while(true){
            transform.localScale = Vector3.Lerp(transform.localScale, projectedScale, orbSpeed * Time.deltaTime);

            sceneRadius = (transform.localScale.x/2) - 0.4f;
            forceRadius = baseForceRadius * sceneRadius;

            var shape = childParticleSystem.shape;
            shape.scale = transform.localScale;

            mainLight.intensity = initialIntensity + ((transform.localScale.magnitude - baseScale.magnitude) * 3000);
            mainLight.range = forceRadius;

            wire.SetActive(true);

            if(maxSize <= transform.localScale.magnitude){
                cardReaderDoor.unlocked = false;
                maxSize = 1000000;
            }

            yield return null;
        }
    }

    private IEnumerator SceneCheck(){
        statedSceneCheck = true;
        while(true){
            sceneColliders = Physics.OverlapSphere(transform.position, sceneRadius);

            foreach(Collider collider in sceneColliders){
                GameObject obj = collider.transform.gameObject;
                if(obj.layer == 9){
                    // TODO: change scene to the next
                    manager.RestartScene();
                }
                else if(obj.layer != 7 && obj.layer != 8 && obj.layer != 0){
                    objects.Add(obj);
                    if(obj.GetComponent<Renderer>() != null){
                        projectedScale += new Vector3(1,1,1) * obj.GetComponent<Renderer>().bounds.extents.magnitude;
                    }
                    obj.SetActive(false);
                }
            }

            yield return null;
        }
    }

    private IEnumerator ForceCheck(){
        statedForceCheck = true;
        while(true){
            forceColliders = Physics.OverlapSphere(transform.position, forceRadius);

            foreach(Collider collider in forceColliders){
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if(collider.transform.parent != null){
                    if(collider.transform.parent.GetComponent<Rigidbody>() != null){
                        rb = collider.transform.parent.GetComponent<Rigidbody>();
                    }
                }
                if(rb != null && collider.transform.gameObject.layer != 8 && collider.transform.gameObject.layer != 7 && collider.transform.gameObject.layer != 0){
                    rb.isKinematic = false;

                    Vector3 direction = (transform.position - rb.position).normalized;
                    float distance = Vector3.Distance(transform.position, rb.position);
                    float force = (baseForce * 2/distance) * (Vector3.Distance(baseScale, transform.localScale) + 1);

                    rb.AddForce(direction * force, ForceMode.Acceleration);

                    Debug.DrawRay(rb.position, direction * distance, Color.red);
                }
            }

            yield return null;
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, forceRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sceneRadius);
    }
}

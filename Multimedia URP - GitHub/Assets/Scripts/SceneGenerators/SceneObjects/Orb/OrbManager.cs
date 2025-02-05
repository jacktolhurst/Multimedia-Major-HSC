using UnityEngine;
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

    void Awake(){
        baseScale = transform.localScale;

        projectedScale = transform.localScale;

        initialIntensity = mainLight.intensity;
    }

    void Update(){
        if(cardReaderDoor.unlocked){
            SceneCheck();

            transform.localScale = Vector3.Lerp(transform.localScale, projectedScale, orbSpeed * Time.deltaTime);

            sceneRadius = (transform.localScale.x/2) - 0.4f;
            forceRadius = baseForceRadius * sceneRadius;

            var shape = childParticleSystem.shape;
            shape.scale = transform.localScale;

            mainLight.intensity = initialIntensity + ((transform.localScale.magnitude - baseScale.magnitude) * 3000);
            mainLight.range = forceRadius;

            wire.SetActive(true);
        }
    }

    void FixedUpdate(){
        if(cardReaderDoor.unlocked){
            ForceCheck();
        }
    }

    private void SceneCheck(){
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
    }

    private void ForceCheck(){
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
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, forceRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sceneRadius);
    }
}

using UnityEngine;
using System.Collections.Generic; 

public class OrbManager : MonoBehaviour
{
    private List<GameObject> objects = new List<GameObject>();

    private ParticleSystem childParticleSystem;

    Collider[] forceColliders;
    Collider[] sceneColliders;

    private Vector3 baseScale;
    private Vector3 projectedScale;

    [SerializeField] private float baseForceRadius;
    private float forceRadius;
    [SerializeField] private float baseForce;
    private float sceneRadius;
    [SerializeField] private float orbSpeed;

    void Awake(){
        baseScale = transform.localScale;

        childParticleSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    void Update(){
        SceneCheck();

        projectedScale = baseScale * (objects.Count + 1);
        transform.localScale = Vector3.Lerp(transform.localScale, projectedScale, orbSpeed * Time.deltaTime);

        sceneRadius = (transform.localScale.x/2) - 0.1f;
        forceRadius = baseForceRadius * sceneRadius;

        var shape = childParticleSystem.shape;
        shape.scale = transform.localScale;

    }

    void FixedUpdate(){
        ForceCheck();
    }

    private void SceneCheck(){
        sceneColliders = Physics.OverlapSphere(transform.position, sceneRadius);

        foreach(Collider collider in sceneColliders){
            GameObject obj = collider.transform.gameObject;
            if(obj.layer == 9){
                // TODO: change scene to the next
            }
            else if(obj.layer != 7 && obj.layer != 8){
                objects.Add(obj);
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
            if(rb != null){
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

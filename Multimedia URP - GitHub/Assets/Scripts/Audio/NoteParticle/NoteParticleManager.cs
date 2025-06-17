using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NoteParticleManager : MonoBehaviour
{
    private GameObject followObj;

    private Rigidbody selfRb;

    private List<Collider> followObjColliders = new List<Collider>();
    private Collider selfCollider;

    private Vector3 targetPos;
    private Vector3 velocity = Vector3.zero;

    private float startTime;
    private float lifeTime;
    [HideInInspector] public float endTime;
    [HideInInspector] public float speed = 100;
    private float randScaleChangeTime;

    void Awake(){
        selfRb = GetComponent<Rigidbody>();
        selfRb.linearVelocity = Vector3.zero;
        
        selfCollider = GetComponent<Collider>();
        selfCollider.isTrigger = true;
    }

    public void StartObj(GameObject newFollowObj, float newEndTime){
        followObj = newFollowObj;
        endTime = newEndTime;

        selfCollider.isTrigger = false;
        foreach(Collider followObjCollider in followObj.GetComponents<Collider>()) {
            followObjColliders.Add(followObjCollider);
            Physics.IgnoreCollision(selfCollider, followObjCollider, true);
        }

        startTime = Time.time;
        lifeTime = endTime - startTime;

        randScaleChangeTime = ((startTime + lifeTime) - (lifeTime/3)) - Random.Range(-0.2f,0.2f);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0,360), transform.eulerAngles.z);

        transform.position = RandomPointInsideMesh(followObjColliders[Random.Range(0,followObjColliders.Count)]);

        selfRb = GetComponent<Rigidbody>();
        selfRb.linearVelocity = Vector3.zero;

        targetPos = transform.position + (Random.insideUnitSphere * 4);

        StartCoroutine(ColliderCheck());
    }

    public void StartPosition(float newEndTime){
        endTime = newEndTime;

        startTime = Time.time;
        lifeTime = endTime - startTime;

        randScaleChangeTime = ((startTime + lifeTime) - (lifeTime/3)) - Random.Range(-0.2f,0.2f);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0,360), transform.eulerAngles.z);    
    }

    private IEnumerator ColliderCheck(){
        yield return null;
        
        while(!followObjColliders.Any(followObjCollider => AreTouching(selfCollider, followObjCollider))){
            yield return null;
        }

        foreach(Collider followObjCollider in followObjColliders){
            Physics.IgnoreCollision(selfCollider, followObjCollider, false);
        }

        Vector3 dir = (targetPos - transform.position).normalized;
        selfRb.AddForce(dir * speed, ForceMode.VelocityChange);
    }   

    void Update(){
        if(Time.time > endTime){
            Destroy(transform.gameObject);
        }

        if(randScaleChangeTime < Time.time){   
            transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.zero, ref velocity, 0.1f);
        }
    }

    private bool AreTouching(Collider colliderA, Collider colliderB){
        Vector3 direction;
        float distance;
        return Physics.ComputePenetration(
            colliderA, colliderA.transform.position, colliderA.transform.rotation,
            colliderB, colliderB.transform.position, colliderB.transform.rotation,
            out direction, out distance
        );
    }
    private Vector3 RandomPointInsideMesh(Collider collider){
        Bounds bounds = collider.bounds;
        Vector3 point;
        int attempts = 0;

        do{
            point = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
            attempts++;
        }
        while (!collider.bounds.Contains(point) && attempts < 100);

        return point;
    }
}

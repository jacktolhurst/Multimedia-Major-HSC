using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NoteParticleManager : MonoBehaviour
{
    private Coroutine collideCheckCoroutine;

    private GameObject followObj;

    private Rigidbody selfRb;

    private List<Collider> followObjColliders = new List<Collider>();
    private Collider selfCollider;

    private Vector3 targetPos;
    private Vector3 velocity = Vector3.zero;

    private float startTime;
    private float lifeTime;
    [HideInInspector] public float endTime;
    private float speed = 1;
    private float randScaleChangeTime;

    void Awake(){
        selfRb = GetComponent<Rigidbody>();
        selfRb.linearVelocity = Vector3.zero;
        
        selfCollider = GetComponent<Collider>();
    }

    public void StartObj(GameObject newFollowObj, float newEndTime){ // used for if starting by given an object
        followObj = newFollowObj;
        endTime = newEndTime;

        foreach(Collider followObjCollider in followObj.GetComponents<Collider>()) {
            followObjColliders.Add(followObjCollider);
            Physics.IgnoreCollision(selfCollider, followObjCollider, true);
        }

        startTime = Time.time;
        lifeTime = endTime - startTime;

        randScaleChangeTime = ((startTime + lifeTime) - (lifeTime/3)) - Random.Range(-0.2f,0.2f);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0,360), transform.eulerAngles.z);

        selfRb.position = RandomPointInCollider(followObj.GetComponents<Collider>());

        collideCheckCoroutine = StartCoroutine(ColliderCheck());
    }

    public void StartPosition(float newEndTime){ // used for if starting by given a position
        endTime = newEndTime;

        startTime = Time.time;
        lifeTime = endTime - startTime;

        randScaleChangeTime = ((startTime + lifeTime) - (lifeTime/3)) - Random.Range(-0.2f,0.2f);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0,360), transform.eulerAngles.z);    

        targetPos = transform.position + (Random.insideUnitSphere*2);
        Vector3 dir = (targetPos - transform.position).normalized;
        selfRb.AddForce(dir * speed, ForceMode.VelocityChange);
    }

    private IEnumerator ColliderCheck(){
        while(!followObjColliders.Any(followObjCollider => AreTouching(selfCollider, followObjCollider))){
            yield return null;
        }

        foreach(Collider followObjCollider in followObjColliders){
            Physics.IgnoreCollision(selfCollider, followObjCollider, false);
        }

        targetPos = transform.position + (Random.insideUnitSphere*2);
        Vector3 dir = (targetPos - transform.position).normalized;
        selfRb.AddForce(dir * speed, ForceMode.VelocityChange);
    }   

    void Update(){
        if(Time.time > endTime){
            Destroy(transform.gameObject);
            if(collideCheckCoroutine != null){
                StopCoroutine(collideCheckCoroutine);
            }
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

    public Vector3 RandomPointInCollider(Collider[] colliders, int maxTries = 30){
        Collider col = colliders[Random.Range(0, colliders.Length)];
        var b = col.bounds;
        for (int i = 0; i < maxTries; i++)
        {
            Vector3 p = new Vector3(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y),
                Random.Range(b.min.z, b.max.z)
            );
            if (col.ClosestPoint(p) == p)
                return p;
        }
        return col.bounds.center;
    }

}

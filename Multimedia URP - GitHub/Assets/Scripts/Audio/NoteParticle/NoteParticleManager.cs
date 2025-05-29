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
    [HideInInspector] public float speed = 1;
    private float randScaleChangeTime;

    public void StartObj(GameObject newFollowObj, float newEndTime){
        followObj = newFollowObj;
        endTime = newEndTime;

        startTime = Time.time;
        lifeTime = endTime - startTime;

        targetPos = transform.position + (Random.insideUnitSphere * Random.Range(0,3));

        randScaleChangeTime = ((startTime + lifeTime) - (lifeTime/3)) - Random.Range(-0.2f,0.2f);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0,360), transform.eulerAngles.z);

        selfRb = GetComponent<Rigidbody>();
        selfRb.linearVelocity = (transform.position - targetPos).normalized * speed;

        selfCollider = GetComponent<Collider>();

        foreach(Collider collider in followObj.GetComponents<Collider>()) {
            followObjColliders.Add(collider);
            Physics.IgnoreCollision(selfCollider, collider, true);
        }
        StartCoroutine(ColliderCheck());
    }

    private IEnumerator ColliderCheck(){
        yield return null;
        
        bool touching = true;
        
        while(touching){
            touching = followObjColliders.Any(followObjCollider => AreTouching(followObjCollider, selfCollider));

            yield return null;
        }

        foreach(Collider followObjCollider in followObjColliders) {
            Physics.IgnoreCollision(selfCollider, followObjCollider, false);
        }
    }   

    void Update(){
        if(Time.time > endTime){
            Destroy(transform.gameObject);
        }

        if(randScaleChangeTime < Time.time){   
            transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.zero, ref velocity,0.1f);
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
}

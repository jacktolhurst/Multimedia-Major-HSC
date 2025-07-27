using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class NoteParticleManager : MonoBehaviour
{
    private Coroutine collideCheckCoroutine;

    private GameObject followObj;

    private Rigidbody selfRb;

    private List<Collider> followObjColliders = new List<Collider>();
    private Collider selfCollider;

    private Material particleMat;

    private Vector3 targetPos;
    private Vector3 scaleDownVelocity = Vector3.zero;
    private Vector3 scaleUpVelocity = Vector3.zero;
    private Vector3 defaultSize;

    private float startTime;
    private float lifeTime;
    public float endTime;
    public float currTime;
    private float randomPointSpeed = 1;
    private float scaleDownTime;
    private float scaleDuration = 0.5f;

    private bool scaledUp = false;
    private bool scaledDown = false;
    
    void Awake(){
        selfRb = GetComponent<Rigidbody>();
        
        selfCollider = GetComponent<Collider>();

        particleMat = GetComponent<MeshRenderer>().material;

        defaultSize = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    public void StartObj(GameObject newFollowObj, float newEndTime){ // used for if starting by given an object
        SetEndTime(newEndTime);

        followObj = newFollowObj;

        foreach(Collider followObjCollider in followObj.GetComponents<Collider>()) {
            followObjColliders.Add(followObjCollider);
            Physics.IgnoreCollision(selfCollider, followObjCollider, true);
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0,360), transform.eulerAngles.z);

        selfRb.position = RandomPointInCollider(followObj.GetComponents<Collider>());

        collideCheckCoroutine = StartCoroutine(ColliderCheck());
    }

    public void StartPosition(float newEndTime){ // used for if starting by given a position
        SetEndTime(newEndTime);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0,360), transform.eulerAngles.z);    

        targetPos = transform.position + (Random.insideUnitSphere*2) + (Vector3.up * 100);
    }

    private IEnumerator ColliderCheck(){
        while(!followObjColliders.Any(followObjCollider => AreTouching(selfCollider, followObjCollider))){
            yield return null;
        }

        foreach(Collider followObjCollider in followObjColliders){
            Physics.IgnoreCollision(selfCollider, followObjCollider, false);
        }

        targetPos = transform.position + (Random.insideUnitSphere*2) + (Vector3.up*100);
    }   

    void Update(){
        if(Time.time > endTime){
            DOTween.Kill(transform);
            Destroy(transform.gameObject);
            if (collideCheckCoroutine != null){
                StopCoroutine(collideCheckCoroutine);
            }
        }
        else{
            Vector3 dir = (targetPos - transform.position).normalized;
            selfRb.linearVelocity = dir * randomPointSpeed;

            if(Time.time > scaleDownTime){   
                if(!scaledUp){
                    transform.DOScale(Vector3.zero, scaleDuration);
                    scaledUp = true;
                }
            }
            else if(!IsVector3Close(defaultSize, transform.localScale, 0.1f)){
                if(!scaledDown){
                    transform.DOScale(defaultSize, scaleDuration);
                    scaledDown = true;
                }

                selfRb.linearVelocity = Vector3.zero;
                selfRb.angularVelocity = Vector3.zero;
            }

            particleMat.SetFloat("_TimeLeft", (float)(((Time.time - startTime) / (scaleDownTime - startTime))));
        }
    }

    public void SetEndTime(float newEndTime, bool isRandom=true){
        endTime = newEndTime + scaleDuration;

        startTime = Time.time;
        lifeTime = endTime - startTime;

        if(isRandom) scaleDownTime = (endTime-scaleDuration)+ Random.Range(-0.2f,0.2f);
        else scaleDownTime = endTime-scaleDuration;
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

    public float GetScaleDuration(){
        return scaleDuration;
    }

    private Vector3 RandomPointInCollider(Collider[] colliders, int maxTries = 30){
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

    private bool IsVector3Close(Vector3 vecA, Vector3 vecB, float distance) {
        return Vector3.Distance(vecA, vecB) < distance;
    }
}

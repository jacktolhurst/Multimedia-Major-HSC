using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoteParticleManager : MonoBehaviour
{
    [HideInInspector] public Rigidbody followObjRb;
    private Rigidbody selfRb;

    private Collider[] followObjColliders;
    private Collider selfCollider;

    private List<Bounds> followObjBounds = new List<Bounds>();
    private Bounds selfBound;

    private Vector3 targetPos;
    private Vector3 velocity = Vector3.zero;

    private float startTime;
    private float lifeTime;
    [HideInInspector] public float endTime;
    [HideInInspector] public float speed = 1;
    private float randScaleChangeTime;

    [HideInInspector] public bool usingGameObject = false;

    void Start(){
        StartCoroutine(AfterStart());
    }

    private IEnumerator AfterStart(){
        yield return null;

        startTime = Time.time;
        lifeTime = endTime - startTime;

        targetPos = transform.position + (Random.insideUnitSphere * Random.Range(0,3));

        randScaleChangeTime = ((startTime + lifeTime) - (lifeTime/3)) - Random.Range(-0.2f,0.2f);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0,360), transform.eulerAngles.z);

        selfRb = GetComponent<Rigidbody>();
        selfRb.linearVelocity = (transform.position - targetPos).normalized * speed;

        if(usingGameObject){
            selfCollider = GetComponent<Collider>();
            selfCollider.isTrigger = false;
            followObjColliders = followObjRb.gameObject.GetComponents<Collider>();

            foreach(Collider followObjCollider in followObjColliders) followObjBounds.Add(followObjCollider.bounds);

            selfBound = GetComponent<Collider>().bounds;

            CheckIfColliding();
        }

        // Bounds bounds = obj.GetComponent<Collider>().bounds;
        // Vector3 extents = bounds.extents;
        // List<GameObject> newNoteParticleObjs = new List<GameObject>();

        // Vector3 spawnPoint = Vector3.zero;
        // if(bounds != null){
            // Vector3 dir = Random.onUnitSphere;
            // float requiredDistance =  (Mathf.Abs(dir.x) * extents.x) + (Mathf.Abs(dir.y) * extents.y) + (Mathf.Abs(dir.z) * extents.z);
            // float spawnDistance = requiredDistance + 1;
            // spawnPoint = bounds.center + dir * spawnDistance;
        // }
        // else spawnPoint = obj.transform.position;
    }

    void Update(){
        if(Time.time > endTime){
            Destroy(transform.gameObject);
        }

        if(randScaleChangeTime < Time.time){   
            transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.zero, ref velocity,0.1f);
        }

        if(usingGameObject) CheckIfColliding();
    }

    private void CheckIfColliding(){
        foreach(Bounds followObjBound in followObjBounds){
            if(selfBound.Intersects(followObjBound)){
                foreach(Collider followObjCollider in followObjColliders) Physics.IgnoreCollision(selfCollider, followObjCollider, true);
            }
            else{
                foreach(Collider followObjCollider in followObjColliders) Physics.IgnoreCollision(selfCollider, followObjCollider, false);
            }
        }
    }
}

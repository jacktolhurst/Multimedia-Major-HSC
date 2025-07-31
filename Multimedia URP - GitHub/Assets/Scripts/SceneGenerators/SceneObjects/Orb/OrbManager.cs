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
    [SerializeField] private float baseDeletionSize;
    [SerializeField] private float orbForce;
    [SerializeField] private float sizeSpeed;

    private int lastDeletedObjsSize;

    void Awake(){
        selfBounds = GetBoundsFromObj(transform.gameObject);
        baseSize = selfBounds.size;
    }

    void Update(){
        selfBounds = GetBoundsFromObj(transform.gameObject);

        ManageRigidbodies();
        ManageScaling();
    }

    private void ManageRigidbodies(){
        ApplyForceToRigidbodies(GetRigidbodiesInRange(GetCheckSize(selfBounds.extents, baseForceSize)), orbForce);
        ApplyDeletionToRigidbodies(GetRigidbodiesInRange(GetCheckSize(selfBounds.extents, baseDeletionSize)));
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

            float distanceScaledForce = Mathf.Min(baseForce/distance, 1000);

            body.AddForce(direction * distanceScaledForce, ForceMode.Acceleration);

            Collider[] bodyColliders = body.gameObject.GetComponents<Collider>();
            foreach(Collider collider in bodyColliders){
                collider.material.dynamicFriction = 0;
                collider.material.staticFriction = 0;
                collider.material.frictionCombine = PhysicsMaterialCombine.Minimum;
            }
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

    private float GetCheckSize(Vector3 givenScale, float baseSize=0){
        float average = (givenScale.x + givenScale.y + givenScale.z)/3;
        return average + baseSize;
    }

    private float ApplyOrbRatio(Bounds bounds, float originalScale){
        return bounds.size.magnitude / originalScale;
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
        float velocity = objRigidbody.linearVelocity.magnitude;
        if(velocity < 2) velocity = 5;
        float distance = Vector3.Distance(objTransform.position, transform.position);
        

        objTransform.DOScale(Vector3.zero, scaleDuration);
        objTransform.DOMove(transform.position, distance/velocity);
        
        while(Time.time < deletionTime){
            yield return null;
        }

        DOTween.Kill(objTransform);
        Destroy(obj);
    }

    void OnDrawGizmos(){
        if(Application.isPlaying){
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, GetCheckSize(selfBounds.extents, baseForceSize));
            Gizmos.DrawWireSphere(transform.position, GetCheckSize(selfBounds.extents, baseDeletionSize));
        }
    }
}

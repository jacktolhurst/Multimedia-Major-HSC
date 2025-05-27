using UnityEngine;

public class NoteParticleManager : MonoBehaviour
{
    [HideInInspector] public Rigidbody followObjRb = null;

    private Vector3 targetPos;
    [HideInInspector] public Vector3 offset = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    private float startTime;
    private float lifeTime;
    [HideInInspector] public float endTime;
    [HideInInspector] public float speed = 1;
    private float randScaleChangeTime;

    void Start(){
        startTime = Time.time;
        lifeTime = endTime - startTime;

        targetPos = transform.position + (Random.insideUnitSphere * Random.Range(0,3));

        randScaleChangeTime = ((startTime + lifeTime) - (lifeTime/3)) - Random.Range(-0.2f,0.2f);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0,360), transform.eulerAngles.z);
    }

    void Update(){
        if(Time.time > endTime){
            Destroy(transform.gameObject);
        }
        transform.position = Vector3.Lerp(transform.position, targetPos + offset, speed * Time.deltaTime);

        if(randScaleChangeTime < Time.time){   
            transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.zero, ref velocity,0.1f);
        }
        
        if(followObjRb != null && startTime + 0.1f > Time.time){
            offset = followObjRb.linearVelocity/2;
        }
    }
}

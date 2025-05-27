using UnityEngine;

public class NoteParticleManager : MonoBehaviour
{
    private Vector3 targetPos;
    private Vector3 velocity = Vector3.zero;

    private float startTime;
    private float lifeTime;
    [HideInInspector] public float endTime;
    [HideInInspector] public float speed = 1;

    void Start(){
        startTime = Time.time;
        lifeTime = endTime - startTime;

        targetPos = transform.position + (Random.insideUnitSphere * Random.Range(0,3));
    }

    void Update(){
        if(Time.time > endTime){
            Destroy(transform.gameObject);
        }
        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);

        if((startTime + lifeTime) - (lifeTime/3) < Time.time){   
            transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.zero, ref velocity,0.1f);
        }
    }
}

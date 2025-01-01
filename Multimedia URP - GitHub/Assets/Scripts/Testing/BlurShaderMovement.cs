using UnityEngine;
using System.Collections;

public class BlurShaderMovement : MonoBehaviour
{
    private GameObject child;

    private Vector3 lastPosition;

    [SerializeField] private float smoothing;
    [SerializeField] private float waitSeconds;
    private float lastWaitSeconds;


    void Awake(){
        child = transform.gameObject.transform.GetChild(0).gameObject;

        StartCoroutine(Interval());
    }

    void Update(){
        lastPosition = Vector3.Lerp( lastPosition, transform.position, Time.deltaTime * smoothing);
        child.transform.position = lastPosition;

        if(waitSeconds != lastWaitSeconds){
            StopCoroutine(Interval());
            StartCoroutine(Interval());
        }
    }

    void LateUpdate(){
        lastWaitSeconds = waitSeconds;
    }

    private IEnumerator Interval(){
        while(true){
            lastPosition = transform.position;
            if(waitSeconds == 0){
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(waitSeconds);
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelSpinner : MonoBehaviour
{
    [System.Serializable]
    public class SpinnerClass{
        public GameObject obj;
        public float speed;

        [HideInInspector] public float nextPosition;
    }

    [SerializeField] private List<SpinnerClass> spinners = new List<SpinnerClass>();

    [SerializeField] private float rotateDist;

    void Awake(){
        foreach(SpinnerClass spinner in spinners){ 
            StartCoroutine(Spin(spinner.obj.transform, spinner.speed));
        }
    }

    private IEnumerator Spin(Transform objTrans, float speed){ 
        while(true){ 
            float projectedY = Random.Range(0f, 360f);
            float randTimes = Random.Range(-1,2);

            if(randTimes == 0){
                randTimes = 1;
            }

            while(!IsClose(objTrans.eulerAngles.y, projectedY, rotateDist)){
                objTrans.Rotate(0, 0, Mathf.Lerp(objTrans.rotation.y, projectedY, speed * Time.deltaTime) * randTimes * Random.Range(0,2));
                yield return null;
            }
            yield return new WaitForSeconds(Random.Range(0,4));
        }
    }

    private bool IsClose(float a, float b, float dist){
        float distance = Mathf.Abs(a-b);
        if(distance < dist){
            return true;
        }
        else{
            return false;
        }
    }
}

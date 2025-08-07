using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class LeverManager : MonoBehaviour
{  
    public enum Difference{
        less,
        equal,
        more
    }

    [System.Serializable]
    private class Lever{
        public Difference difference;

        public GameObject obj;

        [HideInInspector] public Transform trans;

        [HideInInspector] public Rigidbody rb;

        [HideInInspector] public Vector3 lastEulerAngle;

        public bool isActive; 
        [HideInInspector] public bool isRotating;
    }

    [SerializeField] private List<Lever> leverObjs;

    void Awake(){
        foreach(Lever lever in leverObjs){
            lever.trans = lever.obj.transform;

            lever.rb = lever.obj.GetComponent<Rigidbody>();

            lever.lastEulerAngle = lever.trans.eulerAngles;

            switch (lever.difference){
                case Difference.less:
                    StartCoroutine(ManageLeverLess(lever));
                    break;

                case Difference.equal:
                    StartCoroutine(ManageLeverEqual(lever));
                    break;

                case Difference.more:
                    StartCoroutine(ManageLeverMore(lever));
                    break;

                default:
                    break;
            }
        }
    }

    void Update(){
        foreach(Lever lever in leverObjs) print(lever.isRotating);
    }

    private IEnumerator ManageLeverLess(Lever lever){
        yield return null;
        while(lever.isActive){
            if(lever.lastEulerAngle.y > lever.trans.eulerAngles.y) lever.isRotating = true;
            else lever.isRotating = false;

            lever.lastEulerAngle = lever.trans.eulerAngles;

            yield return null;
        }
    }

    private IEnumerator ManageLeverEqual(Lever lever){
        yield return null;
        while(lever.isActive){
            if(lever.lastEulerAngle.y == lever.trans.eulerAngles.y) lever.isRotating = true;
            else lever.isRotating = false;

            lever.lastEulerAngle = lever.trans.eulerAngles;

            yield return null;
        }
    }

    private IEnumerator ManageLeverMore(Lever lever){
        yield return null;
        while(lever.isActive){
            if(lever.lastEulerAngle.y < lever.trans.eulerAngles.y) lever.isRotating = true;
            else lever.isRotating = false;

            lever.lastEulerAngle = lever.trans.eulerAngles;
            
            yield return null;
        }
    }

    // void Update(){
    //     if(leverTrans.eulerAngles.y < lastEulerAngle.y) print("rotating correct!");
    //     else print("Not rotating");
    // }

    // void LateUpdate(){
    //     lastEulerAngle = leverTrans.eulerAngles;
    // }
}
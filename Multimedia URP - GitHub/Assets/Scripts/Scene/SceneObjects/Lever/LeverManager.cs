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
    public class Lever{
        public string name;

        public Difference difference;

        public GameObject obj;

        [HideInInspector] public Transform trans;

        [HideInInspector] public Rigidbody rb;

        [HideInInspector] public Vector3 lastEulerAngle;

        public float minimumDifference;

        public bool isActive; 
        [HideInInspector] public bool isRotating;
    }

    public List<Lever> leverObjs;

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

    private IEnumerator ManageLeverLess(Lever lever){
        yield return null;
        while(lever.isActive){
            float difference = lever.lastEulerAngle.y - lever.trans.eulerAngles.y;
            if(difference > lever.minimumDifference) lever.isRotating = true;
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
            float difference = lever.lastEulerAngle.y - lever.trans.eulerAngles.y;
            if(difference < lever.minimumDifference) lever.isRotating = true;
            else lever.isRotating = false;

            lever.lastEulerAngle = lever.trans.eulerAngles;
            
            yield return null;
        }
    }

    public Lever GetLeverByName(string name){
        foreach(Lever lever in leverObjs){
            if(lever.name.ToLower() == name.ToLower()){
                return lever;
            }
        }
        return null;
    }
}
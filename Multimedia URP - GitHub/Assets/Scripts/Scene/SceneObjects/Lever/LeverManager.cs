using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeverManager : MonoBehaviour
{
    public enum Difference {
        less,
        equal,
        more
    }

    public enum Axis {
        X,
        Y,
        Z,
        All
    }

    [System.Serializable]
    public class Lever {
        public string name;
        public Difference difference;
        public GameObject obj;

        public Axis checkAxis = Axis.Y;         
        public float minimumDifference = 1f;    
        public float tolerance = 0.05f;        

        [HideInInspector] public Transform trans;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public Quaternion lastRotation;
        [HideInInspector] public Vector3 lastEulerAngle;
        [HideInInspector] public float totalRotation = 0f; // Track total rotation
        [HideInInspector] public bool hasCompletedFullRotation = false; // Flag for 360° completion

        public bool isActive = true;
        [HideInInspector] public bool isRotating;
    }

    public List<Lever> leverObjs;

    void Awake() {
        foreach (Lever lever in leverObjs) {
            if (lever.obj == null) continue;

            lever.trans = lever.obj.transform;
            lever.rb = lever.obj.GetComponent<Rigidbody>();
            lever.lastRotation = lever.trans.rotation;
            lever.lastEulerAngle = lever.trans.eulerAngles;
            lever.totalRotation = 0f;
            lever.hasCompletedFullRotation = false;

            switch (lever.difference) {
                case Difference.less:
                    StartCoroutine(ManageLeverLess(lever));
                    break;
                case Difference.equal:
                    StartCoroutine(ManageLeverEqual(lever));
                    break;
                case Difference.more:
                    StartCoroutine(ManageLeverMore(lever));
                    break;
            }
        }
    }

    private float ComputeAngleDifference(Lever lever) {
        if (lever.checkAxis == Axis.All) {
            return Quaternion.Angle(lever.lastRotation, lever.trans.rotation);
        } else {
            float last = 0f;
            float curr = 0f;
            switch (lever.checkAxis) {
                case Axis.X:
                    last = lever.lastEulerAngle.x;
                    curr = lever.trans.eulerAngles.x;
                    break;
                case Axis.Y:
                    last = lever.lastEulerAngle.y;
                    curr = lever.trans.eulerAngles.y;
                    break;
                case Axis.Z:
                    last = lever.lastEulerAngle.z;
                    curr = lever.trans.eulerAngles.z;
                    break;
            }
            return Mathf.DeltaAngle(last, curr); // Remove Abs to preserve direction
        }
    }

    private void UpdateTotalRotation(Lever lever) {
        float angleDiff = ComputeAngleDifference(lever);
        lever.totalRotation += Mathf.Abs(angleDiff);
        
        // Check if we've completed a full 360° rotation
        if (lever.totalRotation >= 360f) {
            lever.hasCompletedFullRotation = true;
            lever.totalRotation = lever.totalRotation % 360f; // Reset for next cycle
        }
    }

    private bool CheckFullRotationAndReset(Lever lever) {
        if (lever.hasCompletedFullRotation) {
            lever.hasCompletedFullRotation = false;
            return true;
        }
        return false;
    }

    private IEnumerator ManageLeverLess(Lever lever) {
        yield return null;

        while (lever.isActive && lever.obj != null) {
            if(lever.obj.layer == 12){
                UpdateTotalRotation(lever);
                
                // Return true every 360 degrees
                lever.isRotating = CheckFullRotationAndReset(lever);
            }

            lever.lastRotation = lever.trans.rotation;
            lever.lastEulerAngle = lever.trans.eulerAngles;

            yield return null;
        }
    }

    private IEnumerator ManageLeverEqual(Lever lever) {
        yield return null;
        while (lever.isActive && lever.obj != null) {
            if(lever.obj.layer == 12){
                UpdateTotalRotation(lever);
                
                // Return true every 360 degrees
                lever.isRotating = CheckFullRotationAndReset(lever);
            }

            lever.lastRotation = lever.trans.rotation;
            lever.lastEulerAngle = lever.trans.eulerAngles;

            yield return null;
        }
    }

    private IEnumerator ManageLeverMore(Lever lever) {
        yield return null;
        while (lever.isActive && lever.obj != null) {
            if(lever.obj.layer == 12){
                UpdateTotalRotation(lever);
                
                // Return true every 360 degrees
                lever.isRotating = CheckFullRotationAndReset(lever);
            }

            lever.lastRotation = lever.trans.rotation;
            lever.lastEulerAngle = lever.trans.eulerAngles;

            yield return null;
        }
    }

    public Lever GetLeverByName(string name) {
        foreach (Lever lever in leverObjs) {
            if (lever.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return lever;
        }
        Debug.LogWarning("There is no lever with name: " + name);
        return null;
    }

    public Lever GetLeverAtIndex(int index){
        index -= 1;
        if(index <= leverObjs.Count) return leverObjs[index];
        else return null;
    }

    // Optional: Method to manually reset a lever's rotation tracking
    public void ResetLeverRotation(string leverName) {
        Lever lever = GetLeverByName(leverName);
        if (lever != null) {
            lever.totalRotation = 0f;
            lever.hasCompletedFullRotation = false;
        }
    }
}
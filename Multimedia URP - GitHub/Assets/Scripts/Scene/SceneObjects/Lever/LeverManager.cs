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
            return Mathf.Abs(Mathf.DeltaAngle(last, curr));
        }
    }

    private IEnumerator ManageLeverLess(Lever lever) {
        yield return null;
        while (lever.isActive && lever.obj != null) {
            float angleDiff = ComputeAngleDifference(lever);

            lever.isRotating = (angleDiff > lever.tolerance) && (angleDiff < lever.minimumDifference);

            lever.lastRotation = lever.trans.rotation;
            lever.lastEulerAngle = lever.trans.eulerAngles;

            yield return null;
        }
    }

    private IEnumerator ManageLeverEqual(Lever lever) {
        yield return null;
        while (lever.isActive && lever.obj != null) {
            float angleDiff = ComputeAngleDifference(lever);

            lever.isRotating = !(angleDiff <= lever.tolerance);

            lever.lastRotation = lever.trans.rotation;
            lever.lastEulerAngle = lever.trans.eulerAngles;

            yield return null;
        }
    }

    private IEnumerator ManageLeverMore(Lever lever) {
        yield return null;
        while (lever.isActive && lever.obj != null) {
            float angleDiff = ComputeAngleDifference(lever);

            lever.isRotating = angleDiff > lever.minimumDifference;

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
}

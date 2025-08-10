using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    public void Lock(float lockAmount = 10000000, float damperAmount = 0, float massScale = 100){
        ChangeHinge(lockAmount, damperAmount, massScale);
    }

    public void Unlock(Vector3 unlockForce, float lockAmount = 1, float damperAmount = 0, float massScale = 1){
        ChangeHinge(lockAmount, damperAmount, massScale);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = unlockForce;
    }

    void ChangeHinge(float springAmount, float damperAmount, float massScale){
        HingeJoint hinge = GetComponent<HingeJoint>();
        hinge.connectedMassScale = massScale;
        JointSpring spring = hinge.spring;
        spring.spring = springAmount;
        spring.damper = damperAmount;
        hinge.spring = spring;
    }
}

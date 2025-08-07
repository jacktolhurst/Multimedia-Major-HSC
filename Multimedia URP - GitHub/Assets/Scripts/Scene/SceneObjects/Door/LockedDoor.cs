using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    public void Lock(float lockAmount = 10000000, float damperAmount = 0){
        ChangeHinge(lockAmount, damperAmount);
    }

    public void Unlock(Vector3 unlockForce, float lockAmount = 1, float damperAmount = 0){
        ChangeHinge(lockAmount, damperAmount);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = unlockForce;
    }

    void ChangeHinge(float springAmount, float damperAmount = 0){
        HingeJoint hinge = GetComponent<HingeJoint>();
        JointSpring spring = hinge.spring;
        spring.spring = springAmount;
        spring.damper = damperAmount;
        hinge.spring = spring;
    }
}

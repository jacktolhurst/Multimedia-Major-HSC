using UnityEngine;
using System.Collections.Generic;

[SelectionBase]
public class MicrophoneScript : MonoBehaviour
{
    private SoundDetection SoundDetectionScript;

    private List<Rigidbody> childrenRigidbodies = new List<Rigidbody>();


    private bool hasCollided = false;

    void Awake(){
        foreach(Transform child in transform){
            Rigidbody childRb = child.gameObject.GetComponent<Rigidbody>();
            if(childRb != null) childrenRigidbodies.Add(child.gameObject.GetComponent<Rigidbody>());
        }

        SoundDetectionScript = GetComponent<SoundDetection>();
    }

    void Update(){
        if(!hasCollided){
            foreach(Rigidbody rb in childrenRigidbodies){
                if(rb.isKinematic == false){
                    foreach(Rigidbody changeRb in childrenRigidbodies){
                        changeRb.isKinematic = false;
                    }
                    hasCollided = true;
                    SoundDetectionScript.checkSounds = false;
                    break;
                }
            }
        }
    }
}

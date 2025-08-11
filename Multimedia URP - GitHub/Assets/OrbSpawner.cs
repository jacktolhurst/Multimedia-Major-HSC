using UnityEngine;
using System.Collections.Generic;

public class OrbSpawner : MonoBehaviour
{
    void Awake(){
        foreach (KeyValuePair<GameObject, Vector3> pair in OrbData.objs) {
            GameObject obj = pair.Key;
            Vector3 size = pair.Value;
            obj.transform.position = transform.position;
            obj.transform.rotation = Random.rotation;
            obj.transform.localScale = size*2;
            obj.SetActive(true);

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.linearDamping = 0;
            rb.angularDamping = 0;
            rb.isKinematic = false;
            rb.WakeUp();

            foreach(Collider coll in obj.GetComponents<Collider>()){
                coll.material = null;
            }
        }
    }
}

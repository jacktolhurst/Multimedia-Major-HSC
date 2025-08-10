using UnityEngine;
using System.Collections.Generic;

[SelectionBase]
public class ObjectHolderScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> objs;

    private List<Rigidbody> objRbs = new List<Rigidbody>();

    private bool isStatic = true;

    void Awake(){
        foreach(GameObject obj in objs) objRbs.Add(obj.GetComponent<Rigidbody>());

        UpdateRbs(objRbs, true);
    }

    void Update(){
        if(isStatic){
            List<Rigidbody> rbToRemove = new List<Rigidbody>();
            foreach(Rigidbody rb in objRbs){
                if(rb == null) rbToRemove.Add(rb);
                else if(!rb.isKinematic){
                    UpdateRbs(objRbs, false);
                    isStatic = false;
                    break;
                }
            }
            foreach(Rigidbody rb in rbToRemove) objRbs.Remove(rb);
        }
    }

    private void UpdateRbs(List<Rigidbody> rbs, bool isKinematic){
        foreach(Rigidbody rb in rbs){
            rb.isKinematic = isKinematic;
        }
    }

}

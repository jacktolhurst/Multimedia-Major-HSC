using UnityEngine;
using System.Collections.Generic;

public class TurnLowPolyScript : MonoBehaviour
{
    public void TurnLowPoly(){
        GameObject found = new List<GameObject>(GameObject.FindGameObjectsWithTag("Cubicle")).Find(g => g.transform.IsChildOf( this.transform));
        foreach(Transform child in transform){
            if(found.transform != child){
                child.gameObject.SetActive(false);
            }
        }
    }
    public void TurnOffLowPoly(){
        foreach(Transform child in transform){
            child.gameObject.SetActive(true);
        }
    }
}

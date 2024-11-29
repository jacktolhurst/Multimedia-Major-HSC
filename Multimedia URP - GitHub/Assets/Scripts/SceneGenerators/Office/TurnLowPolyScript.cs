using UnityEngine;
using System.Collections.Generic;

public class TurnLowPolyScript : MonoBehaviour
{
    public void TurnLowPoly(){
        List<GameObject> foundObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("StayInScene")).FindAll(g => g.transform.IsChildOf(this.transform));
        foreach(Transform child in transform){
            if(!foundObjects.Contains(child.gameObject)){
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

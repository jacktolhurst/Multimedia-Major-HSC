using UnityEngine;
using System.Collections.Generic;

public class TurnLowPolyScript : MonoBehaviour
{
    public void TurnLowPoly(){
        foreach(Transform child in transform){
            if(child.gameObject.tag != "StayInScene"){
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

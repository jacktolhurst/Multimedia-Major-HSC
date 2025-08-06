using UnityEngine;
using System.Collections.Generic;

public class FrontDoorLOD : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects = new List<GameObject>();

    private Vector3 startPos;

    private bool showObjs = false;
    [SerializeField] private bool doLOD;

    void Awake(){
        startPos = transform.position;

        if(doLOD){
            foreach(var obj in objects){
                obj.SetActive(false);
            }
        }
    }

    void Update(){
        if(!showObjs && transform.position != startPos){
            showObjs = true;
            foreach(var obj in objects){
                obj.SetActive(true);
            }
        }
    }
}

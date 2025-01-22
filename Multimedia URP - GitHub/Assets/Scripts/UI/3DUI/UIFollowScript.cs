using UnityEngine;
using System.Collections.Generic; 

public class UIFollowScript : MonoBehaviour
{
    [System.Serializable]
    private class UIElementsClass{
        public GameObject objType;
        public Quaternion rotation;
        public Vector3 offsetPos;
    }

    [SerializeField] private List<UIElementsClass> UIElements = new List<UIElementsClass>();

    [SerializeField] private Transform cameraPos;

    void Start(){
        GameObject UIParent = new GameObject("UIParent");
        foreach(UIElementsClass element in UIElements){
            GameObject generatedUI;
            generatedUI = Instantiate(element.objType, cameraPos.position + element.offsetPos, element.rotation);
            generatedUI.transform.parent = UIParent.transform;
        }
    }

    //TODO: in the class add a section for the position of the object, then iterate thru them in the update so they can lerp betweeen their position and the recalculated pos

    void Update(){
    }
}

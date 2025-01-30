using UnityEngine;
using System.Collections.Generic; 

public class UIFollowScript : MonoBehaviour
{
    [System.Serializable]
    public class UIElementsClass{
        public enum HorizontalAlignment{
            Left,
            Centre,
            Right
        }
        public enum VerticalAlignment{
            Top,
            Centre,
            Bottom
        }
        public HorizontalAlignment horizontalAlign;
        public VerticalAlignment verticalAlign;

        public GameObject objType;
        public Vector3 baseRotation;
        public Vector2 pixelPadding;
        public float scaleMultiplier;
        public float zDistance;
        public float smoothing;

        [HideInInspector] public GameObject realObj;
        [HideInInspector] public Vector3 realPos;
        [HideInInspector] public Vector3 velocity;
        [HideInInspector] public Vector3 originalScale;
        [HideInInspector] public Vector2 screenCoordinate;
    }

    public List<UIElementsClass> UIElements = new List<UIElementsClass>();

    private FirstPersonCamera firstPersonCameraScript;

    public Camera mainCam;

    private Quaternion camRotation;

    private float FOV;
    private float baseFOV;

    [SerializeField] private int UILayer;

    void Awake(){
        GameObject UIParent = new GameObject("UIParent");
        foreach(UIElementsClass element in UIElements){
            element.screenCoordinate = CheckCordinates(element.horizontalAlign, element.verticalAlign);

            element.realObj = Instantiate(element.objType, mainCam.ScreenToWorldPoint(new Vector3(element.screenCoordinate.x, element.screenCoordinate.y, element.zDistance)), Quaternion.identity);
            element.realPos = element.realObj.transform.position;
            element.originalScale = element.realObj.transform.localScale;
            element.realObj.transform.localScale = element.originalScale * ((element.scaleMultiplier) * new Vector2(Screen.width, Screen.height).magnitude);
            element.realObj.transform.parent = UIParent.transform;
            element.realObj.layer = UILayer;
        }

        FirstPersonCamera[] firstPersonCameraScripts = FindObjectsByType<FirstPersonCamera>(FindObjectsSortMode.None);
        if(firstPersonCameraScripts.Length > 0){
            firstPersonCameraScript = firstPersonCameraScripts[0];
        }
        else{
            Debug.LogError("No FirstPersonCamera component found in the scene");
        }

        baseFOV = firstPersonCameraScript.standardFov;
    }

    void LateUpdate(){
        camRotation = Quaternion.Euler(mainCam.transform.eulerAngles.x, mainCam.transform.eulerAngles.y, 0);

        FOV = mainCam.fieldOfView;

        foreach (UIElementsClass element in UIElements)
        {
            element.screenCoordinate = CheckCordinates(element.horizontalAlign, element.verticalAlign);

            Vector3 targetPos = mainCam.ScreenToWorldPoint(new Vector3(Mathf.Lerp(element.screenCoordinate.x, Screen.width / 2, (element.pixelPadding.x / 10)), Mathf.Lerp(element.screenCoordinate.y, Screen.height / 2, element.pixelPadding.y / 10), element.zDistance));

            element.realPos = Vector3.SmoothDamp(element.realPos, targetPos, ref element.velocity, element.smoothing);

            element.realObj.transform.position = element.realPos;

            Quaternion newBaseRotation = Quaternion.Euler(element.baseRotation.x, element.baseRotation.y, element.baseRotation.z);
            element.realObj.transform.rotation = camRotation * newBaseRotation;

            float fovScale = Mathf.Tan(FOV * 0.5f * Mathf.Deg2Rad) / Mathf.Tan(baseFOV * 0.5f * Mathf.Deg2Rad);
            element.realObj.transform.localScale = element.originalScale * (element.scaleMultiplier/10) * fovScale;
        }
    }

    private Vector2 CheckCordinates(UIElementsClass.HorizontalAlignment horizontalAlignment, UIElementsClass.VerticalAlignment verticalAlignment){
        Vector2 value = Vector2.zero;

        if(horizontalAlignment == UIElementsClass.HorizontalAlignment.Left){
            value.x = 0;
        }
        else if(horizontalAlignment == UIElementsClass.HorizontalAlignment.Centre){
            value.x = Screen.width/2;
        }
        else if(horizontalAlignment == UIElementsClass.HorizontalAlignment.Right){
            value.x = Screen.width;
        }

        if(verticalAlignment == UIElementsClass.VerticalAlignment.Top){
            value.y = Screen.height;
        }
        else if(verticalAlignment == UIElementsClass.VerticalAlignment.Centre){
            value.y = Screen.height/2;
        }
        else if(verticalAlignment == UIElementsClass.VerticalAlignment.Bottom){
            value.y = 0;
        }

        return value;
    }
}

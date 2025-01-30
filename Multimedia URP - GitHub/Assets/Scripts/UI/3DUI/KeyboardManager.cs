using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using TMPro;

public class KeyboardManager : MonoBehaviour
{
    private GrabObjects grabObjects;
    private UIFollowScript UIFollowScript;

    private Dictionary<string, GameObject> textObjs = new Dictionary<string, GameObject>();

    private GameObject UICanvas;
    private UIFollowScript.UIElementsClass keyboard;

    private Camera mainCam;
    
    private Renderer objRenderer;

    private Vector3 currentBounds;

    private Quaternion camRotation;

    void Awake(){
        UICanvas = GameObject.Find("UI(Graphy)");

        UIFollowScript[] followScripts = FindObjectsByType<UIFollowScript>(FindObjectsSortMode.None);
        if(followScripts.Length > 0){
            UIFollowScript = followScripts[0];
        }
        else{
            Debug.LogError("No UIFollowScript component found in the scene");
        }

        mainCam = UIFollowScript.mainCam;
    }

    void Start(){
        objRenderer = GetComponent<Renderer>();

        GrabObjects[] objects = FindObjectsByType<GrabObjects>(FindObjectsSortMode.None);
        if(objects.Length > 0){
            grabObjects = objects[0];
        }
        else{
            Debug.LogError("No GrabObjects component found in the scene");
        }

        keyboard = UIFollowScript.UIElements[0];
    }

    void Update(){
        if(grabObjects.canGrab == true){
            // GenerateText("LeftClickText");
        }
        else{
            DestroyText("LeftClickText");
        }
    }

    void LateUpdate(){
        UpdateValues();
    }

    private void GenerateText(string name){
        if (!textObjs.ContainsKey(name)){
            GameObject generatedText = new GameObject(name);
            generatedText.transform.SetParent(UICanvas.transform);

            generatedText.transform.position = new Vector3(keyboard.realPos.x, objRenderer.bounds.min.y + objRenderer.bounds.size.y, keyboard.realPos.z);
            generatedText.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            TMP_Text textMeshPro = generatedText.AddComponent<TextMeshPro>();
            textMeshPro.fontSize = 36;
            textMeshPro.text = "Hello world";
            textMeshPro.color = new Color(0f, 0f, 0f, 0f);

            textObjs.Add(name, generatedText);
        }
    }

    private void DestroyText(string name){
        if(textObjs.ContainsKey(name)){
            Destroy(textObjs[name]);
            textObjs.Remove(name);
        }
    }

    private void UpdateValues(){
        camRotation = Quaternion.Euler(mainCam.transform.eulerAngles.x, mainCam.transform.eulerAngles.y, 0);

        foreach(KeyValuePair<string, GameObject> pair in textObjs){
            Vector3 screenPosition = mainCam.WorldToScreenPoint(keyboard.realPos + new Vector3(0f, 1f, 0f));

            Vector3 worldPosition = mainCam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Vector3.Distance(keyboard.realPos, mainCam.transform.position)));
    
            pair.Value.transform.position = worldPosition;
            pair.Value.transform.rotation = camRotation;
            
            pair.Value.GetComponent<TextMeshPro>().color = Color.Lerp(pair.Value.GetComponent<TextMeshPro>().color, new Color(0f,0f,0f,1f), 6f * Time.deltaTime);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class KeyBoardHandler : MonoBehaviour
{
    [System.Serializable]
    public class KeyboardNote{
        public string text;

        private GameObject obj;
        private GameObject textObj;

        private Material originalMat;
        private Material selectedMat;

        private Vector3 basePos;
        public Vector3 textOffset;

        float textScale;
        [Range(1,10)]
        public int index;

        public bool isSelected;
        
        public void Awake(GameObject mainObj, Material newMat, float textScale){
            obj = IndexToNote(index, mainObj);
            Material objMat = obj.GetComponent<MeshRenderer>().material;
            originalMat = objMat;
            selectedMat = newMat;
            
            textObj = SpawnTextElement(obj, text, textScale);
        }
        

        public void Update(){
            if(isSelected) {
                textObj.SetActive(true);

                obj.GetComponent<MeshRenderer>().material = selectedMat;
            }
            else{
                textObj.SetActive(false);
                obj.GetComponent<MeshRenderer>().material = originalMat;
            }

            textObj.transform.position = basePos + textOffset;
        }

        public GameObject SpawnTextElement(GameObject givenObj, string givenText, float scale){
            GameObject spawnedTextObj = new GameObject("KeyboardNoteText " + index);
            
            spawnedTextObj.transform.SetParent(givenObj.transform);

            Vector3 scaleVector = Vector3.one * scale;
            scaleVector.x = scaleVector.x / 4;
            spawnedTextObj.transform.localScale = scaleVector;

            spawnedTextObj.transform.position = givenObj.transform.position;

            spawnedTextObj.transform.localRotation = Quaternion.Euler(0, 180, 90);

            TextMeshPro textMeshPro = spawnedTextObj.AddComponent<TextMeshPro>();
            textMeshPro.text = givenText;
            textMeshPro.fontSize = 4;
            textMeshPro.color = Color.black;
            textMeshPro.alignment = TextAlignmentOptions.Center;

            textMeshPro.ForceMeshUpdate();

            Bounds textBounds = textMeshPro.bounds;
            Vector3 size = textBounds.size;

            spawnedTextObj.transform.position += new Vector3(size.x,0,0);
            basePos = spawnedTextObj.transform.position;

            spawnedTextObj.layer = 5;

            Renderer textRenderer = spawnedTextObj.GetComponent<Renderer>();
            textRenderer.renderingLayerMask = 0u;

            return spawnedTextObj;
        }

        
        public GameObject IndexToNote(int index, GameObject mainObj){
            GameObject chosenObj = null;

            foreach(Transform child in mainObj.transform){
                if(child.gameObject.name.ToLower().Contains("mainkeys")){
                    int currIndex = 1;
                    foreach(Transform innerChild in child){
                        if(index == currIndex) {
                            chosenObj = innerChild.gameObject;
                        }
                        currIndex ++;
                    }
                }
            }
            return chosenObj;
        }
    }
    
    [SerializeField] private List<KeyboardNote> keyboardNotes;
    
    [SerializeField] private Material selectedMat;

    [SerializeField] private float textScale;
    
    void Awake(){
        foreach(KeyboardNote keyboardNote in keyboardNotes){
            keyboardNote.Awake(gameObject, selectedMat, textScale);
        }
    }
    
    void Update(){
        foreach(KeyboardNote keyboardNote in keyboardNotes){
            keyboardNote.Update();
        }
    }
}
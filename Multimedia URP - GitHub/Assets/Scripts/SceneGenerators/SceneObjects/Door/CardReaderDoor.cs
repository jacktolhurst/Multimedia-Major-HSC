using UnityEngine;
using System.Collections.Generic; 

public class CardReaderDoor : MonoBehaviour
{
    [System.Serializable]
    public class ReaderClass{
        public GameObject obj;
        public Collider collider;

        [HideInInspector] public List<Material> readerMat = new List<Material>();
        [HideInInspector] public Bounds bounds;
    }

    [SerializeField] private List<ReaderClass> readers = new List<ReaderClass>();

    private LockedDoor lockedDoor;

    [SerializeField] private GameObject key;

    private List<Material> readerMat = new List<Material>();

    public bool unlocked = false;

    void Awake(){
        lockedDoor = GetComponent<LockedDoor>();

        foreach(ReaderClass reader in readers){
            reader.readerMat = new List<Material>(reader.obj.GetComponent<Renderer>().materials);

            reader.bounds = reader.collider.bounds;
        }
    }

    void Update(){
        foreach(ReaderClass reader in readers){
            if(!unlocked && reader.bounds.Contains(key.transform.position)){
                lockedDoor.Unlock(new Vector3(-2, 0, 0), 10f, 50f);

                unlocked = true;
            }
            if(unlocked){
                reader.readerMat[0].SetColor("_MainColor", Color.green);
                reader.readerMat[2].SetColor("_MainColor", Color.green);
                reader.readerMat[3].SetColor("_Color", Color.green + (Color.white/2));
            }
            else{
                lockedDoor.Lock();

                reader.readerMat[0].SetColor("_MainColor", Color.red);
                reader.readerMat[2].SetColor("_MainColor", Color.red);
                reader.readerMat[3].SetColor("_Color", Color.red + (Color.white/2));
            }
        }
    }
}

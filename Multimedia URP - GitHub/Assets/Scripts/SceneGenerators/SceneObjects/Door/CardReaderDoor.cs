using UnityEngine;
using System.Collections.Generic; 

public class CardReaderDoor : MonoBehaviour
{
    private LockedDoor lockedDoor;

    [SerializeField] private GameObject key;
    [SerializeField] private GameObject reader;

    [SerializeField] private Collider interactCollider;

    private List<Material> readerMat = new List<Material>();

    private Bounds boundsCollider;

    private bool unlocked = false;

    void Awake(){
        lockedDoor = GetComponent<LockedDoor>();
        boundsCollider = interactCollider.bounds;

        readerMat = new List<Material>(reader.GetComponent<Renderer>().materials);
    }

    void Update(){
        if(!unlocked && boundsCollider.Contains(key.transform.position)){
            lockedDoor.Unlock(new Vector3(-1, 0, 0), 10f, 50f);

            readerMat[0].SetColor("_Color", Color.green);
            readerMat[2].SetColor("_Color", Color.green);
            readerMat[3].SetColor("_Color", Color.green + (Color.white/2));

            unlocked = true;
        }
    }
}

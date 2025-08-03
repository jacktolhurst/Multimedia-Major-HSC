using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class StickyNoteMaker : MonoBehaviour
{
    private List<GameObject> notes = new List<GameObject>();
    private List<GameObject> madeNotes = new List<GameObject>();

    void OnEnable(){
        notes = CubicleGeneratorV2.instance.notes;
    }

    public void SpawnNotes(int amount=10){
        DestroyObjs(madeNotes);
        madeNotes = StickNotes(amount);
    }

    private void DestroyObjs(List<GameObject> objs){
        foreach(GameObject obj in objs){
            Destroy(obj);
        }
    }

    private List<GameObject> StickNotes(int amount){
        List<GameObject> spawnedNotes = new List<GameObject>();

        for(int i = 0; i < amount; i++){
            Vector3 direction = Random.onUnitSphere;
            Ray mainRay = new Ray(transform.position, direction);

            if(Physics.Raycast(mainRay, out RaycastHit hit, 100)){
                int layer = hit.transform.gameObject.layer;
                if(layer != 7 && layer != 8){
                    GameObject chosenNote = notes[Random.Range(0, notes.Count)];
                    float chosenNoteHeight = chosenNote.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;

                    Quaternion surfaceRotation = Quaternion.LookRotation(hit.normal);
                    Quaternion originalRotation = chosenNote.transform.rotation;

                    Quaternion finalRotation = surfaceRotation * originalRotation;

                    GameObject spawnedObj = Instantiate(chosenNote, (hit.point + hit.normal * 0.01f) + new Vector3(0, chosenNoteHeight, 0), finalRotation);

                    spawnedObj.transform.SetParent(hit.transform);

                    MeshRenderer renderer = spawnedObj.GetComponent<MeshRenderer>();
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                
                    block.SetVector("_RandomNumber", Random.onUnitSphere);
                    renderer.SetPropertyBlock(block);

                    spawnedNotes.Add(spawnedObj);
                }
            }
        }

        return spawnedNotes;
    }
}

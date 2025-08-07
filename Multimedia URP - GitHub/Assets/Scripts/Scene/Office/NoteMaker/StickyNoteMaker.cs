using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class StickyNoteMaker : MonoBehaviour
{
    private List<GameObject> notes = new List<GameObject>();
    private List<GameObject> madeNotes = new List<GameObject>();

    private List<Vector3> prevPoints = new List<Vector3>();

    private int noteAmount;

    void Awake(){
        notes = CubicleGeneratorV2.instance.notes;
    }

    public void SpawnNotes(int amount=10){
        SetNoteAmount(amount);

        DestroyObjs(madeNotes);
        madeNotes = StickNotes(GetNoteAmount());
    }

    public void SetNoteAmount(int amount){
        noteAmount = amount;
    }

    public int GetNoteAmount(){
        return noteAmount;
    }

    private void DestroyObjs(List<GameObject> objs){
        foreach(GameObject obj in objs){
            Destroy(obj);
        }
    }

    private bool IsNearAnyVector(List<Vector3> points, Vector3 targetPoint, float threshold) {
        return points.Any(p => Vector3.Distance(p, targetPoint) < threshold);
    }

    private List<GameObject> StickNotes(int amount){
        List<GameObject> spawnedNotes = new List<GameObject>();

        for(int i = 0; i < amount; i++){
            Vector3 direction = Random.onUnitSphere;
            direction.y = Mathf.Abs(direction.y);
            direction.z = Mathf.Abs(direction.z);

            Ray mainRay = new Ray(transform.position, direction);

            if(Physics.Raycast(mainRay, out RaycastHit hit, 100)){
                int layer = hit.transform.gameObject.layer;
                if(layer != 0 && layer != 7 && layer != 8 && !IsNearAnyVector(prevPoints, hit.point, 0.5f)){
                    GameObject hitObj = hit.transform.gameObject;
                    GameObject chosenNote = notes[Random.Range(0, notes.Count)];

                    Quaternion surfaceRotation = Quaternion.LookRotation(hit.normal);
                    Quaternion originalRotation = chosenNote.transform.rotation;
                    Quaternion finalRotation = surfaceRotation * originalRotation;

                    Bounds noteBounds = chosenNote.GetComponent<Renderer>().bounds;
                    Vector3 halfExtents = noteBounds.extents;

                    GameObject spawnedObj = Instantiate(chosenNote, (hit.point + hit.normal * 0.01f) - new Vector3(0, halfExtents.y, 0), finalRotation);

                    if(hitObj.name.Contains("Cubicle")){
                        spawnedObj.isStatic = true;
                    }

                    spawnedObj.transform.SetParent(hit.transform);

                    MeshRenderer renderer = spawnedObj.GetComponent<MeshRenderer>();
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    Vector2 randomOffset = Random.onUnitSphere;
                    block.SetVector("_GeneralOffset", randomOffset);
                    renderer.SetPropertyBlock(block);

                    spawnedNotes.Add(spawnedObj);
                    prevPoints.Add(hit.point);
                }
            }
        }

        return spawnedNotes;
    }
}

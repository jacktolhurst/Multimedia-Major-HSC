using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class StickyNoteMaker : MonoBehaviour
{
    [SerializeField] private List<GameObject> notes = new List<GameObject>();

    private Collider[] selfColliders;

    void Awake(){
        selfColliders = GetComponents<Collider>();

        StartCoroutine(StickNotes());
    }

    private IEnumerator StickNotes(){
        yield return null;
        yield return null;

        for(int i = 0; i < 100; i++){
            Vector3 direction = Random.onUnitSphere;
            Ray mainRay = new Ray(transform.position, direction);

            if(Physics.Raycast(mainRay, out RaycastHit hit, 100)){
                if(selfColliders.Contains(hit.collider.GetComponent<Collider>())){
                    GameObject chosenNote = notes[Random.Range(0, notes.Count)];
                    Quaternion surfaceRotation = Quaternion.LookRotation(-hit.normal);
                    surfaceRotation = surfaceRotation * Quaternion.Euler(0f, 0f, 180f);
                    
                    Instantiate(chosenNote, hit.point + hit.normal * 0.01f, surfaceRotation);
                }
            }
        }
    }

}

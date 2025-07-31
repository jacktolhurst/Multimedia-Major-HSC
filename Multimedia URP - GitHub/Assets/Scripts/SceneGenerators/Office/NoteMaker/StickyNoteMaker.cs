using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class StickyNoteMaker : MonoBehaviour
{
    [SerializeField] private List<GameObject> notes = new List<GameObject>();

    private Collider[] selfColliders;

    void Awake(){
        selfColliders = GetComponents<Collider>();

        Vector3 direction = new Vector3(Random.value, Random.value, Random.value);
        Ray mainRay = new Ray(transform.position, direction);
        
        if(Physics.Raycast(mainRay, out RaycastHit hit, 10)){
            if(selfColliders.Contains(hit.collider.GetComponent<Collider>())){
                Instantiate(notes[Random.Range(0, notes.Count-1)], hit.point, Quaternion.identity);
                print("hit something");
            }
        }

        print("woke up");
    }
}

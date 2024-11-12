using UnityEngine;
using System.Collections.Generic;

public class CubicleGenerator : MonoBehaviour
{
    private Renderer cubicleRenderer;

    [SerializeField] private GameObject cubiclePrefab;
    private GameObject lastCubicle;
    private GameObject currentCubicle;
    private List<GameObject> allCubicles = new List<GameObject>();

    [SerializeField] private Vector2 cubicleDistance;

    private bool reachedMaxCubicles = false;   

    void OnEnable(){
        GenerateStatics();
    }

    private void GenerateStatics(){
        cubicleRenderer = cubiclePrefab.GetComponent<Renderer>();

        if(lastCubicle == null){
            lastCubicle = cubiclePrefab;
        }

        while(!reachedMaxCubicles){
            Vector3 areaSize = cubicleRenderer.bounds.size;
            Vector3 generatedPosition = new Vector3(lastCubicle.transform.position.x + areaSize.x + cubicleDistance.x, cubiclePrefab.transform.position.y, lastCubicle.transform.position.z); 

            generatedPosition = CheckPhysicsBox(generatedPosition, areaSize);

            currentCubicle = Instantiate(cubiclePrefab, generatedPosition, cubiclePrefab.transform.rotation);
            allCubicles.Add(currentCubicle);
            currentCubicle.transform.localScale = Vector3.Scale(cubiclePrefab.transform.localScale, cubiclePrefab.transform.parent.gameObject.transform.localScale);

            lastCubicle = currentCubicle;
        }
    }

    private Vector3 CheckPhysicsBox(Vector3 position, Vector3 size){
        bool hasGround = false;

        Collider[] collidersInArea = Physics.OverlapBox(position, size / 2);
        if(collidersInArea.Length > 0){
            foreach (Collider collider in collidersInArea){
                if(collider.gameObject.layer == 11){
                    hasGround = true;
                }
                else{
                    position.x = cubiclePrefab.transform.position.x;
                    position.z -= cubicleDistance.y + size.z;

                    return CheckPhysicsBox(position, size);
                }
            }
        }

        if(!hasGround){
            reachedMaxCubicles = true;
        }
        return position;
    }

    void OnDisable(){
        foreach(GameObject obj in allCubicles){
            Destroy(obj);
        }
        allCubicles.Clear();
        reachedMaxCubicles = false;
    }
}

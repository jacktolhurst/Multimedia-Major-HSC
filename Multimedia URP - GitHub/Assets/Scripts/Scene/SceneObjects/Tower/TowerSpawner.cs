using UnityEngine;
using System.Collections.Generic;

public class TowerSpawner : MonoBehaviour
{
    private List<GameObject> spawnedObjs;

    [SerializeField] private LayerMask spawnMask;

    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private GameObject lookAtObj;
    private GameObject parent;

    [SerializeField] private Vector3 checkArea;
    private Vector3 towerSize;
    
    [SerializeField] private int towerAmount;

    [SerializeField] private bool respawn;

    void Awake(){
        spawnedObjs = SpawnTowers(towerAmount, transform.position, checkArea, SizeOfPrefab(towerPrefab), towerPrefab, lookAtObj, spawnMask);

        parent = new GameObject("TowerMain");
        AddObjectsToParent(parent, spawnedObjs);
    }

    void Update(){
        if(respawn){
            DestroyTowers(spawnedObjs);

            spawnedObjs = SpawnTowers(towerAmount, transform.position, checkArea, SizeOfPrefab(towerPrefab), towerPrefab, lookAtObj, spawnMask);

            AddObjectsToParent(parent, spawnedObjs);


            respawn = false;
        }
    }

    private void RotateObjsToObj(GameObject target, List<GameObject> objs){
        Transform targetTrans = target.transform;
        foreach(GameObject obj in objs){
            Vector3 direction = targetTrans.position - obj.transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            Vector3 euler = targetRotation.eulerAngles;
            float yRotation = euler.y;

            Vector3 currentEuler = obj.transform.rotation.eulerAngles;

            obj.transform.rotation = Quaternion.Euler(currentEuler.x, yRotation + Random.Range(-30,30), currentEuler.z);
        }
    }

    private void AddObjectsToParent(GameObject parent, List<GameObject> children){
        Transform parentTrans = parent.transform;
        foreach(GameObject child in children){
            child.transform.parent = parentTrans;
        }
    }

    private void DestroyTowers(List<GameObject> objs){
        foreach(GameObject obj in objs){
            Destroy(obj);
        }
    }

    private List<GameObject> SpawnTowers(int amount, Vector3 spawnCenter, Vector3 spawnSize, Vector3 towerSize, GameObject tower, GameObject target, LayerMask spawnMask){
        int iterations = 0;
        List<GameObject> spawnedObjs = new List<GameObject>();
        Transform targetTrans = target.transform;
        for(int i = 0; i < amount; i++){
            Vector3 point = RandomPointInBox(spawnCenter, spawnSize);

            Ray ray = new Ray(point, Vector3.down);
            if(Physics.Raycast(ray, out RaycastHit hit, spawnSize.y - point.y, spawnMask, QueryTriggerInteraction.Ignore)){
                if(hit.collider.gameObject.layer == 11 || Physics.CheckBox(hit.point, towerSize/2, Quaternion.identity, ~spawnMask)){
                    iterations ++;
                    i--;
                    continue;
                }
                else{

                    Vector3 forward = Vector3.ProjectOnPlane(Vector3.forward, hit.normal);
                    if (forward.sqrMagnitude < 0.0001f)
                        forward = Vector3.ProjectOnPlane(Vector3.right, hit.normal);
                    
                    Vector3 pos =  hit.point + new Vector3(0,-1,0);
                    Quaternion rotation = Quaternion.LookRotation(forward.normalized, hit.normal);

                    Vector3 direction = targetTrans.position - pos;

                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    Vector3 euler = targetRotation.eulerAngles;
                    float yRotation = euler.y;

                    Vector3 currentEuler = rotation.eulerAngles;
                    rotation = Quaternion.Euler(currentEuler.x, yRotation + Random.Range(-30,30), currentEuler.z);

                    GameObject spawnedObj = Instantiate(tower, pos, rotation);
                    spawnedObjs.Add(spawnedObj);
                }
            }
            else {
                i--;
            }

            if(iterations > amount*2){
                break;
            }

            iterations++;
        }

        return spawnedObjs;
    }

    private Vector3 RandomPointInBox(Vector3 center, Vector3 size){
        float x = Random.Range(-size.x / 2f, size.x / 2f);
        float y = Random.Range(-size.y / 2f, size.y / 2f);
        float z = Random.Range(-size.z / 2f, size.z / 2f);

        return center + new Vector3(x, y, z);
    }

    private Vector3 SizeOfPrefab(GameObject prefab){
        GameObject temp = Instantiate(prefab);
        Renderer[] renderers = temp.GetComponentsInChildren<Renderer>();

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        Vector3 prefabSize = bounds.size;
        Destroy(temp);

        return prefabSize;
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, checkArea);
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class CubicleGeneratorV2 : MonoBehaviour
{    
    [System.Serializable]
    private class objects{
        public GameObject objMain;
        public int chance;
    }

    private Dictionary<GameObject, Vector3> parentVector = new Dictionary<GameObject, Vector3>();
    
    [SerializeField] private List<objects> cubicleObjs = new List<objects>();

    private List<GameObject> generatedObjs = new List<GameObject>();

    [SerializeField] private GameObject player;
    private GameObject nonChunkedParent;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;

    private Vector3 startingPos = Vector3.zero;

    [SerializeField] private float additionDistFromPrev;
    [SerializeField] private float walkwayDistance;
    [SerializeField] private float checkDistance;

    private int rows = 0;
    [SerializeField] private int chunkSize;
    private int iteration = 0;
    [SerializeField] private int maxIteration; // the max iteration count, pretty arbetrary
    [SerializeField] private int interval;

    [SerializeField] private bool reGenerate;
    [SerializeField] private bool drawGizmos;

    void Awake(){
        startingPos = transform.position;

        CreateCubicles();

    }

    void Start(){
        StartCoroutine(DistCheck());
    }

    void Update(){ // if reGenerate then redo all cubicles, this is mostly for testing purposes
        if(reGenerate){
            transform.position = startingPos;
            rows = 0;
            iteration = 0;

            DestroyCubicles();
            CreateCubicles();
            reGenerate = false;
        }
    }

    private IEnumerator DistCheck(){
        while(true){
            foreach(KeyValuePair<GameObject, Vector3> pair in parentVector){
                if(!IsClose(pair.Value, player.transform.position, checkDistance)){
                    if(pair.Key.activeSelf != false){
                        pair.Key.SetActive(false);
                        yield return null;
                    }
                }
                else{
                    if(pair.Key.activeSelf != true){
                        pair.Key.SetActive(true);
                        yield return null;
                    }
                }
            }
            yield return null;
        }
    }


    private void CreateCubicles(){ // starts the creation of all cubicles
        List<GameObject> nonChunkedObjects = new List<GameObject>();
        
        while(iteration < maxIteration){

            List<GameObject> chunkedObjects = new List<GameObject>();

            for(int i = 0; i < chunkSize; i++){

                GameObject obj = GetObj();
                Vector3 position = transform.position;

                if(chunkedObjects.Count != 0){
                    position = GetPos(obj); 

                    if(position == Vector3.zero){
                        position = startingPos - new Vector3(0,0,walkwayDistance * (rows + 1));
                        if(IsPosValid(position, obj)){
                            rows ++;
                            transform.position = position;

                            break;
                        }
                        else{
                            iteration = maxIteration;
                            break;
                        }
                    }
                    else{
                        transform.position = position;
                    }
                }

                GameObject instanceObj = Instantiate(obj, position, Quaternion.identity);

                foreach(Transform childTrans in instanceObj.transform) {
                    GameObject child = childTrans.gameObject;

                    if(child.tag == "StayInScene"){
                        nonChunkedObjects.Add(child);
                        Destroy(child);
                    }
                }

                chunkedObjects.Add(instanceObj);

                if(i == 0){
                    Destroy(chunkedObjects[chunkedObjects.Count-1]);
                }
            }

            GameObject parentObj = new GameObject($"Parent {iteration + 1}");
            generatedObjs.Add(parentObj);

            List<GameObject> children = new List<GameObject>();

            foreach(GameObject obj in chunkedObjects){
                children.Add(obj);

                obj.transform.parent = parentObj.transform;
            }
            
            parentVector.Add(parentObj, GetMidPosition(children)); 

            iteration ++;
        }

        nonChunkedParent = new GameObject("Parent Stay In Scene");
        generatedObjs.Add(nonChunkedParent);
        
        foreach(GameObject child in nonChunkedObjects){
            GameObject obj = Instantiate(child, child.transform.position, child.transform.rotation, nonChunkedParent.transform);
            if(child.name.Contains("Cubicle")){
                obj.transform.localScale *= 1.6f;
            }
        }
    }
    
    private GameObject GetObj(){ //  gets the random obj each time, uses the RandomChance func to get the chance then returns if the total chance is higher teh random chance, oif nothing returns just do the most likely one
        int randChance = RandomChance();

        int totalChance = 0;

        int highestChance = 0;
        GameObject highestChanceObj = null;

        foreach(objects obj in cubicleObjs){
            totalChance += obj.chance;

            if(randChance < totalChance){
                return obj.objMain;
            }

            if(obj.chance > highestChance){
                highestChance = obj.chance;
                highestChanceObj = obj.objMain;
            }
        }

        return highestChanceObj;
    }

    private int RandomChance(){ // gets the random chance of the list by adding all the chances up and returning a random value
        int chance = 0;

        foreach(objects obj in cubicleObjs){
            chance += obj.chance;
        }

        return Random.Range(0, chance);
    }

    private Vector3 GetPos(GameObject obj){
        for(int i = 0; i < 2; i++){
            Vector3 generatedPos = transform.position +  new Vector3(GetBounds(obj).size.x + additionDistFromPrev, 0, 0);
            if(IsPosValid(generatedPos, obj)){
                return generatedPos;
            }
        }
        return Vector3.zero;
    }

    private bool IsPosValid(Vector3 pos, GameObject obj){ // checks if the position is valid, if there is a ground but nothing else, true, if it hirts a wall, false, if it hits nothing, false
        if(Physics.CheckBox(pos, GetBounds(obj).extents,  Quaternion.identity, wallMask)){
            return false;
        }
        else if(Physics.CheckBox(pos, GetBounds(obj).extents, Quaternion.identity, groundMask)){
            return true;
        }
        return false;
    }

    private Bounds GetBounds(GameObject obj){ // returns the bounds of the largest object on the x in the children of the object
        float boundsSize = 0;
        Bounds newBounds = new Bounds();

        foreach(Transform childTrans in obj.transform){
            GameObject child = childTrans.gameObject;
            if(child.GetComponent<Renderer>()){
                if(child.GetComponent<Renderer>().bounds.size.x > boundsSize){
                    newBounds = child.GetComponent<Renderer>().bounds;
                    boundsSize = newBounds.size.x;
                }
            }
        }

        return newBounds;
    }

    private Vector3 GetMidPosition(List<GameObject> objs){ //  returns the middle position between the highest and lowest in the list, the Vector3 is timsed so then you can get the axis
        if(objs.Count >= 2){
            return (objs[0].transform.position + objs[objs.Count - 1].transform.position) / 2;
        }
        else{
            return objs[0].transform.position;
        }
    }

    private bool IsClose(Vector3 APos, Vector3 BPos, float dist){
        return (APos - BPos).sqrMagnitude < dist * dist;
    }

    private void DestroyCubicles(){
        foreach(GameObject obj in generatedObjs){
            Destroy(obj);
        }

        generatedObjs.Clear();
        parentVector.Clear();
    }

    public void KeepObject(GameObject obj){
        parentVector.Remove(obj);
        obj.transform.parent = nonChunkedParent.transform;
    }

    void OnDrawGizmos(){
        if(drawGizmos){
            Gizmos.color = Color.red;
            foreach(KeyValuePair<GameObject, Vector3> pair in parentVector){
                Gizmos.DrawWireSphere(pair.Value + new Vector3(0, 10,0), 2);
            }

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(player.transform.position, checkDistance);
        }
    }
}

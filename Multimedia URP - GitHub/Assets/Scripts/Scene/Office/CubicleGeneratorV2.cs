using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic; 

public class CubicleGeneratorV2 : MonoBehaviour
{    
    [System.Serializable]
    private class objects{
        public GameObject objMain;
        public int chance;
        public int noteAmount;
    }

    public static CubicleGeneratorV2 instance;


    private Dictionary<GameObject, Vector3> parentVector = new Dictionary<GameObject, Vector3>();

    private List<StickyNoteMaker> noteMakers = new List<StickyNoteMaker>();

    [SerializeField] private List<objects> cubicleObjs = new List<objects>();
    [SerializeField] private List<objects> cubicleBorderObjs = new List<objects>();

    public List<GameObject> notes = new List<GameObject>();
    private List<GameObject> generatedObjs = new List<GameObject>();

    [SerializeField] private FirstPersonMovement FPM;

    [SerializeField] private GameObject player;
    private GameObject nonChunkedParent;
    private GameObject majorParent;
    [SerializeField] private GameObject playerCubicle;

    private Bounds checkBounds;
    private Bounds playerMovementBounds;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;

    private Vector3 startingPos = Vector3.zero;
    private Vector3 playerPos;
    [SerializeField] private Vector3 checkBoundsSize;
    [SerializeField] private Vector3 playerDistBoundsSize;
    private Vector3 delayedPlayerPos;

    [SerializeField] private float additionDistFromPrev;
    [SerializeField] private float walkwayDistance;
    [SerializeField] private float gapDistance;

    private int rows = 0;
    [SerializeField] private int chunkSize;
    private int iteration = 0;
    [SerializeField] private int maxIteration; // the max iteration count, pretty arbetrary but needed
    [SerializeField] private int playerCubicleNum;

    [SerializeField] private bool reGenerate;
    [SerializeField] private bool drawGizmos;
    private bool updatedPlayerMovementBounds;
    [SerializeField] private bool doDistCheck;

    void Awake(){
        if(instance != null){
            Debug.LogWarning("Two CubicleGenerator V2 instances");
        }
        instance = this;

        startingPos = transform.position;
        playerPos = player.transform.position;
        delayedPlayerPos = playerPos;

        CreateCubicles();
        SpawnNotes(noteMakers);

        checkBounds = GetComponent<BoxCollider>().bounds;
        checkBounds.center = playerPos;
        checkBounds.size = checkBoundsSize;

        playerMovementBounds = new Bounds(playerPos, playerDistBoundsSize);
    }

    void Start(){
        EarlyDistCheck();

        StartCoroutine(DistCheck());
    }

    void Update(){ // if reGenerate then redo all cubicles, this is mostly for testing purposes
        if(reGenerate){
            transform.position = startingPos;
            rows = 0;
            iteration = 0;

            DestroyCubicles();
            CreateCubicles();
            SpawnNotes(noteMakers);

            reGenerate = false;
        }

        playerPos = player.transform.position;

        checkBounds.center = playerPos;
        checkBounds.size = checkBoundsSize;
        
        playerMovementBounds.size = playerDistBoundsSize;
    }

    private void SpawnNotes(List<StickyNoteMaker> noteMakers){
        foreach(StickyNoteMaker noteMaker in noteMakers) noteMaker.SpawnNotes(Random.Range(0,100));
    }

    private void EarlyDistCheck(){ // an early version of discheck used in the awake to leverage intial memory
        if(doDistCheck){
            foreach(KeyValuePair<GameObject, Vector3> pair in parentVector){
                if(!checkBounds.Contains(pair.Value)){
                    if(pair.Key.activeSelf){
                        pair.Key.SetActive(false);
                    }
                }
                else{
                    if(!pair.Key.activeSelf){
                        pair.Key.SetActive(true);
                    }
                }
            }
        }
    }

    private IEnumerator DistCheck(){
        while(doDistCheck){
            if(!playerMovementBounds.Contains(playerPos)){
                foreach(KeyValuePair<GameObject, Vector3> pair in parentVector){
                    if(!checkBounds.Contains(pair.Value)){
                        if(pair.Key.activeSelf){
                            pair.Key.SetActive(false);
                            yield return null;
                        }
                    }
                    else{
                        if(!pair.Key.activeSelf){
                            pair.Key.SetActive(true);
                            yield return null;
                        }
                    }
                }
                updatedPlayerMovementBounds = true;
                playerMovementBounds.center = playerPos;
            }
            else{
                yield return null;
            }
        }
    }

    private void CreateCubicles(){ // starts the creation of all cubicles
        transform.position = GetPos(null, gapDistance);

        List<GameObject> nonChunkedObjects = new List<GameObject>();
        List<GameObject> parentObjects = new List<GameObject>();

        nonChunkedParent = new GameObject("Parent Stay In Scene");
        generatedObjs.Add(nonChunkedParent);
        parentObjects.Add(nonChunkedParent);

        
        while(iteration < maxIteration){
            List<GameObject> chunkedObjects = new List<GameObject>();

            for(int i = 0; i < chunkSize + 1; i++){

                objects objClass = GetObjFromList(cubicleObjs);
                GameObject obj = objClass.objMain;

                
                if(playerCubicleNum == iteration * chunkSize + i){
                    obj = playerCubicle;
                }

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
                        
                        if(playerCubicleNum == iteration * chunkSize + i){
                            FPM.generatedStartPos = transform.position;
                        }
                    }
                }

                if(i != 0){
                    GameObject instanceObj = Instantiate(obj, position, Quaternion.identity);

                    foreach(Transform childTrans in instanceObj.transform) {
                        GameObject child = childTrans.gameObject;

                        if(child.tag == "StayInScene"){
                            if (child.name.Contains("Cubicle")){

                                objects borderClass = GetObjFromList(cubicleBorderObjs);
                                GameObject borderPrefab = borderClass.objMain;

                                GameObject spawnedObj = Instantiate(borderPrefab, childTrans.position, borderPrefab.transform.rotation, instanceObj.transform);
                                spawnedObj.tag = "StayInScene";

                                Destroy(child);

                                child = spawnedObj;
                            }
                            nonChunkedObjects.Add(child);

                            child.transform.SetParent(nonChunkedParent.transform);
                        }
                    }

                    if(playerCubicleNum != iteration * chunkSize + i){
                        StickyNoteMaker stickyNoteScript = instanceObj.AddComponent<StickyNoteMaker>();
                        stickyNoteScript.SetNoteAmount(objClass.noteAmount);

                        noteMakers.Add(stickyNoteScript);
                    }

                    chunkedObjects.Add(instanceObj);
                }
            }

            GameObject parentObj = new GameObject($"Parent {iteration + 1}");
            generatedObjs.Add(parentObj);
            parentObjects.Add(parentObj);

            List<GameObject> children = new List<GameObject>();

            foreach(GameObject obj in chunkedObjects){
                children.Add(obj);

                obj.transform.parent = parentObj.transform;
            }
            
            parentVector.Add(parentObj, GetMidPosition(children)); 

            iteration ++;

            transform.position = GetPos(null, gapDistance);
        }

        CombineRecursively(nonChunkedParent);

        
        majorParent = new GameObject("CubiclesParent");
        foreach(GameObject obj in parentObjects){
            obj.transform.parent = majorParent.transform;
        }
    }

    private void CombineRecursively(GameObject root){
        List<CombineInstance> combines = new List<CombineInstance>();
        List<Material> materials = new List<Material>();

        void CollectMeshesRecursive(Transform current){
            MeshFilter mf = current.GetComponent<MeshFilter>();
            MeshRenderer mr = current.GetComponent<MeshRenderer>();

            if (mf != null && mr != null && mf.sharedMesh != null) {
                Mesh mesh = mf.sharedMesh;
                Material[] mats = mr.sharedMaterials;

                for (int sub = 0; sub < mesh.subMeshCount; sub++){
                    CombineInstance ci = new CombineInstance{
                        mesh = mesh,
                        subMeshIndex = sub,
                        transform = mf.transform.localToWorldMatrix
                    };
                    combines.Add(ci);

                    if (sub < mats.Length)
                        materials.Add(mats[sub]);
                    else
                        materials.Add(mats[0]);
                }
            }

            foreach (Transform child in current){
                if(!child.GetComponent<Rigidbody>())CollectMeshesRecursive(child);
            }
        }

        CollectMeshesRecursive(root.transform);

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        combinedMesh.CombineMeshes(combines.ToArray(), false, true);

        MeshFilter mfRoot = root.GetComponent<MeshFilter>();
        if (mfRoot == null) mfRoot = root.AddComponent<MeshFilter>();
        mfRoot.sharedMesh = combinedMesh;

        MeshRenderer mrRoot = root.GetComponent<MeshRenderer>();
        if (mrRoot == null) mrRoot = root.AddComponent<MeshRenderer>();
        mrRoot.sharedMaterials = materials.ToArray();

        foreach (Transform child in root.transform){
            Destroy(child.gameObject.GetComponent<MeshRenderer>());
            Destroy(child.gameObject.GetComponent<MeshFilter>());

            if (child.gameObject.GetComponents<Component>().Length == 1) 
            {
                Destroy(child.gameObject);
            }
        }
    }

    
    private objects GetObjFromList(List<objects> objects){ //  gets the random obj each time, uses the RandomChance func to get the chance then returns if the total chance is higher teh random chance, oif nothing returns just do the most likely one
        int randChance = RandomChanceFromList(objects);

        int totalChance = 0;

        int highestChance = 0;
        objects highestChanceObj = null;

        foreach(objects obj in objects){
            totalChance += obj.chance;

            if(randChance < totalChance){
                return obj;
            }

            if(obj.chance > highestChance){
                highestChance = obj.chance;
                highestChanceObj = obj;
            }
        }

        return highestChanceObj;
    }

    private int RandomChanceFromList(List<objects> objects){ // gets the random chance of the list by adding all the chances up and returning a random value
        int chance = 0;

        foreach(objects obj in objects){
            chance += obj.chance;
        }

        return Random.Range(0, chance);
    }

    private Vector3 GetPos(GameObject obj = null, float size = 0){
        if(obj != null){
            for(int i = 0; i < 2; i++){
                Vector3 generatedPos = transform.position +  new Vector3(GetBounds(obj).size.x + additionDistFromPrev + Random.Range(0f, 0.1f), 0, 0);
                if(IsPosValid(generatedPos, obj)){
                    return generatedPos;
                }
            }
            return Vector3.zero;
        }
        else{
            Vector3 generatedPos = transform.position +  new Vector3(size + additionDistFromPrev, 0, 0);
            return generatedPos;
        }
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

    private Bounds GetBounds(GameObject obj) {
        Bounds combined = new Bounds();
        bool first = true;
    
        foreach (var r in obj.GetComponentsInChildren<Renderer>()) {
            Bounds b = r.localBounds;
            b.size = Vector3.Scale(b.size, r.transform.lossyScale);
    
            if (first) {
                combined = b;
                first = false;
            } else {
                combined.Encapsulate(new Bounds(b.center, b.size));
            }
        }
    
        return combined;
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
        noteMakers.Clear();
    }

    public void KeepObject(GameObject obj){
        parentVector.Remove(obj);
        obj.transform.parent = nonChunkedParent.transform;
    }

    void OnDrawGizmos(){
        if(drawGizmos && Application.isPlaying){
            Gizmos.color = Color.red;
            foreach(KeyValuePair<GameObject, Vector3> pair in parentVector){
                Gizmos.DrawWireSphere(pair.Value + new Vector3(0, 10,0), 2);
            }

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(delayedPlayerPos, checkBoundsSize);

            Gizmos.DrawWireCube(delayedPlayerPos, playerDistBoundsSize);

            if(updatedPlayerMovementBounds){
                delayedPlayerPos = playerPos;
                updatedPlayerMovementBounds = false;
            }
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

public class CubicleGenerator : MonoBehaviour
{
    [System.Serializable]
    private class cubicleObj{
        public GameObject gameObject;
        public float chance;
    }

    [Header("Cubicle")]
    [SerializeField] private List<cubicleObj> cubicleObjs = new List<cubicleObj>();
    private List<GameObject> generatedCubicles = new List<GameObject>();

    [Header("Cubicle Attributes")]
    [SerializeField] private Vector3 addedPosition;
    [SerializeField] private Vector3 CubicleSize;

    private GameObject selectedCubicle;

    [SerializeField] private int generations;
    
    [Header("Positioning")]
    [SerializeField] private GameObject player;

    private Vector3 initialPosition;
    private Vector3 lastPlayerPos = Vector3.zero;
    private Vector3 playerBoundsSize;
    private Vector3 playerIntervalPos;

    [Header("Rendering")]
    [SerializeField] private float cubicleRenderDist;
    private float lastCubicleRenderDist;
    [Range(0.1f, 20f)]
    [SerializeField] private float interval;
    private float lastInterval;


    void Awake(){
        playerBoundsSize = player.GetComponent<Renderer>().bounds.size;
    }

    void OnEnable(){
        initialPosition = transform.position;

        for (int i = 0; i < generations; i++){
            selectedCubicle = Instantiate(PickCubicle(cubicleObjs),  CreatePosition(),  Quaternion.identity);
            generatedCubicles.Add(selectedCubicle);

            transform.position = new Vector3(transform.position.x + 6, transform.position.y, transform.position.z);
        }

        InvokeRepeating(nameof(EveryInterval), 0, interval);

        lastInterval = interval;
        lastCubicleRenderDist = cubicleRenderDist;
    }

    private float GetChances(List<cubicleObj> cubicleObjs){
        float totalWeight = 0;
        foreach(cubicleObj obj in cubicleObjs){
            totalWeight += obj.chance;
        }
        return Random.Range(0, totalWeight);;
    }

    private GameObject PickCubicle(List<cubicleObj> cubicleObjs){

        float randomChance = GetChances(cubicleObjs);

        float cumulativeWeight = 0f;

        foreach(cubicleObj obj in cubicleObjs){
            cumulativeWeight += obj.chance;

            if (randomChance < cumulativeWeight){
                return obj.gameObject;
            }
        }

        return cubicleObjs[cubicleObjs.Count - 1].gameObject;
    }

    private Vector3 CreatePosition(){
        Vector3 lastPosition = Vector3.zero; // The last position
        Vector3 tempPosition = Vector3.zero; // A temporary postion, mimicing the newPosition but without physics box testing.
        Vector3 newPosition = Vector3.zero; // The final position

        bool touchesWall = false;

    
        if(generatedCubicles.Count == 0){
            lastPosition = transform.position;
        }
        else{
            lastPosition = generatedCubicles[generatedCubicles.Count - 1].transform.position;
        }
        tempPosition = lastPosition + new Vector3(addedPosition.x, 0, 0);

        Collider[] physicsBox = Physics.OverlapBox(tempPosition, CubicleSize / 2, Quaternion.identity);
        if(physicsBox.Length > 0){
            foreach(Collider collider in physicsBox){
                if(collider.gameObject.layer == 8){
                    touchesWall = true;
                }
            }
        }

        if(touchesWall){
            newPosition = initialPosition + new Vector3(0, 0, addedPosition.z) + new Vector3(0, 0, lastPosition.z);
        }
        else{
            newPosition = tempPosition;
        }

        return newPosition;
    }

    void FixedUpdate(){
        if(lastInterval != interval || lastCubicleRenderDist != cubicleRenderDist){
            CancelInvoke(nameof(EveryInterval));
            InvokeRepeating(nameof(EveryInterval), 0, interval);

            lastInterval = interval;
            lastCubicleRenderDist = cubicleRenderDist;
        }
    }

    void EveryInterval(){
        foreach(GameObject obj in generatedCubicles){
            if(!isClose(player.transform.position, obj.transform.position, cubicleRenderDist)){
                obj.GetComponent<TurnLowPolyScript>().TurnLowPoly();
            }
            else{
                obj.GetComponent<TurnLowPolyScript>().TurnOffLowPoly();
            }
        }

        lastPlayerPos = player.transform.position;
    }

    private bool isClose(Vector3 APos, Vector3 BPos, float dist){
        return Vector3.Distance(APos, BPos) < dist;
    }

    void OnDisable(){
        foreach(GameObject cubicle in generatedCubicles){
            Destroy(cubicle);
        }

        generatedCubicles.Clear();

        transform.position = initialPosition;

        CancelInvoke(nameof(EveryInterval));
    }

    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(player.transform.position, cubicleRenderDist);
    
        Gizmos.DrawWireCube(lastPlayerPos, playerBoundsSize - new Vector3(0.1f,0.1f,0.1f));
    }

}

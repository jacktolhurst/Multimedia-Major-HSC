using UnityEngine;

public class CubicleGenerator : MonoBehaviour
{
    private Renderer cubicleRenderer;

    [SerializeField] private GameObject cubiclePrefab;
    private GameObject lastCubicle;
    private GameObject currentCubicle;

    [SerializeField] private int cubicleGenerations =  5;
    [SerializeField] private int cubicleXDistance;
    [SerializeField] private int cubicleZDistance;
    
    

    void Start(){
        GenerateStatics();
    }

    private void GenerateStatics(){
        cubicleRenderer = cubiclePrefab.GetComponent<Renderer>();

        if(lastCubicle == null){
            lastCubicle = cubiclePrefab;
        }
        for (int i = 0; i < cubicleGenerations; i++) {
            Vector3 areaSize = cubicleRenderer.bounds.size;
            Vector3 generatedPosition = new Vector3(lastCubicle.transform.position.x + areaSize.x + cubicleXDistance, cubiclePrefab.transform.position.y, lastCubicle.transform.position.z); 

            generatedPosition = CheckPhysicsBox(generatedPosition, areaSize);

            if(cubicleGenerations == 0){
                break;
            }

            currentCubicle = Instantiate(cubiclePrefab, generatedPosition, cubiclePrefab.transform.rotation);
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
                    position.z -= cubicleZDistance + size.z;

                    position = CheckPhysicsBox(position, size);
                }
            }
        }

        if(!hasGround){
            cubicleGenerations = 0;
        }
        return position;
    }
}

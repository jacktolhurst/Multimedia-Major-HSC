using UnityEngine;

public class TeleportRoom : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private BoxCollider selfCollider;

    private Bounds selfBounds;

    [SerializeField] private Vector3 teleportSize;
    [SerializeField] private Vector3 teleportPos;

    void Awake(){
        selfCollider = transform.gameObject.AddComponent<BoxCollider>();
        selfCollider.size = teleportSize;
        selfCollider.center = Vector3.zero; 
        selfCollider.isTrigger = true;

        selfBounds = selfCollider.bounds;

    }

    void Update(){
        if(selfBounds.Contains(player.transform.position)){
            player.transform.position = teleportPos;
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(teleportPos, 2);
        Gizmos.DrawWireCube(transform.position, teleportSize);
    }
}

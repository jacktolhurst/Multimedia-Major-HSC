using UnityEngine;
using UnityEngine.AI;

public class EnemyAIScript : MonoBehaviour
{
    [SerializeField] private SoundDetection SoundDetectionScript;

    private NavMeshAgent agent;

    private bool patrolling = false;

    void Awake(){
        agent = GetComponent<NavMeshAgent>();
    }
    
    void Update(){
        if(SoundDetectionScript.chosenEvent != null){
            agent.SetDestination(SoundDetectionScript.chosenEvent.position);
            patrolling = false;
        }
        else if (!patrolling){
            agent.SetDestination(transform.position + new Vector3(Random.Range(-10,10),Random.Range(-10,10),Random.Range(-10,10)));
            patrolling = true;
        }
    }
}

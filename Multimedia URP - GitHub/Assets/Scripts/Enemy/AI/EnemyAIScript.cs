using UnityEngine;
using UnityEngine.AI;

public class EnemyAIScript : MonoBehaviour
{
    [SerializeField] private SoundDetection SoundDetectionScript;

    private NavMeshAgent agent;

    void Awake(){
        agent = GetComponent<NavMeshAgent>();
    }
    
    void Update(){
        if(SoundDetectionScript.chosenEvent != null){
            agent.SetDestination(SoundDetectionScript.chosenEvent.position);
        }
    }
}

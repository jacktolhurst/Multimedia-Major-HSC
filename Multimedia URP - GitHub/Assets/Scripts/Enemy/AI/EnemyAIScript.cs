using UnityEngine;
using UnityEngine.AI;

public class EnemyAIScript : MonoBehaviour
{
    [SerializeField] private SoundDetection SoundDetectionScript;

    public NavMeshAgent agent;

    public LayerMask whatIsGround, whatIsPlayer;

    public Vector3 playerPos;
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, PlayerInAttackRange;

    void Awake(){
        agent = GetComponent<NavMeshAgent>();
    }

    void Update(){
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInSightRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!playerInSightRange && !PlayerInAttackRange) Patroling();

        if(playerInSightRange && !PlayerInAttackRange) ChasePlayer();

        if(playerInSightRange && PlayerInAttackRange)  AttackPlayer();

        Debug.Log(SoundDetectionScript.chosenEvent.position);
        
        if(SoundDetectionScript.chosenEvent.position != null) playerPos = SoundDetectionScript.chosenEvent.position;
    }

    private void Patroling(){
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)  agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void SearchWalkPoint(){
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX,transform.position.y ,transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) walkPointSet = true;
    }

    private void ChasePlayer(){
        agent.SetDestination(playerPos);
    }

    private void AttackPlayer(){
        agent.SetDestination(transform.position);
    }

    void OnDrawGizmos(){
        
    }
}

using UnityEngine;

public class KeyCardMaker : MonoBehaviour
{
    private LeverManager leverManager;

    private LeverManager.Lever lever;

    [SerializeField] private CardReaderDoor cardReaderDoorScript;

    [SerializeField] private GameObject keyCardPrefab;

    [SerializeField] private Vector3 cardSpawnPoint;
    [SerializeField] private Vector3 soundSpawnPoint;

    [SerializeField] private float health;
    private float originalHealth;
    [SerializeField] private float leverDamage;
    [SerializeField] private float soundSize;

    [SerializeField] private string leverName;

    [SerializeField] private AudioManager.AudioReferenceClass cardSpawnSound;

    void Awake(){
        originalHealth = health;

        leverManager = GetComponent<LeverManager>();

        lever = leverManager.GetLeverByName(leverName);
    }

    void Update(){
        if(lever.isRotating){
            health -= leverDamage*Time.deltaTime;
        }

        if(health <= 0){
            GameObject keyCard = Instantiate(keyCardPrefab, cardSpawnPoint, Random.rotation);
            cardReaderDoorScript.AddKey(keyCard);
            cardSpawnSound.PlaySoundPosition(soundSpawnPoint, soundSize);
            health = originalHealth;
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(cardSpawnPoint, 0.1f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(soundSpawnPoint, soundSize);
    }
}

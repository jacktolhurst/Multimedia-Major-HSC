using UnityEngine;
using System.Collections.Generic;

public class SoundDetection : MonoBehaviour
{
    private List<FMODEvents.SoundEventClass> soundEvents = new List<FMODEvents.SoundEventClass>();
    private FMODEvents.SoundEventClass chosenEvent;

    [SerializeField] private GameObject obj;

    [SerializeField] private float soundDistance;
    [SerializeField] private float sphereBaseSize;

    void Update(){
        soundEvents = AudioManager.instance.GetAllSoundsInRange(soundDistance, transform.position);

        float highestImpact = 0;
        foreach(FMODEvents.SoundEventClass soundEvent in soundEvents){
            if(soundEvent.impact > highestImpact){
                highestImpact = soundEvent.impact;
                chosenEvent = soundEvent;
            }
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, soundDistance);

        if(Application.isPlaying){  
            Gizmos.color = Color.red;
            foreach(FMODEvents.SoundEventClass soundEvent in soundEvents){
                if(chosenEvent != null && soundEvent == chosenEvent){
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawWireSphere(soundEvent.position, sphereBaseSize + (soundEvent.impact-1));
            }
            if(chosenEvent != null && soundEvents.Count != 0){
                Debug.DrawRay(transform.position, (chosenEvent.position - transform.position).normalized * Vector3.Distance(transform.position, chosenEvent.position), Color.green);
            }
        }
    }
}

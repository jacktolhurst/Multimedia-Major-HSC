using UnityEngine;
using System.Collections.Generic;

public class SoundDetection : MonoBehaviour
{
    List<AudioManager.EventHandler> soundEvents = new List<AudioManager.EventHandler>();
    [SerializeField] private AudioManager.EventHandler chosenEvent;


    [SerializeField] private float soundDistance;

    void Update(){
        soundEvents = AudioManager.instance.CurrentEventsInRange(transform.position, soundDistance);

        float highestImpact = 0;
        foreach(AudioManager.EventHandler soundEvent in soundEvents){
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
            foreach(AudioManager.EventHandler soundEvent in soundEvents){
                if(chosenEvent != null && soundEvent == chosenEvent){
                    Gizmos.color = Color.green;
                }
                else{
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawWireSphere(soundEvent.position, soundEvent.impact);
            }
            if(chosenEvent != null && soundEvents.Count != 0){
                Gizmos.color = Color.green;
                Debug.DrawRay(transform.position, (chosenEvent.position - transform.position).normalized * Vector3.Distance(transform.position, chosenEvent.position), Color.green);
            }
        }
    }
}

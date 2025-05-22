using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class SoundDetection : MonoBehaviour
{
    List<AudioManager.EventHandler> soundEvents = new List<AudioManager.EventHandler>();
    public AudioManager.EventHandler chosenEvent;

    [SerializeField] private float soundDistance;

    [SerializeField] private GameObject particleObject;
    
    public bool checkSounds = true;

    void Update(){
        if(checkSounds){
            soundEvents = AudioManager.instance.CurrentEventsInRange(transform.position, soundDistance);

            if(soundEvents.Count != 0){
                float highestImpact = 0;
                float lastEventTime = 0;
                foreach(AudioManager.EventHandler soundEvent in soundEvents){
                    if(soundEvent.impact > highestImpact){
                        highestImpact = soundEvent.impact;
                        lastEventTime = soundEvent.time;
                    }
                    if(soundEvent.impact == highestImpact){
                        if(soundEvent.time > lastEventTime){
                            lastEventTime = soundEvent.time;
                            chosenEvent = soundEvent;
                        }
                    }
                }
                Debug.Log(chosenEvent.position);
                // particleObject.transform.position = chosenEvent.position;
            }
            else{
                chosenEvent = null;
            }
        }
    }

    void OnDrawGizmos(){
        if(checkSounds){
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
}

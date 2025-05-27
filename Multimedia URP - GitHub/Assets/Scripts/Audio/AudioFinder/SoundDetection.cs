using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class SoundDetection : MonoBehaviour
{
    public AudioManager.EventHandler chosenEvent;

    [SerializeField] private Transform particleTargetTrans;

    [SerializeField] private LayerMask hitLayerMask;

    [SerializeField] private float soundDistance;

    public bool checkSounds = true;

    void Update(){
        chosenEvent = GetChosenEvent();
    }

    private AudioManager.EventHandler GetChosenEvent(){
        AudioManager.EventHandler newChosenEvent = null;

        if(checkSounds){
            List<AudioManager.EventHandler> soundEvents = AudioManager.instance.CurrentEventsInRange(transform.position, soundDistance);

            if(soundEvents.Count != 0){
                float highestImpact = 0;
                float lastEventTime = 0;
                foreach(AudioManager.EventHandler soundEvent in soundEvents){
                    Ray eventRay = new Ray(soundEvent.position, particleTargetTrans.position - soundEvent.position);
                    
                    if(!Physics.Raycast(eventRay, out RaycastHit hit, Vector3.Distance(particleTargetTrans.position, soundEvent.position),hitLayerMask)){
                        if(soundEvent.impact > highestImpact){
                            highestImpact = soundEvent.impact;
                            lastEventTime = soundEvent.time;
                            newChosenEvent = soundEvent;
                        }
                        if(soundEvent.impact == highestImpact){
                            if(soundEvent.time > lastEventTime){
                                lastEventTime = soundEvent.time;
                                newChosenEvent = soundEvent;
                            }
                        }
                    }
                }
            }
        }

        return newChosenEvent;
    }   

    void OnDrawGizmos(){
        if(checkSounds){
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, soundDistance);

            if(Application.isPlaying){  
                List<AudioManager.EventHandler> soundEvents = AudioManager.instance.CurrentEventsInRange(transform.position, soundDistance);
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

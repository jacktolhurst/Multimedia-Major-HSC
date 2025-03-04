using UnityEngine;
using System.Collections.Generic;

public class SoundDetection : MonoBehaviour
{
    private List<FMODEvents.SoundEventClass> soundEvents = new List<FMODEvents.SoundEventClass>();

    [SerializeField] private GameObject obj;

    [SerializeField] private float soundDistance;
    [SerializeField] private float sphereBaseSize;
    [SerializeField] private float sphereBaseTime;

    void Update(){
        soundEvents = AudioManager.instance.GetAllSoundsInRange(soundDistance, transform.position);
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, soundDistance);

        if(Application.isPlaying){  
            Gizmos.color = Color.red;
            foreach(FMODEvents.SoundEventClass soundEvent in soundEvents){
                Gizmos.DrawWireSphere(soundEvent.position, sphereBaseSize + (soundEvent.impact-1));
            }
        }
    }
}

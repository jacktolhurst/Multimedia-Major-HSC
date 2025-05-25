using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class SoundDetection : MonoBehaviour
{
    private List<AudioManager.EventHandler> soundEvents = new List<AudioManager.EventHandler>();
    public AudioManager.EventHandler chosenEvent;

    [SerializeField] private GameObject particlePrefab;
    private GameObject particleObject;

    private ParticleSystem newParticleSystem;
    private ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
    private ParticleSystem.Particle[] currParticles;

    [SerializeField] private Transform particleTargetTrans;

    [SerializeField] private LayerMask blockLayerMask;

    private Vector3 particleObjOffset = new Vector3(0,-200,0);
    
    [SerializeField] private float soundDistance;

    public bool checkSounds = true;

    void Awake(){
        particleObject = GameObject.Find(particlePrefab.transform.name);
        if(particleObject == null) {
            particleObject = Instantiate(particlePrefab, particleObjOffset, Quaternion.identity);
        }

        newParticleSystem = particleObject.GetComponent<ParticleSystem>();

        var main = newParticleSystem.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        currParticles = new ParticleSystem.Particle[newParticleSystem.main.maxParticles];
    }

    void Update(){
        if(checkSounds){
            soundEvents = AudioManager.instance.CurrentEventsInRange(transform.position, soundDistance);

            if(soundEvents.Count != 0){
                chosenEvent = GetChosenEvent(soundEvents);
                MoveParticles();
            }
        }
        else{
            chosenEvent = null;
        }
    }

    private void MoveParticles(){
        foreach(AudioManager.EventHandler soundEvent in soundEvents){
            Ray particleRay = new Ray(soundEvent.position, particleTargetTrans.position - soundEvent.position);
            if(!Physics.Raycast(particleRay, out RaycastHit hit, Vector3.Distance(particleTargetTrans.position, soundEvent.position), blockLayerMask)){
                soundEvent.SendParticlesObject(particleTargetTrans.gameObject, soundDistance);
            }
        }
    }

    private AudioManager.EventHandler GetChosenEvent(List<AudioManager.EventHandler> events){
        AudioManager.EventHandler newChosenEvent = null;

        float highestImpact = 0;
        float lastEventTime = 0;

        foreach(AudioManager.EventHandler soundEvent in soundEvents){
            Ray particleRay = new Ray(soundEvent.position, particleTargetTrans.position - soundEvent.position);
            if(!Physics.Raycast(particleRay, out RaycastHit hit, Vector3.Distance(particleTargetTrans.position, soundEvent.position), blockLayerMask)){
                if(soundEvent.impact > highestImpact){
                    highestImpact = soundEvent.impact;
                    lastEventTime = soundEvent.time;
                    newChosenEvent = soundEvent;
                }
                else if(soundEvent.impact == highestImpact){
                    if(soundEvent.time > lastEventTime){
                        lastEventTime = soundEvent.time;
                        newChosenEvent = soundEvent;
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

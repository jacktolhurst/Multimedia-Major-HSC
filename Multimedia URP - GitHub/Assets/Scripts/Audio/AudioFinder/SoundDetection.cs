using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class SoundDetection : MonoBehaviour
{
    List<AudioManager.EventHandler> soundEvents = new List<AudioManager.EventHandler>();
    List<AudioManager.EventHandler> lastSoundEvents = new List<AudioManager.EventHandler>();
    public AudioManager.EventHandler chosenEvent;

    [SerializeField] private GameObject particlePrefab;
    private GameObject particleObject;

    private ParticleSystem soundParticles;
    private ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

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

        soundParticles = particleObject.GetComponent<ParticleSystem>();

        var main = soundParticles.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
    }

    void Update(){
        if(checkSounds){
            soundEvents = AudioManager.instance.CurrentEventsInRange(transform.position, soundDistance);

            if(soundEvents.Count != 0){
                float highestImpact = 0;
                float lastEventTime = 0;
                foreach(AudioManager.EventHandler soundEvent in soundEvents){
                    Ray particleRay = new Ray(soundEvent.position, particleTargetTrans.position - soundEvent.position);
                    if(!Physics.Raycast(particleRay, out RaycastHit hit, Vector3.Distance(particleTargetTrans.position, soundEvent.position), blockLayerMask)){
                        if(soundEvent.impact > highestImpact){
                            highestImpact = soundEvent.impact;
                            lastEventTime = soundEvent.time;
                            chosenEvent = soundEvent;
                        }
                        else if(soundEvent.impact == highestImpact){
                            if(soundEvent.time > lastEventTime){
                                lastEventTime = soundEvent.time;
                                chosenEvent = soundEvent;
                            }
                        }

                        if(!lastSoundEvents.Contains(soundEvent)){
                            for (int i = 0; i < soundEvent.impact; i++){
                                Vector3 eventPos = (soundEvent.position) + (Random.insideUnitSphere * soundEvent.particleOffset);
                                Vector3 direction = (particleTargetTrans.position - eventPos).normalized;
                                float distance = Vector3.Distance(particleTargetTrans.position, eventPos);
                                float particleSpeed = soundEvent.particleSpeed;

                                emitParams.position = eventPos;
                                emitParams.velocity = direction * particleSpeed;
                                emitParams.startLifetime = distance / particleSpeed;

                                soundParticles.Emit(emitParams, 1);
                            }
                        }
                    }
                    else{
                        chosenEvent = null;
                    }
                }
            }
            lastSoundEvents = soundEvents;
        }
        else{
            chosenEvent = null;
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

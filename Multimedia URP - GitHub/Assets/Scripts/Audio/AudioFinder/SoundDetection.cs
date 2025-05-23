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

    private ParticleSystem particleSystem;
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

        particleSystem = particleObject.GetComponent<ParticleSystem>();

        var main = particleSystem.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        currParticles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
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
                        Vector3 particleTargetPos = particleTargetTrans.position;
                        float particleSpeed = soundEvent.particleSpeed;

                        if(!lastSoundEvents.Contains(soundEvent)){
                            for (int i = 0; i < soundEvent.impact; i++){
                                Vector3 eventPos = (soundEvent.position) + (Random.insideUnitSphere * soundEvent.particleOffset);
                                Vector3 direction = (particleTargetPos - eventPos).normalized;
                                float distance = Vector3.Distance(particleTargetPos, eventPos);

                                emitParams.position = eventPos;
                                emitParams.velocity = direction * particleSpeed;
                                emitParams.startLifetime = distance / particleSpeed;

                                particleSystem.Emit(emitParams, 1);
                            }
                        }
                        else{
                            int count = particleSystem.GetParticles(currParticles);
                            for (int i = 0; i < count; i++){
                                currParticles[i].velocity = (particleTargetPos - currParticles[i].position).normalized * particleSpeed;
                            }

                            particleSystem.SetParticles(currParticles, count);
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

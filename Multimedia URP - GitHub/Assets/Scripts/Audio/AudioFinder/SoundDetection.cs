using UnityEngine;
using System.Collections.Generic;

public class SoundDetection : MonoBehaviour
{
    public AudioManager.EventHandler chosenEvent;

    private List<GameObject> currParticles = new List<GameObject>();

    [SerializeField] private Transform particleTargetTrans;

    [SerializeField] private LayerMask hitLayerMask;

    [SerializeField] private float soundDistance;
    [SerializeField] private float particleMoveSpeed;

    public bool checkSounds = true;

    void Update(){
        chosenEvent = GetChosenEvent();

        if(chosenEvent != null && chosenEvent.GetParticles() != null)currParticles.AddRange(chosenEvent.GetParticles());
    }

    void FixedUpdate(){
        UpdateParticlePosition();
    }

    private void UpdateParticlePosition(){
        List<GameObject> particlesToRemove = new List<GameObject>();
        foreach(GameObject particle in currParticles){
            if(particle == null){
                    particlesToRemove.Add(particle);
                    continue;
                }

            if(particle.activeSelf){
                NoteParticleManager particleManager = particle.GetComponent<NoteParticleManager>();
                Rigidbody particleRigidbody = particle.GetComponent<Rigidbody>();

                Vector3 direction = (particleTargetTrans.position - particle.transform.position).normalized;
                float distance = Vector3.Distance(particleTargetTrans.position, particle.transform.position);

                particleRigidbody.linearVelocity = direction * distance * particleMoveSpeed;
            }
            else{
                particlesToRemove.Add(particle);
            }
        }
        currParticles.RemoveAll(p => particlesToRemove.Contains(p));
    }

    private void ChangeParticleEndTime(AudioManager.EventHandler chosenEvent){
        foreach(GameObject particle in chosenEvent.GetParticles()){
            if(particle != null && particle.activeSelf){
                particle.GetComponent<NoteParticleManager>().SetEndTime(Time.time + (1f / particleMoveSpeed), false);
            }
        }
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

        if(newChosenEvent != null) ChangeParticleEndTime(newChosenEvent);

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

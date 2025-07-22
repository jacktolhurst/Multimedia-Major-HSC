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

        List<GameObject> newParticles = GetNewParticles();
        if(newParticles != null) currParticles.AddRange(newParticles);
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
            else if(particle.activeSelf){
                Vector3 particlePos = particle.transform.position;
                Rigidbody particleRigidbody = particle.GetComponent<Rigidbody>();
                NoteParticleManager particleManager = particle.GetComponent<NoteParticleManager>();

                if(!IsVector3Close(particlePos, particleTargetTrans.position, 0.01f)){
                    Vector3 direction = (particleTargetTrans.position - particlePos).normalized;

                    particleRigidbody.linearVelocity = direction * particleMoveSpeed;
                }
                else{
                    particleRigidbody.linearVelocity = Vector3.zero;
                    particleManager.SetEndTime(Time.time, false);
                    particlesToRemove.Add(particle);
                }
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
                float distance = Vector3.Distance(particleTargetTrans.position, particle.transform.position);
                particle.GetComponent<NoteParticleManager>().SetEndTime(Time.time + (distance/particleMoveSpeed), false);
            }
        }
    }

    private List<GameObject> GetNewParticles(){
        List<GameObject> newParticles = null;
        List<AudioManager.EventHandler> newCurrentEvents = AudioManager.instance.GetNewCurrentEventsInRange(transform.position, soundDistance);
        if(newCurrentEvents != null){
            if(newParticles == null){
                newParticles = new List<GameObject>();
            }
            foreach(AudioManager.EventHandler soundEvent in newCurrentEvents){
                newParticles.AddRange(soundEvent.GetParticles());
            }
        }

        return newParticles;
    }

    private AudioManager.EventHandler GetChosenEvent(){
        AudioManager.EventHandler newChosenEvent = null;

        if(checkSounds){
            List<AudioManager.EventHandler> soundEvents = AudioManager.instance.CurrentEventsInRange(transform.position, soundDistance);

            if(soundEvents != null){
                float highestImpact = 0;
                float lastEventTime = 0;
                foreach(AudioManager.EventHandler soundEvent in soundEvents){
                    Ray eventRay = new Ray(soundEvent.position, particleTargetTrans.position - soundEvent.position);
                    
                    if(!Physics.Raycast(eventRay, out RaycastHit hit, Vector3.Distance(particleTargetTrans.position, soundEvent.position),hitLayerMask)){
                            ChangeParticleEndTime(soundEvent);

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

    private bool IsVector3Close(Vector3 vecA, Vector3 vecB, float distance) {
        return Vector3.Distance(vecA, vecB) < distance;
    }

    void OnDrawGizmos(){
        if(checkSounds){
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, soundDistance);

            if(Application.isPlaying){  
                List<AudioManager.EventHandler> soundEvents = AudioManager.instance.CurrentEventsInRange(transform.position, soundDistance);
                if(soundEvents != null){
                    foreach(AudioManager.EventHandler soundEvent in soundEvents){
                        if(chosenEvent != null && soundEvent == chosenEvent){
                            Gizmos.color = Color.green;
                        }
                        else{
                            Gizmos.color = Color.red;
                        }
                        Gizmos.DrawWireSphere(soundEvent.position, soundEvent.impact);
                    }
                    if(chosenEvent != null){
                        Gizmos.color = Color.green;
                        Debug.DrawRay(transform.position, (chosenEvent.position - transform.position).normalized * Vector3.Distance(transform.  position, chosenEvent.position), Color.green);
                    }
                }
            }
        }
    }
}

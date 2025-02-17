using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{

    public class BPMBatchClass{
        public List<FMODEvents.SoundEventClass> soundEvents = new List<FMODEvents.SoundEventClass>();

        public float BPM;

        public IEnumerator StartSound(){
            while(true){
                foreach(FMODEvents.SoundEventClass soundEvent in soundEvents){
                    if(soundEvent.playNow){
                        RuntimeManager.PlayOneShot(soundEvent.eventReference, soundEvent.position);
                        if(!soundEvent.continuous){
                            soundEvent.playNow = false;
                        }
                    }
                }
                yield return new WaitForSeconds(60 / BPM);
            }
        }
    }

    public Dictionary<float, BPMBatchClass> bpmBatchClasseDict = new Dictionary<float, BPMBatchClass>();
    public Dictionary<float, Coroutine> bpmCoroutineDict = new Dictionary<float, Coroutine>();

    public static AudioManager instance {get; private set;}

    private List<EventInstance> eventInstances = new List<EventInstance>();

    void Awake(){
        if(instance != null){
            Debug.LogError("More then one audio manager");
        }
        instance = this;
    }

    void Start(){
        ReOrderList();
    }

    public void ChangeBPM(float prevBPM, FMODEvents.SoundEventClass soundEventClass){
        float newBPM = soundEventClass.BPM;

        bpmBatchClasseDict[prevBPM].soundEvents.Remove(soundEventClass);

        if(!bpmBatchClasseDict.ContainsKey(newBPM)){
            bpmBatchClasseDict.Add(newBPM, new BPMBatchClass());
            bpmBatchClasseDict[newBPM].BPM = newBPM;
            bpmCoroutineDict.Add(newBPM, StartCoroutine(bpmBatchClasseDict[newBPM].StartSound()));
        }
        bpmBatchClasseDict[newBPM].soundEvents.Add(soundEventClass);

        Debug.Log(bpmBatchClasseDict[newBPM].BPM);
        Debug.Log(bpmBatchClasseDict[newBPM].soundEvents[0].name);
    }

    private void ReOrderList(){
        foreach(FMODEvents.SoundEventClass soundEvent in FMODEvents.instance.soundEvents){
            if(!bpmBatchClasseDict.ContainsKey(soundEvent.BPM)){
                bpmBatchClasseDict.Add(soundEvent.BPM, new BPMBatchClass());
            }
            bpmBatchClasseDict[soundEvent.BPM].soundEvents.Add(soundEvent);
            bpmBatchClasseDict[soundEvent.BPM].BPM = soundEvent.BPM;
        }

        foreach(KeyValuePair<float, BPMBatchClass> keyValuePair in bpmBatchClasseDict){
            bpmCoroutineDict.Add(keyValuePair.Key, StartCoroutine(keyValuePair.Value.StartSound()));
        }
    }

    public FMODEvents.SoundEventClass GetSoundEventClass(string name){
        if(name == null || name.Length == 0){
            Debug.LogError("Sound name has nothing in it");
            return null;
        }
        foreach(FMODEvents.SoundEventClass soundEvent in FMODEvents.instance.soundEvents){
            if(soundEvent.name == name){
                return soundEvent;
            }
        }
        Debug.LogError("Sound event not found: " + name);
        return null;
    }

    // public void PlayOneShot(EventReference sound, Vector3 worldPos){
    //     RuntimeManager.PlayOneShot(sound, worldPos);
    // }

    // public EventInstance CreateInstance(EventReference eventReference){
    //     EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
    //     eventInstances.Add(eventInstance);
    //     return eventInstance;
    // }

    // private void CleanUp(){
    //     foreach(EventInstance eventInstance in eventInstances){ 
    //         eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    //         eventInstance.release();
    //     }
    // }

    // void OnDestroy(){
    //     CleanUp();
    // }
}

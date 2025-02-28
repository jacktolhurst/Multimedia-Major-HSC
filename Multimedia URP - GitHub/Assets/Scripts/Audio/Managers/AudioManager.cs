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
                    if( !soundEvent.dontPlay){
                        if(soundEvent.playNow){
                            if(soundEvent.continuous){
                                if(!instance.IsPlaying(soundEvent.eventInstance)){
                                    soundEvent.eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(soundEvent.position));
                                    soundEvent.eventInstance.start();
                                }
                            }
                            else{
                                soundEvent.eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(soundEvent.position));
                                soundEvent.eventInstance.start();
                                soundEvent.playNow = false;
                            }
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

    public void ChangeBPM(FMODEvents.SoundEventClass soundEventClass, float prevBPM){
        float newBPM = soundEventClass.BPM;

        bpmBatchClasseDict[prevBPM].soundEvents.Remove(soundEventClass);

        if (bpmBatchClasseDict[prevBPM].soundEvents.Count == 0){
            StopCoroutine(bpmCoroutineDict[prevBPM]);
            bpmBatchClasseDict.Remove(prevBPM);
            bpmCoroutineDict.Remove(prevBPM);
        }

        if(!bpmBatchClasseDict.ContainsKey(newBPM)){
            bpmBatchClasseDict.Add(newBPM, new BPMBatchClass());
            bpmBatchClasseDict[newBPM].BPM = newBPM;
            bpmCoroutineDict.Add(newBPM, StartCoroutine(bpmBatchClasseDict[newBPM].StartSound()));
        }
        bpmBatchClasseDict[newBPM].soundEvents.Add(soundEventClass);
    }

    public void StopSound(FMOD.Studio.EventInstance eventInstance){
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }

    public void StopAllSounds(){ 
        foreach(Coroutine coroutine in bpmCoroutineDict.Values){
            StopCoroutine(coroutine);
        }

        bpmBatchClasseDict.Clear();
        bpmCoroutineDict.Clear();


        FMOD.Studio.Bus masterBus;
        FMODUnity.RuntimeManager.StudioSystem.getBus("bus:/", out masterBus);
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ChangeMaxDistance(FMOD.Studio.EventInstance eventInstance, float maxDist){
        eventInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, maxDist);
    }

    public void ChangeMinDistance(FMOD.Studio.EventInstance eventInstance, float minDist){
        eventInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, minDist);
    }

    public void ChangeVolume(FMOD.Studio.EventInstance eventInstance, float volume){ 
        eventInstance.setVolume(volume);
    }
    
    public void AddVolumeToAllSounds(float volume){
        foreach(BPMBatchClass bpmClass in bpmBatchClasseDict.Values){
            foreach(FMODEvents.SoundEventClass eventClass in bpmClass.soundEvents){
                eventClass.ChangeVolume(Mathf.Max(0, eventClass.GetVolume() + volume));
            }
        }
    }

    public bool IsPlaying(FMOD.Studio.EventInstance eventInstance){
	    FMOD.Studio.PLAYBACK_STATE state;   
	    eventInstance.getPlaybackState(out state);
	    return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
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
}

using UnityEngine;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances;

    public static AudioManager instance { get; private set;}

    private void Awake(){
        if(instance != null){
            Debug.LogError("Found more then one audio manager in the scene.");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos){
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreatEventInstance(EventReference eventReference){
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    private void CleanUp(){
        foreach(EventInstance eventInstance in eventInstances){
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy(){
        CleanUp();
    }
}

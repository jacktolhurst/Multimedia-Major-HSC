using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance {get; private set;}

    private FMOD.Studio.Bus masterBus;

    void Awake(){
        if(instance != null){
            Debug.LogError("More then one audio manager");
        }
        instance = this;

        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos){
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public void StopAllSound(){
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void StopAllSoundFade(){
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}

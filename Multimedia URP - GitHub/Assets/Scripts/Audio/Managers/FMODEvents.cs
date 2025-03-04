using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [System.Serializable]
    public class SoundEventClass{
        public string name;
        
        [field: SerializeField] public EventReference eventReference {get; private set;}
        [HideInInspector] public FMOD.Studio.EventInstance eventInstance;

        [HideInInspector] public Vector3 position;

        public float BPM = 60;
        private float baseBPM;
        private float originalBPM;
        [Range(0,2)]
        public float volume = 1;
        [Range(1,10)]
        public float impact;

        public bool continuous;
        public bool dontPlay;

        public void Awake(){
            baseBPM = BPM;
            originalBPM = BPM;
            eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
        }

        public void Start(){
            ChangeVolume(volume);
        }

        public void Update(){
            BPM = baseBPM;
        }

        public void PlaySound(Vector3 newPosition){
            position = newPosition;
            AudioManager.instance.PlaySound(this);
        }

        public void StopSound(){
            position = Vector3.zero;
            AudioManager.instance.StopSound(this);
        }

        public float GetBPM(){
            return BPM;
        }

        public float GetOriginalBPM(){
            return originalBPM;
        }

        public void ChangeBPM(float newBPM){
            float prevBPM = BPM;
            baseBPM = newBPM;
            BPM = newBPM;
            AudioManager.instance.ChangeBPM(this, prevBPM);
        }

        public void ChangeMaxDistance(float maxDist){
            AudioManager.instance.ChangeMaxDistance(eventInstance, maxDist);
        }

        public void ChangeMinDistance(float minDist){
            AudioManager.instance.ChangeMinDistance(eventInstance, minDist);
        }

        public void ChangeVolume(float changedVolume){
            volume = changedVolume;
            AudioManager.instance.ChangeVolume(eventInstance,volume);
        }

        public float GetVolume(){ 
            return volume;
        }

        public bool IsPlaying(){
            return AudioManager.instance.IsPlaying(eventInstance);
        }
    }

    public List<SoundEventClass> soundEvents = new List<SoundEventClass>();

    public static FMODEvents instance {get; private set;}

    void Awake(){
        if(instance != null){
            Debug.LogError("More then one FMOD event manager");
        }
        instance = this;    

        foreach(SoundEventClass soundEvent in soundEvents){
            soundEvent.Awake();
        }
    }

    void Start(){    
        foreach(SoundEventClass soundEvent in soundEvents){
            soundEvent.Start();
        }
    }

    void Update(){
        foreach(SoundEventClass soundEvent in soundEvents){
            soundEvent.Update();
        }
    }
}

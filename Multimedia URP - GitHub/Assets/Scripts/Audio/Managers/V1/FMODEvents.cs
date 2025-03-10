using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [System.Serializable]
    public class SoundEventClass{
        public string name;
        
        [field: SerializeField] public EventReference eventReference {get; private set;}
        public List<FMOD.Studio.EventInstance> instances = new List<FMOD.Studio.EventInstance>();

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
        public bool haveMultiples;

        public void Awake(){
            baseBPM = BPM;
            originalBPM = BPM;
        }

        public void Start(){
        }

        public void Update(){
            BPM = baseBPM;
        }

        public void PlaySound(Vector3 newPosition){
            AudioManagerV1.instance.PlaySound(this);
        }

        public void StopSound(){
            position = Vector3.zero;
            AudioManagerV1.instance.StopSound(this);
        }

        public void ChangeBPM(float newBPM){
            float prevBPM = BPM;
            baseBPM = newBPM;
            BPM = newBPM;
            AudioManagerV1.instance.ChangeBPM(this, prevBPM);
        }

        public float GetBPM(){
            return BPM;
        }

        public float GetOriginalBPM(){
            return originalBPM;
        }

        public float GetVolume(){ 
            return volume;
        }

        public bool IsPlaying(){
            if(instances.Count == 0){
                return false;
            }
            else{
                return true;
            }
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

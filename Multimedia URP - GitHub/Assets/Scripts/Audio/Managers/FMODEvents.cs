using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [System.Serializable]
    public class SoundEventClass{
        public string name;
        
        [field: SerializeField] public EventReference eventReference {get; private set; }

        [HideInInspector] public Vector3 position;

        public float BPM;
        private float baseBPM;

        public bool continuous;
        [HideInInspector] public bool playNow;

        public void FirstUpdate(){
            baseBPM = BPM;
        }

        public void UpdateSound(){
            BPM = baseBPM;
        }

        public void PlaySound(Vector3 newPosition){
            position = newPosition;
            playNow = true;
        }

        public void StopSound(){
            position = Vector3.zero;
            playNow = false;
        }

        public float GetBPM(){
            return BPM;
        }

        public void ChangeBPM(float newBPM, SoundEventClass soundEventClass){
            float prevBPM = BPM;
            baseBPM = newBPM;
            BPM = newBPM;
            AudioManager.instance.ChangeBPM(prevBPM, soundEventClass);
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
            soundEvent.FirstUpdate();
        }
    }

    void Update(){
        foreach(SoundEventClass soundEvent in soundEvents){
            soundEvent.UpdateSound();
        }
    }
}

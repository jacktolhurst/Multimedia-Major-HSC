using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [System.Serializable]
    public class SoundEventClass{
        [field: SerializeField] public EventReference eventReference {get; private set; }

        [HideInInspector] public Vector3 position;

        public string name;

        public float BPM;

        public bool continuous;
        [HideInInspector] public bool playNow;

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

        public void ChangeBPM(float newBPM){
            BPM = newBPM;
            AudioManager.instance.ChangeBPM();
        }
    }

    public List<SoundEventClass> soundEvents = new List<SoundEventClass>();

    public static FMODEvents instance {get; private set;}

    void Awake(){
        if(instance != null){
            Debug.LogError("More then one FMOD event manager");
        }
        instance = this;
    }

    void OnValidate(){
        if(instance != null && AudioManager.instance.bpmBatchClasseDict.Count != 0){
            AudioManager.instance.ChangeBPM();
        }
    }
}

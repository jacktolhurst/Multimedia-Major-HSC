using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioReferenceClass{
        public EventReference eventReference;
        public List<EventHandler> eventHandlers = new List<EventHandler>();

        [Range(0,3)]
        public float volume;
        [Min(0)]
        public float maxDistance = 5;
        [Min(0)]
        public float minDistance = 1;
        [Min(0)]
        public int noteParticleLifetime = 1;
        [Min(0)]
        private float initializedTime;

        [Min(0)]
        public int impact = 1;

        private bool initialized;
        public bool dontUseSound;
        
        private IEnumerator UpdateLoop(){
            while(initialized){
                if(initializedTime + 60 <= Time.time && eventHandlers.Count == 0){
                    initialized = false;
                }
                yield return null;
            }
        }

        public void PlaySoundObject(GameObject obj){
            if(!dontUseSound){
                FMOD.Studio.EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
                eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(obj));

                Rigidbody objRb = obj.GetComponent<Rigidbody>();

                GameObject oldNoteParticleObj = AudioManager.instance.noteParticleObj;

                List<GameObject> newNoteParticleObjs = new List<GameObject>();
                for (int i = 0; i < impact; i++){
                    GameObject newNoteParticleObj = Instantiate(oldNoteParticleObj, obj.transform.position, oldNoteParticleObj.transform.rotation);

                    newNoteParticleObj.GetComponent<NoteParticleManager>().StartObj(obj, Time.time + noteParticleLifetime);
                    newNoteParticleObjs.Add(newNoteParticleObj);
                }

                PlaySoundMain(eventInstance, newNoteParticleObjs);
            }
        }

        public void PlaySoundPosition(Vector3 pos){
            if(!dontUseSound){
                FMOD.Studio.EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
                eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));

                GameObject oldNoteParticleObj = AudioManager.instance.noteParticleObj;

                List<GameObject> newNoteParticleObjs = new List<GameObject>();
                for (int i = 0; i < impact; i++){
                    GameObject newNoteParticleObj = Instantiate(oldNoteParticleObj, pos, oldNoteParticleObj.transform.rotation);

                    newNoteParticleObj.GetComponent<NoteParticleManager>().StartPosition(Time.time + noteParticleLifetime);
                    newNoteParticleObjs.Add(newNoteParticleObj);
                }

                PlaySoundMain(eventInstance, newNoteParticleObjs);
            }
        }

        public void PlaySoundMain(FMOD.Studio.EventInstance eventInstance, List<GameObject> newNoteParticleObjs){
            if(!initialized){
                initialized = true;
                initializedTime = Time.time;
                AudioManager.instance.StartCoroutine(UpdateLoop());
            }
            
            eventInstance.setVolume(volume);
            eventInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, minDistance);
            eventInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, maxDistance);

            eventInstance.start();

            Coroutine coroutine = AudioManager.instance.StartCoroutine(TrackSound(eventInstance));
            EventHandler eventHandler = new EventHandler(eventInstance, coroutine, newNoteParticleObjs, newNoteParticleObjs[0].GetComponent<NoteParticleManager>().endTime, impact, Time.time);
            AudioManager.instance.currentEvents.Add(eventHandler);
            eventHandlers.Add(eventHandler);

            eventInstance.release();
        }

        public void StopSound(){
            foreach(EventHandler eventHandler in eventHandlers){
                eventHandler.eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
            eventHandlers.Clear();
        }

        public void SetVolume(float newVolume){
            volume = Mathf.Clamp(newVolume, 0, 3);
        }

        public void SetMaxDistance(float newMaxDistance){
            maxDistance = Mathf.Max(newMaxDistance, minDistance);
        }

        public void SetMinDistance(float newMinDistance){
            minDistance = Mathf.Clamp(newMinDistance, 0, maxDistance);
        }

        public void SetImpact(int newImpact){
            impact = Mathf.Max(newImpact, 0);
        }

        public void SetParameterInt(string name, int value){
            eventHandlers[0].eventInstance.setParameterByName(name, value);
        }

        public bool IsPlaying(){
            if(eventHandlers.Count != 0 ){
                return true;
            }
            else{
                return false;
            }
        }

        private IEnumerator TrackSound(FMOD.Studio.EventInstance eventInstance){
            FMOD.Studio.PLAYBACK_STATE state = FMOD.Studio.PLAYBACK_STATE.PLAYING;

            yield return null;

            EventHandler eventHandler = null;
            foreach (EventHandler checkEvent in AudioManager.instance.currentEvents){
                if (checkEvent.eventInstance.Equals(eventInstance)){
                    eventHandler = checkEvent;
                    break;
                }
            }

            while (state != FMOD.Studio.PLAYBACK_STATE.STOPPED || eventHandler.noteParticleEndTime > Time.time){
                eventInstance.getPlaybackState(out state);
                yield return null;
            }

            eventHandlers.Remove(eventHandler);
            AudioManager.instance.currentEvents.Remove(eventHandler);
        }
    }

    public class EventHandler{
        public FMOD.Studio.EventInstance eventInstance;
        public Coroutine activeCoroutine;
        public List<GameObject> noteParticleObjs = new List<GameObject>();
        public Vector3 position;
        public float noteParticleEndTime;
        public float impact;
        public float time;

        public EventHandler(FMOD.Studio.EventInstance newEventInstance, Coroutine newActiveCoroutine, List<GameObject> newNoteParticleObjs, float newNoteParticleEndTime, float newImpact, float newTime){
            eventInstance = newEventInstance;
            activeCoroutine = newActiveCoroutine;
            noteParticleObjs = newNoteParticleObjs; 
            noteParticleEndTime = newNoteParticleEndTime;
            impact = newImpact;
            time = newTime;

            position = AudioManager.instance.GetEventInstancePosition(eventInstance);
        }

        public void Update(){
            Vector3 newPosition = AudioManager.instance.GetEventInstancePosition(eventInstance);
            if(newPosition != Vector3.zero){
                position = newPosition;
            }
        }
    }

    public static AudioManager instance;

    private List<EventHandler> currentEvents = new List<EventHandler>();

    private FMOD.Studio.Bus masterBus;

    [SerializeField] private GameObject noteParticleObj;

    [SerializeField] private int soundCount;

    void Awake() {
        if(instance != null){
            Debug.LogWarning("Two AudioManager instances");
        }
        instance = this;

        FMODUnity.RuntimeManager.StudioSystem.getBus("bus:/", out masterBus);
    }

    void Update() {
        soundCount = currentEvents.Count;

        foreach(EventHandler currentEvent in currentEvents){
            currentEvent.Update();
        }
    }

    public void ChangeAllVolume(float rawVolume) {
        float minVolume = 0.01f;
        float maxVolume = 1;
        rawVolume = Mathf.Clamp(rawVolume, minVolume, maxVolume);
        float volume = Mathf.Pow(rawVolume, 2.2f);
        masterBus.setVolume(volume);
    }

    public List<EventHandler> GetCurrentEvents(){
        return currentEvents;
    }

    public List<EventHandler> CurrentEventsInRange(Vector3 position, float range){
        List<EventHandler> currentEventsInRange = new List<EventHandler>();
        foreach(EventHandler currentEvent in currentEvents){
            if(Vector3.Distance(position, currentEvent.position) <= range){
                currentEventsInRange.Add(currentEvent);
            }
        }
        return currentEventsInRange;
    }

    public Vector3 GetEventInstancePosition(EventInstance instance){
        FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D();
        
        instance.get3DAttributes(out attributes);

        return new Vector3(attributes.position.x, attributes.position.y, attributes.position.z);
    }
}

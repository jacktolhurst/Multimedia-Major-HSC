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
        private float prevVolume;
        [Min(0)]
        public float maxDistance;
        private float prevMaxDistance;
        [Min(0)]
        public float minDistance;
        private float prevMinDistance;
        public float impact;
        private float prevImpact;
        private float initializedTime;

        private bool initialized;
        public bool dontUseSound;
        
        private IEnumerator UpdateLoop(){
            while(initialized){
                if(prevVolume != volume || prevMaxDistance != maxDistance || prevMinDistance != minDistance || prevImpact != impact){
                    foreach(EventHandler eventHandler in eventHandlers){
                        if(prevVolume != volume){
                            eventHandler.eventInstance.setVolume(volume);
                            prevVolume = volume;
                        }
                        if(prevMinDistance != minDistance){
                            eventHandler.eventInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, minDistance);
                            prevMinDistance = minDistance;
                        }
                        if(prevMaxDistance != maxDistance ){
                            eventHandler.eventInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, maxDistance);
                            prevMaxDistance = maxDistance;
                        }
                        if(prevImpact != impact){
                            eventHandler.impact = impact;
                            prevImpact = impact;
                        }
                    }
                }

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
                PlaySoundMain(eventInstance);
            }
        }

        public void PlaySoundPosition(Vector3 pos){
            if(!dontUseSound){
                FMOD.Studio.EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
                eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
                PlaySoundMain(eventInstance);
            }
        }

        public void PlaySoundMain(FMOD.Studio.EventInstance eventInstance){
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
            EventHandler eventHandler = new EventHandler(eventInstance, coroutine, impact, Time.time);
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
            minDistance = Mathf.Clamp(minDistance, 0, maxDistance);
        }

        public void SetImpact(float newImpact){
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

            while (state != FMOD.Studio.PLAYBACK_STATE.STOPPED){
                eventInstance.getPlaybackState(out state);
                yield return null;
            }

            EventHandler eventHandlerToRemove = null;
            foreach(EventHandler eventHandler in AudioManager.instance.currentEvents){
                if(eventHandler.eventInstance.Equals(eventInstance)){
                    eventHandlerToRemove = eventHandler;
                }   
            }
            eventHandlers.Remove(eventHandlerToRemove);
            AudioManager.instance.currentEvents.Remove(eventHandlerToRemove);
        }
    }

    public class EventHandler{
        public FMOD.Studio.EventInstance eventInstance;
        public Coroutine activeCoroutine;
        public Vector3 position;
        public float impact;
        public float time;

        public EventHandler(FMOD.Studio.EventInstance newEventInstance, Coroutine newActiveCoroutine, float newImpact, float newTime){
            eventInstance = newEventInstance;
            activeCoroutine = newActiveCoroutine;
            impact = newImpact;
            time = newTime;
        }

        public void Update(){
            Vector3 newPosition =  AudioManager.instance.GetEventInstancePosition(eventInstance);
            if(newPosition != Vector3.zero){
                position = newPosition;
            }
        }
    }

    public static AudioManager instance;

    private List<EventHandler> currentEvents = new List<EventHandler>();

    private FMOD.Studio.Bus masterBus;

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

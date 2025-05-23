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
        public float volume = 1;
        private float prevVolume;
        [Min(0)]
        public float maxDistance = 10;
        private float prevMaxDistance;
        [Min(0)]
        public float minDistance = 1;
        private float prevMinDistance;
        private float initializedTime;
        [Min(0)]
        public float particleSpeed = 8;
        private float prevParticleSpeed;
        [Min(0)]
        public float particleOffset = 1;
        private float prevParticleOffset;

        [Min(0)]
        public int impact;
        private int prevImpact;

        private bool initialized;
        public bool dontUseSound;
        
        private IEnumerator UpdateLoop(){
            while(initialized){
                if(prevVolume != volume || prevMaxDistance != maxDistance || prevMinDistance != minDistance || prevImpact != impact | prevParticleSpeed != particleSpeed){
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

                        if(prevParticleSpeed != particleSpeed){
                            eventHandler.particleSpeed = particleSpeed;
                            prevParticleSpeed = particleSpeed;
                        }
                        if(prevParticleOffset != particleOffset){
                            eventHandler.particleOffset = particleOffset;
                            prevParticleOffset = particleOffset;
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
            EventHandler eventHandler = new EventHandler(this, eventInstance, coroutine, Time.time, particleSpeed, particleOffset, impact);
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
        public AudioManager.AudioReferenceClass referenceClass;
        public FMOD.Studio.EventInstance eventInstance;
        public Coroutine activeCoroutine;
        public Vector3 position;
        public float time;
        public float particleSpeed;
        public float particleOffset;
        public int impact;

        public EventHandler(AudioManager.AudioReferenceClass newReferenceClass, FMOD.Studio.EventInstance newEventInstance, Coroutine newActiveCoroutine, float newTime, float newParticleSpeed, float newParticleOffset, int newImpact){
            referenceClass = newReferenceClass;
            eventInstance = newEventInstance;
            activeCoroutine = newActiveCoroutine;
            time = newTime;
            particleSpeed = newParticleSpeed;
            particleOffset = newParticleOffset;
            impact = newImpact;

            FMOD.ATTRIBUTES_3D attributes;
            eventInstance.get3DAttributes(out attributes);
            position = new Vector3(attributes.position.x, attributes.position.y, attributes.position.z);
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

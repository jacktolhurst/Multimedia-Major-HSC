using UnityEngine;
using System.Collections.Generic;

public class TowerController : MonoBehaviour
{
    [SerializeField] private LeverManager leverManager;
    private LeverManager.Lever lever;

    private Animator pipeAnimator;

    [SerializeField] private List<GameObject> speakers;
    [SerializeField] private GameObject bigSpeaker;
    [SerializeField] private GameObject pipe;

    [SerializeField] private float health;
    [SerializeField] private float leverDamage;

    [SerializeField] private string pipeAnimationName;

    [SerializeField] private AudioManager.AudioReferenceClass spinningSound;
    [SerializeField] private AudioManager.AudioReferenceClass speakersSound;

    private bool playingAnimation;

    void Awake(){
        lever = leverManager.GetLeverAtIndex(1);

        pipeAnimator = GetComponent<Animator>();
    }


    void Update(){
        if(lever.isRotating){
            health -= leverDamage*Time.deltaTime;
        }

        if(!playingAnimation && health <= 0){
            pipeAnimator.Play(pipeAnimationName);

            Quaternion prevRotation = lever.obj.transform.rotation;
            lever.obj.GetComponent<HingeJoint>().breakForce = 0;
            lever.obj.transform.rotation = prevRotation;

            spinningSound.PlaySoundObject(pipe);

            SpawnParticles(speakers, bigSpeaker);

            playingAnimation = true;
        }
    }

    private void SpawnParticles(List<GameObject> objs, GameObject mainObj){
        foreach(GameObject obj in objs){
            if(obj == mainObj) speakersSound.PlaySoundObject(mainObj);
            else speakersSound.SpawnParticlesObject(obj);
        }
    }
}

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

    [SerializeField] private AudioManager.AudioReferenceClass startSound;
    [SerializeField] private AudioManager.AudioReferenceClass spinningSound;

    private bool playingAnimation;

    void Awake(){
        lever = leverManager.GetLeverAtIndex(1);

        pipeAnimator = GetComponent<Animator>();
    }


    void Update(){
        if(lever.isRotating){
            health -= leverDamage;
        }

        if(!playingAnimation && health <= 0){
            pipeAnimator.Play(pipeAnimationName);

            Quaternion prevRotation = lever.obj.transform.rotation;
            lever.obj.GetComponent<HingeJoint>().breakForce = 0;
            lever.obj.transform.rotation = prevRotation;

            startSound.PlaySoundObject(pipe);
            spinningSound.PlaySoundObject(pipe);

            playingAnimation = true;
        }
    }
}

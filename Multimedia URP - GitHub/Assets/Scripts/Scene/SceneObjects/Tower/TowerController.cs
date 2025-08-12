using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] private LeverManager leverManager;
    private LeverManager.Lever lever;

    private Animator pipeAnimator;

    [SerializeField] private float health;
    [SerializeField] private float leverDamage;

    [SerializeField] private string pipeAnimationName;

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

            lever.obj.GetComponent<HingeJoint>().breakForce = 0;

            playingAnimation = true;
        }
    }


}

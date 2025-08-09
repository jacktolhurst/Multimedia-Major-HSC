using UnityEngine;

public class RobotDogManager : MonoBehaviour
{
    private LeverManager leverManager;

    private LeverManager.Lever lever;

    [SerializeField] private float health;
    [SerializeField] private float leverDamage;

    [SerializeField] private string leverName;


    void Awake(){
        leverManager = GetComponent<LeverManager>();

        lever = leverManager.GetLeverByName(leverName);
    }

    void Update(){
        if(lever.isRotating){
            health -= leverDamage*Time.deltaTime;
        }
    }
}

using UnityEngine;

public class RobotDogManager : MonoBehaviour
{
    [SerializeField] private LeverManager leverManager;

    private LeverManager.Lever lever;

    [SerializeField] private int health;

    [SerializeField] private string leverName;


    void Awake(){
        lever = leverManager.GetLeverByName(leverName);
    }

    void Update(){
        if(lever.isRotating){
            health -= 1;
        }
    }
}

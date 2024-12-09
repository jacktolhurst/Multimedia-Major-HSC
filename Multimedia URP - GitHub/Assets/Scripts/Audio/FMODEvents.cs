using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootSteps {get; private set;}

    [field: Header("Generaton SFX")]
    [field: SerializeField] public EventReference generationKick {get; private set;}

    public static FMODEvents instance { get; private set;}

    private void Awake(){
        if(instance != null){
            Debug.LogError("Found more then one FMOD events scripts in the scene.");
        }
        instance = this;
    }
}

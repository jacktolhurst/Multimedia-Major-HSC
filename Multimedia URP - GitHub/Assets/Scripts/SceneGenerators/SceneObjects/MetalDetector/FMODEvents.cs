using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Metal Detector SFX")]
    [field: SerializeField] public EventReference metalDetectorSFX {get; private set; }

    public static FMODEvents instance {get; private set;}

    void Awake(){
        if(instance != null){
            Debug.LogError("More then one FMOD event manager");
        }
        instance = this;
    }
}

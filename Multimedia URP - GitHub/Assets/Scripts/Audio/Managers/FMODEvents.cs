using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Metal Detector SFX")]
    [field: SerializeField] public EventReference metalDetectorSFX {get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference footStepsSFX {get; private set; }

    public static FMODEvents instance {get; private set;}

    void Awake(){
        if(instance != null){
            Debug.LogError("More then one FMOD event manager");
        }
        instance = this;
    }
}

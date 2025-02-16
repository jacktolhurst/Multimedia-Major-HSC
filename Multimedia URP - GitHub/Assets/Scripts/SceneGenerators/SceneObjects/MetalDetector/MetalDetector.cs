using UnityEngine;

public class MetalDetector : MonoBehaviour
{
    [SerializeField] private string SFXName;

    private FMODEvents.SoundEventClass metalDetectorSFX;

    void Start(){
        metalDetectorSFX = AudioManager.instance.GetSoundEventClass(SFXName);
    }

    void OnTriggerEnter(Collider other) {
        metalDetectorSFX.PlaySound(transform.position);
	}
}

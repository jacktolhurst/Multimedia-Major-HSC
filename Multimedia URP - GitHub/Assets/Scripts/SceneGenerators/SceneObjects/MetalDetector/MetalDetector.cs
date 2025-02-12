using UnityEngine;

public class MetalDetector : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.metalDetectorSFX, transform.position);
	}
}

using UnityEngine;

public class MetalDetector : MonoBehaviour
{
    [SerializeField] private AudioManager.AudioReferenceClass metalDetectorSound;

    void OnTriggerEnter(Collider other) {
        metalDetectorSound.PlaySoundPosition(transform.position);
	}
}

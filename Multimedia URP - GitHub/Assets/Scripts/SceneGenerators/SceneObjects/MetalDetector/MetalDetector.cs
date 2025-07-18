using UnityEngine;

public class MetalDetector : MonoBehaviour
{
    [SerializeField] private AudioManager.AudioReferenceClass metalDetectorSound;

    void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.layer != 0 && collider.gameObject.layer != 13) metalDetectorSound.PlaySoundObject(transform.gameObject);
	}
}

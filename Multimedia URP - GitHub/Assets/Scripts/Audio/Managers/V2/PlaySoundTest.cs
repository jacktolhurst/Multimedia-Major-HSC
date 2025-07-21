using UnityEngine;

public class PlaySoundTest : MonoBehaviour
{
    [SerializeField] private AudioManager.AudioReferenceClass sound;

    [SerializeField] private bool playSound;

    void Update(){
        if(playSound){
            sound.PlaySoundPosition(transform.position);
            playSound = false;
        }
    }
}

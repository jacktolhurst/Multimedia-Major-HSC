using UnityEngine;

public class PlaySoundTest : MonoBehaviour
{
    [SerializeField] private AudioManager.AudioReferenceClass sound;

    [SerializeField] private bool playSound;

    void Update(){
        if(playSound){
            sound.PlaySoundObject(gameObject);
            playSound = false;
        }
    }
}

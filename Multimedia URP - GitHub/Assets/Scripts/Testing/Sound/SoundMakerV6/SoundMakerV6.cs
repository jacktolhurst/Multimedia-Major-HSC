using UnityEngine;

public class SoundMakerV6 : MonoBehaviour
{
    private AudioClip myClip;

    void Awake(){
        myClip = AudioClip.Create("GeneratedClip", 44100, 1, 44100, false, OnAudioRead);
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = myClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    void OnAudioRead(float[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            // Fill data with samples, for example, creating a sine wave
            data[i] = Mathf.Sin(2 * Mathf.PI * i / 44100);
        }
    }
}

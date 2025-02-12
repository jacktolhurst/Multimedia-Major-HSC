using UnityEngine;

public class SoundMakerV4 : MonoBehaviour
{
    [SerializeField, Range(0, 1)] float amplitude;
    [SerializeField] float frequency;
    [SerializeField, Range(0, 1)] float timeToNote;

    private int sampleRate;
    private double phase;

    void Awake(){
        sampleRate = AudioSettings.outputSampleRate;
    }

    void Update(){
        if(Input.GetKey("m")){
            frequency = Mathf.Lerp(frequency, 277.183f, timeToNote);
        }
        else if(Input.GetKey("n")){
            frequency = Mathf.Lerp(frequency, 138.591f, timeToNote);
        }
        else if(Input.GetKey("b")){
            frequency = Mathf.Lerp(frequency, 69.296f, timeToNote);
        }
        else{
            frequency = Mathf.Lerp(frequency, 0, timeToNote);
        }
    }

    private void OnAudioFilterRead(float[] data, int channels){
        double phaseIncrement = frequency / sampleRate;

        for(int sample = 0; sample < data.Length; sample += channels){
            if(frequency != 0){
                float value = SineWave(phase, amplitude);

                phase = (phase + phaseIncrement) % 1;

                for (int channel = 0; channel < channels; channel++){
                    data[sample + channel] = value;
                }
            }
        }
    }


    float SineWave(double phase, float amplitude){
        return Mathf.Sin((float)phase * 2 * Mathf.PI) * amplitude;
    }
}

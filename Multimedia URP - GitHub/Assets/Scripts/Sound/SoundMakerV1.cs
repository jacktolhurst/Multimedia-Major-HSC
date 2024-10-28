using UnityEngine;

public class SoundMakerV1 : MonoBehaviour
{
    [SerializeField, Range(0, 1)] float amplitude = 0.5f;
    [SerializeField] float sineFrequency = 261.62f;     // Frequency for sine wave
    [SerializeField] float sawToothFrequency = 329.63f; // Frequency for sawtooth wave

    double sinePhase;    // Phase for the sine wave
    double sawPhase;     // Phase for the sawtooth wave
    int _sampleRate;

    void Awake()
    {
        _sampleRate = AudioSettings.outputSampleRate;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        double sinePhaseIncrement = sineFrequency / _sampleRate;
        double sawPhaseIncrement = sawToothFrequency / _sampleRate;

        for (int sample = 0; sample < data.Length; sample += channels)
        {
            // Calculate the values for both waveforms
            float sineValue = SineWave(sinePhase, amplitude);
            float sawToothValue = SawTooth(sawPhase, amplitude);

            // Mix the two waveforms
            float value = (sineValue + sawToothValue) / 2;
            
            // Increment the phases for the next iteration
            sinePhase = (sinePhase + sinePhaseIncrement) % 1;
            sawPhase = (sawPhase + sawPhaseIncrement) % 1;

            // Populate all channels with the mixed value
            for (int channel = 0; channel < channels; channel++)
            {
                data[sample + channel] = value;
            }
        }
    }

    float SineWave(double phase, float amplitude)
    {
        return Mathf.Sin((float)phase * 2 * Mathf.PI) * amplitude;
    }

    float SawTooth(double phase, float amplitude)
    {
        return (float)((phase * 2) - 1) * amplitude;
    }
}

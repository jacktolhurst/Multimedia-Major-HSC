using UnityEngine;

public class SoundMakerV3 : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float amplitude = 0.5f;
    [SerializeField] private float baseFrequency = 261.62f; // Base frequency for sine wave
    [SerializeField] private float frequencyRange = 440.0f; // Max frequency for sawtooth wave

    private double _phase;
    private int _sampleRate;

    void Awake()
    {
        _sampleRate = AudioSettings.outputSampleRate;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        double phaseIncrement = 1.0 / _sampleRate; // Increment for the sawtooth wave

        for (int sample = 0; sample < data.Length; sample += channels)
        {
            // Get the sawtooth wave value and map it to a frequency range
            float sawValue = SawTooth(_phase, 1.0f); // Sawtooth range [-1, 1]
            float mappedFrequency = baseFrequency + (sawValue * (frequencyRange - baseFrequency)); // Map to frequency range
            
            // Calculate phase increment based on the current frequency
            double currentPhaseIncrement = mappedFrequency / _sampleRate;

            float value = SineWave(_phase, amplitude);

            _phase = (_phase + currentPhaseIncrement) % 1;

            for (int channel = 0; channel < channels; channel++)
            {
                data[sample + channel] = value;
            }
        }

        // Update phase for the sawtooth wave
        _phase = (_phase + phaseIncrement) % 1;
    }

    float SineWave(double phase, float amplitude)
    {
        return Mathf.Sin((float)phase * 2 * Mathf.PI) * amplitude;
    }

    float SawTooth(double phase, float amplitude)
    {
        return (float)((phase * 2) - 1) * amplitude;
    }

    float SquareWave(double phase, float amplitude)
    {
        return phase < 0.5 ? amplitude : -amplitude;
    }
}

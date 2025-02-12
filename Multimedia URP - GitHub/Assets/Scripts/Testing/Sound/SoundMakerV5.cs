using UnityEngine;

public class SoundMakerV5 : MonoBehaviour
{
    public float baseFrequency = 440f; // Base frequency of the sine wave in Hz
    public float modulationFrequency = 1f; // Frequency of the sawtooth modulator in Hz
    public float modulationDepth = 100f;   // How much the modulator affects the pitch
    public float amplitude = 0.5f;         // Volume of the sine wave

    private float sampleRate;              // Sample rate of the audio (e.g., 44100 Hz)
    private float phase;                   // Phase for the sine wave
    private float modulationPhase;         // Phase for the sawtooth wave

    void Start()
    {
        sampleRate = AudioSettings.outputSampleRate; // Get system sample rate
        phase = 0f;
        modulationPhase = 0f;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        // Iterate through each sample
        for (int i = 0; i < data.Length; i += channels)
        {
            // Generate a sawtooth wave to modulate the frequency (from -1 to 1)
            float sawtoothWave = 2f * (modulationPhase - Mathf.Floor(modulationPhase + 0.5f));

            // Use the sawtooth wave to modulate the sine wave frequency
            float modulatedFrequency = baseFrequency + sawtoothWave * modulationDepth;

            // Generate the sine wave with the modulated frequency
            float sineWave = Mathf.Sin(2 * Mathf.PI * modulatedFrequency * phase);

            // Apply the sine wave to all audio channels (stereo)
            for (int channel = 0; channel < channels; channel++)
            {
                data[i + channel] = sineWave * amplitude;
            }

            // Increment the phase of the sine wave (wrap phase if necessary)
            phase += modulatedFrequency / sampleRate;
            if (phase > 1f) phase -= 1f;

            // Increment the phase of the sawtooth wave (wrap phase if necessary)
            modulationPhase += modulationFrequency / sampleRate;
            if (modulationPhase > 1f) modulationPhase -= 1f;
        }
    }
}

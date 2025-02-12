using UnityEngine;
using System.Collections.Generic;  

[System.Serializable]
public class WaveformV2{
    public SoundMakerV2.WaveformType waveformType;
    [Range(0, 1)] public float amplitude = 0.5f;
    public float frequency = 261.62f;
}

public class SoundMakerV2 : MonoBehaviour{
    public enum WaveformType{
        Sine,
        Sawtooth,
        Square 
    }

    [SerializeField] private List<WaveformV2> waveforms = new List<WaveformV2>();

    private int _sampleRate;
    private double[] phases;

    void Awake(){
        _sampleRate = AudioSettings.outputSampleRate;
        phases = new double[waveforms.Count]; 
    }

    void UpdatePhasesArray(){
        if (phases == null || phases.Length != waveforms.Count)
        {
            phases = new double[waveforms.Count];
        }
    }

    void OnAudioFilterRead(float[] data, int channels){
        UpdatePhasesArray();
        
        for (int i = 0; i < data.Length; i++){
            data[i] = 0;
        }

        for (int w = 0; w < waveforms.Count; w++){
            WaveformV2 waveform = waveforms[w];
            double phaseIncrement = (waveform.frequency * 2 * Mathf.PI) / _sampleRate;

            for (int sample = 0; sample < data.Length; sample += channels){
                float value = 0;

                switch (waveform.waveformType){
                    case WaveformType.Sine:
                        value = SineWave(phases[w], waveform.amplitude);
                        break;
                    case WaveformType.Sawtooth:
                        value = SawTooth(phases[w], waveform.amplitude);
                        break;
                    case WaveformType.Square:
                        value = SquareWave(phases[w], waveform.amplitude);
                        break;
                }

                phases[w] = (phases[w] + phaseIncrement) % 1;

                for (int channel = 0; channel < channels; channel++)
                {
                    data[sample + channel] += value / waveforms.Count;
                }
            }
        }
    }

    float SineWave(double phase, float amplitude){
        return Mathf.Sin((float) phase * 2 * Mathf.PI) * amplitude;
    }

    float SawTooth(double phase, float amplitude){
        return (float)((phase * 2) - 1) * amplitude;
    }

    float SquareWave(double phase, float amplitude) {
        return phase < 0.5 ? amplitude : -amplitude;
    }   
}

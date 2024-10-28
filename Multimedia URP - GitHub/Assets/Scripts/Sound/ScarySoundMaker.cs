using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScarySoundMaker : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float baseAmplitude = 0.5f;
    [SerializeField, Range(0, 1)] private float amplitude;
    [SerializeField] private float frequency;
    [SerializeField] private int maxFrequency = 600;
    [SerializeField] private int amplitudeChange;

    private int _sampleRate;
    private int frequencyListAmount = 50;
    private int randomFrequency;
    private int frequencyThreshold = 5;
    private float changedAmplitude;
    private float frequencyChangeTime = 0.1f;
    private float amplitudeChangeTime = 0.1f;
    private float lastFrequency;
    private double _phase;

    private List<int> randomFrequencies = new List<int>();
    private List<float> changedAmplitudes = new List<float>();
    
    private AudioSource source;

    private void Start(){
        source = GetComponent<AudioSource>();
    }

    private void Awake(){
        amplitude = baseAmplitude;

        _sampleRate = AudioSettings.outputSampleRate;

        for (int i = 0; i < frequencyListAmount; i++){
            AddFrequencies();
        }
        randomFrequency = randomFrequencies[0];
        randomFrequencies.RemoveAt(0);

        changedAmplitude = changedAmplitudes[0];
        changedAmplitudes.RemoveAt(0);

        StartCoroutine(ChangeFrequencyEvery4Seconds());
    }

    private void FixedUpdate(){
        frequency = Mathf.Lerp(frequency, randomFrequency, frequencyChangeTime);
        amplitude = Mathf.Clamp(Mathf.Lerp(amplitude, changedAmplitude, amplitudeChangeTime), 0f, 1f);

        CheckFrequencies();

        while(randomFrequencies.Count < frequencyListAmount){
            AddFrequencies();
        }

        if(source.pitch != 1){
            source.pitch = 1;
        }

        lastFrequency = frequency;
    }

    private void OnAudioFilterRead(float[] data, int channels){
        double phaseIncrement = frequency / _sampleRate;

        for (int sample = 0; sample < data.Length; sample += channels){
            float value = Mathf.Sin((float)_phase * 2 * Mathf.PI) * amplitude;
            _phase = (_phase + phaseIncrement) % 1;

            for (int channel = 0; channel < channels; channel++){
                data[sample + channel] = value;
            }
        }
    }

    private IEnumerator ChangeFrequencyEvery4Seconds(){
        while (true){
            int randomValue = Random.Range(1 ,6);
            yield return new WaitForSeconds(randomValue);
            ChangePitch(randomValue);
            
            randomFrequency = randomFrequencies[0];
            randomFrequencies.RemoveAt(0);

            changedAmplitude = changedAmplitudes[0];
            changedAmplitudes.RemoveAt(0);
        }
    }

    private void ChangePitch(int value){
        source.pitch = Mathf.Clamp(value, 0, 3) - Mathf.Max(Random.Range(-1,3), 0);
    }

    private void AddFrequencies(){
        int randomNumer = Random.Range(50, maxFrequency);

        randomFrequencies.Add(randomNumer);
        changedAmplitudes.Add(baseAmplitude - (float)randomNumer / (float)amplitudeChange);
    }

    private void CheckFrequencies(){
        if(Mathf.Abs(lastFrequency - frequency) <= frequencyThreshold){
            frequency += Random.Range(-50,50);
        }
    }
}

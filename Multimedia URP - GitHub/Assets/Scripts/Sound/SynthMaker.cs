using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthMaker : MonoBehaviour
{
    public double frequency = 440;
    private double increment;
    private double phase;
    private double sampleRate;

    public float gain;
    public float volume = 0.01f;

    public float[] frequencies;
    public int thisFreq;

    private AudioSource source;

    public float amplitudeMod;


    void Start(){
        frequencies = new float[8];
        frequencies[0] = 440;
        frequencies[1] = 494;
        frequencies[2] = 554;
        frequencies[3] = 587;
        frequencies[4] = 659;
        frequencies[5] = 740;
        frequencies[6] = 831;
        frequencies[7] = 880;

        source = GetComponent<AudioSource>();
    }

    void Awake(){
        sampleRate = AudioSettings.outputSampleRate;
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            gain = volume;
            frequency = frequencies[thisFreq];
            thisFreq += 1;
            thisFreq = thisFreq % frequencies.Length;
            source.volume = volume + Mathf.Sin(Time.time * (float)frequency) * amplitudeMod;
        }
        if (Input.GetKeyUp(KeyCode.Space)){
            gain = 0;
        }
    }

    void OnAudioFilterRead(float[] data, int channels){
        increment = frequency * 2 * Mathf.PI / sampleRate;

        for(int i = 0; i < data.Length; i+= channels){
            phase += increment;
            data[i] = (float) (gain * Mathf.Sin((float)phase));

            if(channels == 2){
                data[i + 1] = data[i];
            }

            if(phase > (Mathf.PI * 2)){
                phase = 0.0;
            }
        }
    }
}
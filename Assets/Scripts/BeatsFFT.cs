using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatsFFT : MonoBehaviour
{
    public AudioSource audioSource;
    float[] asamples = new float[64];
    public float avgFreq = 0.0f;
    public float runningAvgFreq = 0.0f;
    public float scaledAvgFreq = 0.0f;
    float alpha = 0.2f; //lowpass filter positions for smooth movement, lower number for more smoothing
    public float maxFreq = float.MinValue;
    public float minFreq = float.MaxValue;

    void Start()
    {

    }

    void Update()
    {
        GetSpectrumAudioSource();
    }

    void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(asamples, 0, FFTWindow.Blackman);

        avgFreq = 0.0f;
        for (int i = 0; i < asamples.Length; i++)
        {
            avgFreq = avgFreq + asamples[i];
        }
        avgFreq *= avgFreq;
        avgFreq = avgFreq / asamples.Length;
        if (avgFreq > maxFreq)
        {
            maxFreq = avgFreq;
        }
        if (avgFreq < minFreq)
        {
            minFreq = avgFreq;
        }
        scaledAvgFreq = Utility.Scale(minFreq, maxFreq, 0f, 1f, avgFreq);
        runningAvgFreq = (avgFreq * alpha) + ((1 - alpha) * runningAvgFreq);

        if (audioSource.clip.name == "Microphone") //boost levels for mic
        {
            avgFreq *= 1000;
            runningAvgFreq *= 1000;
        }

    }
}

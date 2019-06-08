using UnityEngine;
using System.Collections;
using UnityEngine.Audio; // required for dealing with audiomixers

[RequireComponent(typeof(AudioSource))]
public class AudioLineIn : MonoBehaviour
{
    AudioSource audioSource;
    public AudioMixerGroup microphoneMixer;
    int minFreq;
    int maxFreq;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (Microphone.devices.Length > 0)
        {
            string device = Microphone.devices[0].ToString();
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
            if (minFreq == 0 && maxFreq == 0)
            {
                //...meaning 44100 Hz can be used as the recording sampling rate  
                maxFreq = 44100;
            }
            audioSource.clip = Microphone.Start(device, true, 5, maxFreq);
            audioSource.loop = true;
            audioSource.outputAudioMixerGroup = microphoneMixer;
            while (!(Microphone.GetPosition(null) > 0))
            {
                audioSource.Play();
            }
        }
        else
        {
            print("NO MICROPHONES FOUND");
        }

    }

    void Update()
    {

    }

}
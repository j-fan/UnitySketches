using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(BeatsFFT))]
public class StrangeAttractors : MonoBehaviour
{
    // attractors reference
    // https://www.dynamicmath.xyz/strange-attractors/
    // https://www.flickr.com/photos/39445835@N05/sets/72157691795186384

    public AttractorType attractorType;
    public BeatsFFT beatsFFT;
    public OSC osc;

    private ParticleSystem particleSys;
    private ParticleSystem.MainModule particleSysMain;
    private ParticleSystem.Particle[] particles;
    private ParticleSystem.TrailModule particleTrailModule;
    private StrangeAttractor strangeAttractor;
    private float speedModifier = 1.0f;
    
    private void Start()
    {
        // osc.SetAddressHandler("/midi/mixtrack_pro_3/0/3/control_change", OnRightFilter);
        strangeAttractor = new StrangeAttractor();
        particleSys = GetComponent<ParticleSystem>();
        particleSysMain = particleSys.main;
        particleSysMain.simulationSpace = ParticleSystemSimulationSpace.Local;
        particleTrailModule = particleSys.trails;
        ScaleAttractorToFitView();
    }

    private void LateUpdate()
    {
        int maxParticles = particleSysMain.maxParticles;
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }
        particleSys.GetParticles(particles);
        
        for (int i = 0; i < particles.Length; i++){
            particles[i].velocity = strangeAttractor.Apply(attractorType, particles[i].position);

            particles[i].velocity = Vector3.ClampMagnitude(Vector3.Normalize(particles[i].velocity) * 1000 * beatsFFT.avgFreq, 10);
            particles[i].velocity = particles[i].velocity * speedModifier * 4f;

            // prevent particles going to infinity
            if(particles[i].position.magnitude > 800){
                particles[i].startColor = new Color(0, 0, 0, 0);
            }
        }
        particleSys.SetParticles(particles,particles.Length);
        transform.Rotate(30 * beatsFFT.avgFreq, 40 * beatsFFT.avgFreq, 30 * beatsFFT.avgFreq, Space.World);
        float glowFactor = Mathf.Clamp((beatsFFT.runningAvgFreq * 8), 0.01f, 0.1f);
        particleTrailModule.colorOverTrail = new Color(1f, 1f, 1f, glowFactor);
    }

    private void ScaleAttractorToFitView(){
        switch(attractorType){
            case AttractorType.ThreeScroll:
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                particleTrailModule.widthOverTrail = 0.4f;
                break;
            case AttractorType.Chen:
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                break;
            default:
                transform.localScale = new Vector3(1f, 1f, 1f);
                break;
        }
    }

    private void OnRightFilter(OscMessage message)
    {
        int id = Int32.Parse(message.values[0].ToString(), System.Globalization.NumberStyles.HexNumber);
        int value = Int32.Parse(message.values[1].ToString(), System.Globalization.NumberStyles.HexNumber);
        speedModifier = (float)value / 128f;
        print(message.values[1]);
    }
}

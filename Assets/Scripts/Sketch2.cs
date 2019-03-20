using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Sketch2 : MonoBehaviour
{
    private ParticleSystem particleSys;
    private ParticleSystem.MainModule particleSysMain;
    private ParticleSystem.Particle[] particles;

    private float x = 0.01f;
    private float y = 0;
    private float z = 0;

    // constants for lorenz attractor
    // a,b,c equivalent to sigma, rho, beta in original equations
    // interesting starting values for constants can eb found here:
    // https://en.wikipedia.org/wiki/Lorenz_system#Analysis
    private float a = 10;
    private float b = 28;
    private float c = 8.0f/3.0f;


    void Start()
    {
        particleSys = GetComponent<ParticleSystem>();
        particleSysMain = particleSys.main;
    }

    void LateUpdate()
    {
        int maxParticles = particleSysMain.maxParticles;
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }
        // load particles
        particleSys.GetParticles(particles);

        //Transform trueTransform = getParticleSysTransform();

        // dt is time
        float dt = 0.01f;
        // calculate the 3 equations of lorenz
        float dx = (a * (y - x)) * dt;
        float dy = (x * (b - z) - y) * dt;
        float dz = ((x * y) - (c * z)) * dt;
        x = x + dx;
        y = y + dy;
        z = z + dz;

        print(x);

        for (int i = 0; i < particles.Length; i++){
            particles[i].position = new Vector3(x,y,z);
        }

        // save modified particles
        particleSys.SetParticles(particles,particles.Length);

    }

    Transform getParticleSysTransform(){
        
        ParticleSystemSimulationSpace simulationSpace = particleSysMain.simulationSpace;
        Transform trueTransform;
        switch (simulationSpace)
        {
            case ParticleSystemSimulationSpace.Local:
                {
                    trueTransform = transform;
                    break;
                }
            case ParticleSystemSimulationSpace.Custom:
                {
                    trueTransform = particleSysMain.customSimulationSpace;
                    break;
                }
            case ParticleSystemSimulationSpace.World:
                {
                    trueTransform = transform;
                    break;
                }
            default:
                {
                    throw new System.NotSupportedException(

                        string.Format("Unsupported simulation space '{0}'.",
                        System.Enum.GetName(typeof(ParticleSystemSimulationSpace), particleSysMain.simulationSpace)));
                }
        }
        return trueTransform;
    }
}

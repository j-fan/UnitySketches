using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class StrangeAttractors : MonoBehaviour
{
    // TO DO: add the other attractors
    // https://www.dynamicmath.xyz/strange-attractors/

    public enum AttractorType {
        Lorenz,
        Dadras
    }
    public AttractorType attractorType;

    private ParticleSystem particleSys;
    private ParticleSystem.MainModule particleSysMain;
    private ParticleSystem.Particle[] particles;
    
    void Start()
    {
        particleSys = GetComponent<ParticleSystem>();
        particleSysMain = particleSys.main;
        particleSysMain.simulationSpace = ParticleSystemSimulationSpace.Local;
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

        for (int i = 0; i < particles.Length; i++){
            switch (attractorType){
                case AttractorType.Dadras:
                    particles[i].position = applyDadras(particles[i].position);
                    break;
                case AttractorType.Lorenz:
                    particles[i].position = applyLorenz(particles[i].position);
                    break;
                default:
                    particles[i].position = applyDadras(particles[i].position);
                    break;
            }
        }
        // save modified particles
        particleSys.SetParticles(particles,particles.Length);

    }

    Vector3 applyLorenz(Vector3 position){
        // constants for lorenz attractor
        // a,b,c equivalent to sigma, rho, beta in original equations
        // interesting starting values for constants can eb found here:
        // https://en.wikipedia.org/wiki/Lorenz_system#Analysis
        float a = 10;
        float b = 28;
        float c = 8.0f/3.0f;
        // dt is time
        float dt = 0.01f;
        // calculate the 3 equations of lorenz
        float dx = (a * (position.y - position.x)) * dt;
        float dy = (position.x * (b - position.z) - position.y) * dt;
        float dz = ((position.x * position.y) - (c * position.z)) * dt;
        float x = position.x + dx;
        float y = position.y + dy;
        float z = position.z + dz;   
        return new Vector3(x,y,z);    
    }

    Vector3 applyDadras(Vector3 position){
        float a = 3;
        float b = 2.7f;
        float c = 1.7f;
        float d = 2f;
        float e = 9f;
        
        float dt = 0.01f;
        float dx = (position.y - (a * position.x) + (b * position.y * position.z)) * dt;
        float dy = ((c * position.y) - (position.x * position.z) + position.z) * dt;
        float dz = ((d * position.x * position.y) - (e * position.z)) * dt;
        float x = position.x + dx;
        float y = position.y + dy;
        float z = position.z + dz;   
        return new Vector3(x,y,z);         
    }
}

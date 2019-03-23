using System;
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
        Lorenz84,
        Dadras,
        Thomas,
        Aizawa,
        Chen,
        Halvorsen,
        RabinovichFabrikant,
        ThreeScroll,
        WangSu,
        Sprott
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
                case AttractorType.Lorenz84:
                    particles[i].position = applyLorenz84(particles[i].position);
                    break;
                case AttractorType.Aizawa:
                    particles[i].position = applyAizawa(particles[i].position);
                    break;   
                case AttractorType.Thomas:
                    particles[i].position = applyThomas(particles[i].position);
                    break;
                case AttractorType.Sprott:
                    particles[i].position = applySprott(particles[i].position);
                    break;
                case AttractorType.Chen:
                    particles[i].position = applyChen(particles[i].position);
                    break;  
                case AttractorType.Halvorsen:
                    particles[i].position = applyHalvorsen(particles[i].position);
                    break;      
                case AttractorType.RabinovichFabrikant:
                    particles[i].position = applyRabinovichFabrikant(particles[i].position);
                    break;  
                case AttractorType.ThreeScroll:
                    particles[i].position = applyThreeScroll(particles[i].position);
                    break;          
                case AttractorType.WangSu:
                    particles[i].position = applyWangSu(particles[i].position);
                    break;                    
                default:
                    particles[i].position = applyDadras(particles[i].position);
                    break;
            }
        }
        // save modified particles
        particleSys.SetParticles(particles,particles.Length);

    }

    private Vector3 applyLorenz84(Vector3 p)
    {
        float a = 0.95f;
        float b = 7.91f;
        float f = 4.83f;
        float g = 4.66f;

        float dt = 0.005f;
        float dx = (-a*p.x - Mathf.Pow(p.y,2) - Mathf.Pow(p.z,2) + a*f) * dt;
        float dy = (-p.y + p.x*p.y - b*p.x*p.z + g) * dt;
        float dz = (-p.z + b*p.x*p.y + p.x*p.z) * dt;
        float x = p.x + dx;
        float y = p.y + dy;
        float z = p.z + dz;   
        return new Vector3(x,y,z);   
    }

    private Vector3 applyAizawa(Vector3 p)
    {
        float a = 0.95f;
        float b = 0.7f;
        float c = 0.6f;
        float d = 3.5f;
        float e = 0.25f;
        float f = 0.1f;
        
        float dt = 0.01f;
        float dx = ((p.z - b)*p.x - d*p.y);
        float dy = d * p.x + (p.z - b)*p.y;
        float dz = c + a*p.z 
                    - (Mathf.Pow(p.z,3)/3.0f) 
                    - (Mathf.Pow(p.x,2) 
                    + Mathf.Pow(p.y,2))*(1 + e*p.z) 
                    + f*p.z*Mathf.Pow(p.x,3); 
        dx *= dt;
        dy *= dt;
        dz *= dt;
        float x = p.x + dx;
        float y = p.y + dy;
        float z = p.z + dz;   
        return new Vector3(x,y,z);   
    }

    Vector3 applyWangSu(Vector3 position)
    {
        throw new NotImplementedException();
    }

    Vector3 applyThreeScroll(Vector3 p)
    {
        float a = 32.48f;
        float b = 45.84f;
        float c = 1.18f;
        float d = 0.13f;
        float e = 0.57f;
        float f = 14.7f;

        float dx = a*(p.y - p.x) + d*p.x*p.y;
        float dy = b*p.x - p.x*p.z + f*p.y;
        float dz = c*p.z + p.x*p.y - e*Mathf.Pow(p.x,2);

        float dt = 0.001f;
        dx *= dt;
        dy *= dt;
        dz *= dt;

        float x = p.x + dx;
        float y = p.y + dy;
        float z = p.z + dz;   
        return new Vector3(x,y,z);   
    }

    Vector3 applyRabinovichFabrikant(Vector3 position)
    {
        throw new NotImplementedException();
    }
    Vector3 applyHalvorsen(Vector3 position)
    {
        throw new NotImplementedException();
    }

    Vector3 applyChen(Vector3 position)
    {
        float a = 5f;
        float b = -10f;
        float c = -0.38f;
        float dx = a*position.x - position.y*position.z;
        float dy = b*position.y + position.x*position.z;
        float dz= c*position.z + (position.x*position.y/3.0f);
        float dt = 0.01f;
        dx *= dt;
        dy *= dt;
        dz *= dt;
        float x = position.x + dx;
        float y = position.y + dy;
        float z = position.z + dz;
        return new Vector3(x,y,z);
    }

    Vector3 applySprott(Vector3 p)
    {
        float a = 2.07f;
        float b = 1.79f;
        
        float dt = 0.01f;
        float dx = (p.y + a*p.x*p.y + p.x*p.z) * dt;
        float dy = (1 - b*Mathf.Pow(p.x,2) + p.y*p.z) * dt;
        float dz = (p.x - Mathf.Pow(p.x,2) - Mathf.Pow(p.y,2)) * dt;
        float x = p.x + dx;
        float y = p.y + dy;
        float z = p.z + dz;   
        return new Vector3(x,y,z); 
    }

    Vector3 applyThomas(Vector3 position)
    {
        float b = 0.208186f;
        float dt = 0.01f;

        float dx = (Mathf.Sin(position.y) - b * position.x) * dt;
        float dy = (Mathf.Sin(position.z) - b * position.y) * dt;
        float dz = (Mathf.Sin(position.x) - b * position.z) * dt;
        float x = position.x + dx;
        float y = position.y + dy;
        float z = position.z + dz;   
        return new Vector3(x,y,z);    
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    public float perceptionRadius = 1f;
    public float maxSteeringForce = 0.4f;
    private ParticleSystem particleSys;
    private ParticleSystem.MainModule particleSysMain;
    private ParticleSystem.Particle[] particles;
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
        particleSys.GetParticles(particles);
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i] = steer(particles[i]);
            particles[i].rotation3D = Quaternion.LookRotation(particles[i].velocity, Vector3.up).eulerAngles;
        }
        particleSys.SetParticles(particles, particles.Length);
    }
    private ParticleSystem.Particle steer(ParticleSystem.Particle currentParticle)
    {
        var currentPos = currentParticle.position;
        var currentVelocity = currentParticle.velocity;

        Vector3 alignmentSteering = new Vector3();
        Vector3 cohesionSteering = new Vector3();
        int boidsSeen = 0;
        foreach (var particle in particles)
        {
            float dist = Vector3.Distance(currentPos, particle.position);
            if (dist < perceptionRadius)
            {
                alignmentSteering += particle.velocity.normalized;
                cohesionSteering += particle.position;
                boidsSeen++;
            }
        }
        if (boidsSeen > 0)
        {
            alignmentSteering /= boidsSeen;
            alignmentSteering -= currentVelocity;
            alignmentSteering = Vector3.ClampMagnitude(alignmentSteering, maxSteeringForce);

            cohesionSteering /= boidsSeen;
            cohesionSteering -= currentPos;
            cohesionSteering -= currentVelocity;
            cohesionSteering = Vector3.ClampMagnitude(cohesionSteering, maxSteeringForce);

        }
        currentParticle.velocity += cohesionSteering;
        currentParticle.velocity += alignmentSteering;
        return currentParticle;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ForceType
{
    Electric,
    Gravity,
    SimpleAttractor,
    SimpleRepeller,
    Vortex,
    Airflow
}

public enum VortexType
{
    Forward,
    Up,
    UpRepulsion,
    ForwardRepulsion
}

public class ForceAttractor
{
    public GameObject[] Attractors { get; set; }

    public ForceAttractor(GameObject[] attractors)
    {
        if (attractors == null)
        {
            GenerateAttractors(2, null);
        }
        else
        {
            Attractors = attractors;
        }
    }

    // velocity does not matter if forcetype != gravity
    public Vector3 Apply(ForceType type, Vector3 position, Vector3 velocity)
    {
        Vector3 force;
        switch (type)
        {
            case ForceType.SimpleAttractor:
                force = applySimpleAttractor(position);
                break;
            case ForceType.SimpleRepeller:
                force = applySimpleRepeller(position);
                break;
            case ForceType.Gravity:
                force = applyGravity(position);
                break;
            case ForceType.Electric:
                force = applyElectric(position);
                break;
            case ForceType.Vortex:
                force = applyVortex(position, VortexType.Forward);
                break;
            case ForceType.Airflow:
                force = applyAirFlow(position);
                break;
            default:
                force = applySimpleAttractor(position);
                break;
        }

        if (type == ForceType.Gravity)
        {
            velocity += force;   //visualise  acceleration
        }
        else
        {
            velocity = force;    //visualise velocity
        }
        return velocity;
    }

    public Vector3 ApplyVortexSpecial(VortexType type, Vector3 position)
    {
        return applyVortex(position, type);
    }

    public void GenerateAttractors(int numAttractors, GameObject attractorObj)
    {
        Attractors = new GameObject[numAttractors];
        for (int i = 0; i < numAttractors; i++)
        {
            GameObject newAttractor;
            if (attractorObj == null)
            {
                newAttractor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newAttractor.GetComponent<Renderer>().material.color = Color.white;
            }
            else
            {
                newAttractor = GameObject.Instantiate(attractorObj);
            }
            newAttractor.transform.position = new Vector3(
                Random.Range(-4f, 4f),
                Random.Range(-4f, 4f),
                Random.Range(-4f, 4f));
            Attractors[i] = newAttractor;
        }
    }

    // potential flow  https://github.com/arkaragian/Fluid-Field/blob/master/field.js
    private Vector3 applyAirFlow(Vector3 position)
    {
        Vector3 direction = (Vector3.back * 10);
        float distance = float.MaxValue; // used to find closest attractor
        float maxDistance = 20f;
        float fieldStrength = 10f;

        foreach (GameObject a in Attractors)
        {
            distance = Vector3.Distance(position, a.transform.position);
            if (distance < maxDistance)
            {
                float dx = position.x - a.transform.position.x;
                float dz = position.z - a.transform.position.z;

                float angle = Mathf.Atan2(dz, dx);
                float ux = (fieldStrength / distance) * Mathf.Cos(angle);
                float uz = (fieldStrength / distance) * Mathf.Sin(angle);

                float falloff = ((maxDistance - distance) / distance);
                direction = direction + new Vector3(ux, 0, uz) * falloff;
            }

        }
        Vector3 totalForce = direction * Time.deltaTime;
        return totalForce;
    }
    private Vector3 applySimpleAttractor(Vector3 position)
    {
        Vector3 direction = Vector3.zero;
        float distance = float.MaxValue; // used to find closest attractor

        foreach (GameObject a in Attractors)
        {
            if (Vector3.Distance(position, a.transform.position) < distance)
            {
                distance = Vector3.Distance(position, a.transform.position);
                direction = (a.transform.position - position).normalized;
            }

        }
        Vector3 totalForce = direction * Time.deltaTime;
        return totalForce;
    }

    // currently unimplemented
    private Vector3 applySimpleRepeller(Vector3 position)
    {
        Vector3 direction = Vector3.zero;
        float distance = float.MaxValue; // used to find closest attractor

        foreach (GameObject a in Attractors)
        {
            if (Vector3.Distance(position, a.transform.position) < distance)
            {
                distance = Vector3.Distance(position, a.transform.position);
                direction = (a.transform.position - position).normalized;
            }

        }
        Vector3 totalForce = -direction * Time.deltaTime;
        return totalForce;
    }

    /*
     * algo from: https://gamedevelopment.tutsplus.com/tutorials/adding-turbulence-to-a-particle-system--gamedev-13332
     */
    private Vector3 applyVortex(Vector3 pos, VortexType type)
    {
        float distanceX = float.MaxValue;
        float distanceY = float.MaxValue;
        float distanceZ = float.MaxValue;
        float distance = float.MaxValue;

        Vector3 direction = Vector3.zero;
        foreach (GameObject a in Attractors)
        {
            if (Vector3.Distance(pos, a.transform.position) < distance)
            {
                distanceX = (pos.x - a.transform.position.x);
                distanceY = (pos.y - a.transform.position.y);
                distanceZ = (pos.z - a.transform.position.z);
                distance = Vector3.Distance(pos, a.transform.position);
            }

            direction += (a.transform.position - pos).normalized;
        }

        float vortexScale = 10f;
        float vortexSpeed = 5f;

        Vector3 distances = new Vector3(distanceX, distanceY, distanceZ);

        Vector3 totalForce;
        switch (type)
        {
            case VortexType.Forward:
                totalForce = vortexForward(vortexSpeed, 20f, distances, direction);
                break;
            case VortexType.Up:
                totalForce = vortexUp(vortexSpeed, vortexScale, distances, direction);
                break;
            case VortexType.UpRepulsion:
                totalForce = vortexUpRepulsion(vortexSpeed, vortexScale, distances, direction);
                break;
            case VortexType.ForwardRepulsion:
                totalForce = vortexForwardRepulsion(vortexSpeed, 15f, distances, direction);
                break;
            default:
                totalForce = vortexForward(vortexSpeed, vortexScale, distances, direction);
                break;
        }
        return totalForce;
    }

    private Vector3 vortexForward(float vortexSpeed, float vortexScale, Vector3 distances, Vector3 direction)
    {
        float distanceX = distances.x;
        float distanceY = distances.y;
        float distanceZ = distances.z;

        float factor = 1 / (1 + (distanceX * distanceX + distanceY * distanceY) / vortexScale);

        float vx = distanceX * vortexSpeed * factor;
        float vy = distanceY * vortexSpeed * factor;
        float vz = distanceZ * vortexSpeed * factor;
        Vector3 totalForce = Quaternion.AngleAxis(90, Vector3.forward) * new Vector3(vx, vy, 0) + (direction);
        return totalForce;
    }


    private Vector3 vortexUp(float vortexSpeed, float vortexScale, Vector3 distances, Vector3 direction)
    {
        float distanceX = distances.x;
        float distanceY = distances.y;
        float distanceZ = distances.z;

        float factor = 1 / (1 + (distanceX * distanceX + distanceZ * distanceZ) / vortexScale);

        float vx = distanceX * vortexSpeed * factor;
        float vy = distanceY * vortexSpeed * factor;
        float vz = distanceZ * vortexSpeed * factor;
        Vector3 totalForce = Quaternion.AngleAxis(90, Vector3.up) * new Vector3(vx, 0, vz) + (direction);
        return totalForce;
    }

    private Vector3 vortexUpRepulsion(float vortexSpeed, float vortexScale, Vector3 distances, Vector3 direction)
    {
        float distanceX = distances.x;
        float distanceY = distances.y;
        float distanceZ = distances.z;

        float factor = 1 / (1 + (distanceX * distanceX + distanceY * distanceY) / vortexScale);

        float vx = distanceX * vortexSpeed * factor;
        float vy = distanceY * vortexSpeed * factor;
        float vz = distanceZ * vortexSpeed * factor;
        Vector3 totalForce = Quaternion.AngleAxis(90, Vector3.up) * new Vector3(vx, vy, vz) + (direction);
        return totalForce;
    }

    private Vector3 vortexForwardRepulsion(float vortexSpeed, float vortexScale, Vector3 distances, Vector3 direction)
    {
        float distanceX = distances.x;
        float distanceY = distances.y;
        float distanceZ = distances.z;

        //float factor = 1 / (1 + (distanceX * distanceX + distanceZ * distanceZ)/ vortexScale);
        float factor = 1 / (1 + (distanceX * distanceX + distanceY * distanceY) / vortexScale);

        float vx = distanceX * vortexSpeed * factor;
        float vy = distanceY * vortexSpeed * factor;
        float vz = Mathf.Clamp(distanceZ * vortexSpeed * factor, -5f, 5f);

        Vector3 totalForce = Quaternion.AngleAxis(90, Vector3.forward) * new Vector3(vx, vy, vz) + (direction);
        return totalForce;
    }

    private Vector3 applyGravity(Vector3 pos)
    {
        float g = 1f;
        float mass = 2f;

        Vector3 direction = Vector3.zero;
        Vector3 totalForce = Vector3.zero;
        foreach (GameObject a in Attractors)
        {
            direction = (a.transform.position - pos).normalized;
            float magnitude = direction.magnitude;
            Mathf.Clamp(magnitude, 1.0f, 5.0f); //eliminate extreme result for very close or very far objects

            float force = (g * mass * mass) / direction.magnitude * direction.magnitude;
            totalForce += ((direction) * force) * Time.deltaTime;
        }

        return totalForce;
    }

    private Vector3 applyElectric(Vector3 pos)
    {
        Vector3 totalForce = Vector3.zero;
        Vector3 force = Vector3.zero;
        int i = 0;
        foreach (GameObject a in Attractors)
        {
            float dist = Vector3.Distance(pos, a.transform.position) * 100000;
            float fieldMag = 99999 / dist * dist;
            Mathf.Clamp(fieldMag, 0.0f, 5.0f);

            //alternate postive and negative charges
            if (i % 2 == 0)
            {
                force.x -= fieldMag * (pos.x - a.transform.position.x) / dist;
                force.y -= fieldMag * (pos.y - a.transform.position.y) / dist;
                force.z -= fieldMag * (pos.z - a.transform.position.z) / dist;
            }
            else
            {
                force.x += fieldMag * (pos.x - a.transform.position.x) / dist;
                force.y += fieldMag * (pos.y - a.transform.position.y) / dist;
                force.z += fieldMag * (pos.z - a.transform.position.z) / dist;
            }
            i++;
        }
        totalForce = force;
        return totalForce;
    }
}

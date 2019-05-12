using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurlNoise
{
    private FastNoise fastNoise;
    private float offset = 0f;

    public float Frequency { get; set; } = 7f;
    public float Amplitude { get; set; } = 1f;
    public int Octaves { get; set; } = 1;
    public float Persistence { get; set; } = 1f;

    public CurlNoise()
    {
        fastNoise = new FastNoise();
    }

    Vector3 snoiseVec3(Vector3 v)
    {
        float s = octaveSimplex(
            new Vector3(v.x, v.y, v.z),
            Octaves,
            Persistence,
            Frequency,
            Amplitude);
        float s1 = octaveSimplex(
            new Vector3(v.y - 19.1f + offset, v.z + 33.4f, v.x + 47.2f + offset),
            Octaves,
            Persistence,
            Frequency,
            Amplitude);
        float s2 = octaveSimplex(
            new Vector3(v.z + 74.2f + offset, v.x - 124.5f + offset, v.y + 99.4f),
            Octaves,
            Persistence,
            Frequency,
            Amplitude);
        Vector3 c = new Vector3(s, s1, s2);
        return c;

    }

    public Vector3 calculate(Vector3 p)
    {
        float e = 0.01f;
        Vector3 dx = new Vector3(e, 0f, 0f);
        Vector3 dy = new Vector3(0f, e, 0f);
        Vector3 dz = new Vector3(0f, 0f, e);

        Vector3 p_x0 = snoiseVec3(p - dx);
        Vector3 p_x1 = snoiseVec3(p + dx);
        Vector3 p_y0 = snoiseVec3(p - dy);
        Vector3 p_y1 = snoiseVec3(p + dy);
        Vector3 p_z0 = snoiseVec3(p - dz);
        Vector3 p_z1 = snoiseVec3(p + dz);

        float x = p_y1.z - p_y0.z - p_z1.y + p_z0.y;
        float y = p_z1.x - p_z0.x - p_x1.z + p_x0.z;
        float z = p_x1.y - p_x0.y - p_y1.x + p_y0.x;

        float divisor = 1.0f / (2.0f * e);

        offset += 0.001f;

        return Vector3.Normalize(new Vector3(x, y, z) * divisor);
    }

    float octaveSimplex(Vector3 vector, int octaves, float persistence, float frequency, float amplitude)
    {
        float total = 0f;
        float maxValue = 0;  // Used for normalizing result to 0.0 - 1.0
        for (int i = 0; i < octaves; i++)
        {
            total += fastNoise.GetSimplex(vector.x * frequency, vector.y * frequency, vector.z * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }
        return total / maxValue;
    }
}

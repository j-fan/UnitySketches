using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMove : MonoBehaviour
{
    private FastNoise fastNoise = new FastNoise();
    private Vector3 centre;
    private float motionRadius;
    private float seed;
    private int numFrames = 100;
    void Start()
    {
        centre = Vector3.zero;
        motionRadius = Random.Range(1f, 4f);
        seed = Random.Range(1f,9999f);
    }

    void Update()
    {
        float t = 1.0f * ( Time.frameCount - 1) / numFrames;
        transform.position = new Vector3(
            nextX(t, centre.x, motionRadius), 
            nextY(t, centre.y, motionRadius),
            centre.z);   
    }
    private float nextXSimplex(float t, float x, float motion_radius, float seed){
        return x + fastNoise.GetSimplex(seed + motion_radius*Mathf.Cos(Mathf.PI*t),
                                        motion_radius*Mathf.Sin(Mathf.PI*t));
    }
    private float nextYSimplex(float t,float y, float motion_radius, float seed){
        return y  + fastNoise.GetSimplex(seed + motion_radius*Mathf.Cos(Mathf.PI*t),
                                         motion_radius*Mathf.Sin(Mathf.PI*t));
    }
    private float nextX(float t, float x, float radius){
    return x + Mathf.Cos(Mathf.PI * 2 *t) * radius;
    }
    private float nextY(float t,float y, float radius){
    return y + Mathf.Sin(Mathf.PI * 2*t) * radius;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowfieldWorms : MonoBehaviour
{
    public TubeRenderer TubeRenderer;
    public int NumWorms = 20;
    public int maxWormVertexes = 100;
    public float minWormVertexDistance = 0.05f;      // lower numbers = smoother but shorter worms
    public float WormSpeed = 1f;                     

    // the best noise params to play with are octaves and frequency
    public float frequency = 10f;                    // how quickly simplex values change (higher number = smaller curls)
    public float amplitude = 1f;                     // how big the range of simplex values are (effect unknown)
    public int octaves = 1;                          // how many iterations of simplex to use (higher number = more curls)
    public float persistence = 1f;                   // how much effect each subsequent iteration has (higher number = enhances effect of octave count)

    private List<Worm> worms = new List<Worm>();
    private FastNoise fastNoise;
    private float maxDist = 5f;
    private float attractionSpeed = 3.0f;
    float offset = 0f;                               // makes the curl noise field non static

    private class Worm {
        public BoundedQueue<Vector3> PointsQueue { get; set; }
        public TubeRenderer TubeRenderer { get; set; }
        public Vector3 CurrentPos { get; set; }
    }
    
    void Start()
    {
        fastNoise = new FastNoise();
        for(int i = 0; i < NumWorms; i++)
        {
            Worm worm = new Worm();
            worm.CurrentPos = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f)
            );
            worm.PointsQueue = new BoundedQueue<Vector3>(maxWormVertexes);
            worm.TubeRenderer = Instantiate(TubeRenderer,Vector3.zero,Quaternion.identity);
            worms.Add(worm);
        }
    }

    void Update()
    {
        for(int i = 0; i < NumWorms; i++)
        {
            Worm w = worms[i];
            Vector3 flowVector = curlNoise(w.CurrentPos) * WormSpeed;
            Vector3 lastPos = w.CurrentPos;            
            Vector3 newPos =  w.CurrentPos + flowVector * Time.deltaTime;
            if(Vector3.Distance(newPos, lastPos) > minWormVertexDistance)
            {
                w.PointsQueue.Enqueue(newPos);
                w.TubeRenderer.SetPoints(w.PointsQueue.ToArray(),Color.white);
            }
            float dist = Vector3.Distance(newPos, Vector3.zero);
            float step =  attractionSpeed * Time.deltaTime;
            if(dist > maxDist)
            {
                newPos = Vector3.MoveTowards(newPos, Vector3.zero, step);
                // newPos = new Vector3(
                //     Mathf.Clamp(newPos.x,0,maxDist),
                //     Mathf.Clamp(newPos.y,0,maxDist),
                //     Mathf.Clamp(newPos.z,0,maxDist)
                // );
            }
            w.CurrentPos = newPos;
            offset += 0.001f;
        }
    }

    Vector3 snoiseVec3(Vector3 v)
    {
        float s = octaveSimplex(
            new Vector3(v.x, v.y, v.z),
            octaves,
            persistence,
            frequency,
            amplitude);
        float s1 = octaveSimplex(
            new Vector3(v.y - 19.1f+offset, v.z + 33.4f, v.x + 47.2f+offset), 
            octaves,
            persistence,
            frequency,
            amplitude);
        float s2 = octaveSimplex(
            new Vector3(v.z + 74.2f+offset, v.x - 124.5f+offset, v.y + 99.4f),
            octaves,
            persistence,
            frequency,
            amplitude);
        Vector3 c = new Vector3(s, s1, s2);
        return c;

    }

    Vector3 curlNoise(Vector3 p)
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
        return Vector3.Normalize(new Vector3(x, y, z) * divisor);
    }
    
    float octaveSimplex(Vector3 vector, int octaves, float persistence, float frequency, float amplitude){
        float total = 0f;
        float maxValue = 0;  // Used for normalizing result to 0.0 - 1.0
        for(int i = 0; i < octaves; i++) {
            total += fastNoise.GetSimplex(vector.x * frequency, vector.y * frequency, vector.z * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }
        return total/maxValue;
    }
}

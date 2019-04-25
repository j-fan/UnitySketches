using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowfieldWorms : MonoBehaviour
{
    public TubeRenderer TubeRenderer;
    public Flowfield Flowfield;
    public int NumWorms = 5;
    public float WormSpeed = 1f;

    private List<Worm> worms = new List<Worm>();
    private int maxWormVertexes = 200;
    private float minWormVertexDistance = 0.05f;
    private FastNoise fastNoise;
    private float offset = 0.01f;

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
                Random.Range(30f, 40f),
                Random.Range(30f, 40f),
                Random.Range(30f, 40f)
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
            Vector3 flowVector = curlNoise(w.CurrentPos) * 20f;            
            Vector3 newPos =  w.CurrentPos + flowVector * Time.deltaTime;
            Vector3 lastPos = w.TubeRenderer.vertices[w.TubeRenderer.vertices.Length -1].point;
            if(Vector3.Distance(newPos, lastPos) > minWormVertexDistance)
            {
                w.PointsQueue.Enqueue(newPos);
                w.TubeRenderer.SetPoints(w.PointsQueue.ToArray(),Color.white);
            }
            w.CurrentPos = newPos;
        }
    }
    Vector3 snoiseVec3(Vector3 v)
    {

        float s = fastNoise.GetSimplex(v.x, v.y, v.z);
        float s1 = fastNoise.GetSimplex(v.y - 19.1f, v.z + 33.4f, v.x + 47.2f);
        float s2 = fastNoise.GetSimplex(v.z + 74.2f, v.x - 124.5f, v.y + 99.4f);
        Vector3 c = new Vector3(s, s1, s2);
        return c;

    }

    Vector3 curlNoise(Vector3 p)
    {

        float e = .1f;
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
}

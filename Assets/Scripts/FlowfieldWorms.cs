using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FlowfieldWorms : MonoBehaviour
{
    public TubeRenderer TubeRenderer;
    public int NumWorms = 20;
    public int MaxWormVertexes = 100;
    public float MinWormVertexDistance = 0.1f;      // lower numbers = smoother but shorter worms
    public float CurlSpeed = 5f;

    // the best noise params to play with are octaves and frequency
    CurlNoise curlNoise;
    public float Frequency = 7f;                    // how quickly simplex values change (higher number = smaller curls)
    public float Amplitude = 1f;                     // how big the range of simplex values are (effect unknown)
    public int Octaves = 1;                          // how many iterations of simplex to use (higher number = more curls)
    public float Persistence = 1f;                   // how much effect each subsequent iteration has (higher number = enhances effect of octave count)

    public GameObject CentreObj;
    public FlowfieldWormBehaviour flowfieldWormBehaviour;
    private List<Worm> worms = new List<Worm>();
    private FastNoise fastNoise;
    private float maxDist = 5f;
    private float attractionSpeed = 3.0f;
    // makes the curl noise field non static
    private ForceAttractor forceAttractor;


    public enum FlowfieldWormBehaviour
    {
        Vortex,
        Ring,
        CurlNoise,
        Gravity
    }

    private class Worm
    {
        public BoundedQueue<Vector3> PointsQueue { get; set; }
        public TubeRenderer TubeRenderer { get; set; }
        public Vector3 position { get; set; }
        public Vector3 velocity { get; set; }
        public Vector3 acceleration { get; set; }
        public void update()
        {
            velocity += acceleration;
            velocity = Vector3.ClampMagnitude(velocity, 10f);
            position += velocity;
        }
    }

    void Start()
    {
        fastNoise = new FastNoise();
        curlNoise = new CurlNoise();
        if (!CentreObj)
        {
            GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }
        CentreObj.transform.position = Vector3.zero;
        forceAttractor = new ForceAttractor(new GameObject[]{
            CentreObj
        });
        for (int i = 0; i < NumWorms; i++)
        {
            Worm worm = new Worm();
            worm.position = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f)
            );
            worm.velocity = randomVector();
            worm.PointsQueue = new BoundedQueue<Vector3>(MaxWormVertexes);
            worm.TubeRenderer = Instantiate(TubeRenderer, Vector3.zero, Quaternion.identity);
            worms.Add(worm);
        }
    }

    void Update()
    {
        for (int i = 0; i < NumWorms; i++)
        {
            Worm w = worms[i];
            float deltaTime = Time.deltaTime;
            // ThreadPool.QueueUserWorkItem(new WaitCallback((state) => updateWorm(state, w, deltaTime)));
            updateWorm(null, w, deltaTime);
        }
    }

    Vector3 randomVector()
    {
        return new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f));
    }

    void updateWorm(object state, Worm w, float deltaTime)
    {
        Vector3 flowVector;
        Vector3 lastPos = w.position;

        switch (flowfieldWormBehaviour)
        {
            case FlowfieldWormBehaviour.CurlNoise:
                flowVector = curlNoise.calculate(w.position) * CurlSpeed;
                Vector3 newPos = w.position + flowVector * deltaTime;
                float dist = Vector3.Distance(newPos, Vector3.zero);
                float step = attractionSpeed * deltaTime;

                float distanceRatio = Mathf.Clamp(dist, 0, maxDist) / maxDist;
                newPos = distanceRatio * Vector3.MoveTowards(newPos, Vector3.zero, step) +
                    (1 - distanceRatio) * newPos;

                w.position = newPos;
                w.velocity = Vector3.zero;
                break;
            case FlowfieldWormBehaviour.Vortex:
                flowVector = forceAttractor.ApplyVortexSpecial(VortexType.Forward, w.position);
                w.velocity = flowVector * 0.05f;
                break;
            case FlowfieldWormBehaviour.Ring:
                flowVector = forceAttractor.ApplyVortexSpecial(VortexType.ForwardRepulsion, w.position);
                w.velocity = flowVector * 0.05f;
                w.position = new Vector3(w.position.x, w.position.y, Mathf.Clamp(w.position.z, -5f, 5f));
                break;
            case FlowfieldWormBehaviour.Gravity:
                if (w.velocity.magnitude < 0.01f)
                {
                    w.velocity = randomVector();
                }
                flowVector = forceAttractor.Apply(ForceType.Gravity, w.position, w.velocity);
                flowVector = Vector3.ClampMagnitude(flowVector, 2f);
                w.velocity = flowVector;
                break;

        }

        w.update();

        if (Vector3.Distance(w.position, lastPos) > MinWormVertexDistance)
        {
            w.PointsQueue.Enqueue(w.position);
            w.TubeRenderer.SetPoints(w.PointsQueue.ToArray(), Color.white);
        }
    }

}

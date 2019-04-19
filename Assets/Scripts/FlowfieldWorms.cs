using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowfieldWorms : MonoBehaviour
{
    public TubeRenderer TubeRenderer;
    public Flowfield Flowfield;
    public int NumWorms = 5;

    private List<Worm> worms = new List<Worm>();
    private int maxWormVertexes = 20;

    private class Worm {
        public BoundedQueue<Vector3> PointsQueue { get; set; }
        public TubeRenderer TubeRenderer { get; set; }
        public Vector3 CurrentPos { get; set; }
    }
    
    void Start()
    {
        for(int i = 0; i < NumWorms; i++)
        {
            Worm worm = new Worm();
            worm.CurrentPos = new Vector3(
                Random.Range(-5f,5f),
                Random.Range(-5f,5f),
                Random.Range(-5f,5f)
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
            Vector3Int particlePos = new Vector3Int(
                        Mathf.FloorToInt(Mathf.Clamp((w.CurrentPos.x / Flowfield.cellSize),0,Flowfield.gridSize.x-1)),
                        Mathf.FloorToInt(Mathf.Clamp((w.CurrentPos.y / Flowfield.cellSize), 0, Flowfield.gridSize.y - 1)),
                        Mathf.FloorToInt(Mathf.Clamp((w.CurrentPos.z / Flowfield.cellSize), 0, Flowfield.gridSize.z - 1))
                        );
            Vector3 flowVector = Flowfield.flowFieldDirections[particlePos.x, particlePos.y, particlePos.z];

            w.CurrentPos += flowVector * Time.deltaTime * 2f;
            w.PointsQueue.Enqueue(w.CurrentPos);
            w.TubeRenderer.SetPoints(w.PointsQueue.ToArray(),Color.white);
        }
    }
}

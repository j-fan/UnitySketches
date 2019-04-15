using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowfieldWorms : MonoBehaviour
{
    public TubeRenderer TubeRenderer;
    private List<TubeRenderer> worms;
    public Flowfield Flowfield;

    private Vector3 currentPos;
    private BoundedQueue<Vector3> wormPointsQueue;
    private int maxWormVertexes = 100;
    
    void Start()
    {
        currentPos = Vector3.zero;
        wormPointsQueue = new BoundedQueue<Vector3>(maxWormVertexes);
    }

    void Update()
    {

        Vector3Int particlePos = new Vector3Int(
                    Mathf.FloorToInt(Mathf.Clamp((currentPos.x / Flowfield.cellSize),0,Flowfield.gridSize.x-1)),
                    Mathf.FloorToInt(Mathf.Clamp((currentPos.y / Flowfield.cellSize), 0, Flowfield.gridSize.y - 1)),
                    Mathf.FloorToInt(Mathf.Clamp((currentPos.z / Flowfield.cellSize), 0, Flowfield.gridSize.z - 1))
                    );
        Vector3 flowVector = Flowfield.flowFieldDirections[particlePos.x, particlePos.y, particlePos.z] ;
        currentPos += flowVector * 0.05f;
        wormPointsQueue.Enqueue(currentPos);

        TubeRenderer.SetPoints(wormPointsQueue.ToArray(),Color.white);
   
    }
}

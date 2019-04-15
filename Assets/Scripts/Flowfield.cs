using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Script adapted from Peerplay 
 *  https://www.youtube.com/channel/UCBkub2TsbCFIfdhuxRr2Lrw
 */

public class Flowfield : MonoBehaviour {
    FastNoise fastNoise;
    public Vector3Int gridSize;
    public float noiseStrength;
    public Vector3 offset, offsetSpeed;
    public float cellSize;
    public Vector3[,,] flowFieldDirections;
    public bool drawPreview = false;
    

	// Use this for initialization
	void Start () {
        flowFieldDirections = new Vector3[gridSize.x, gridSize.y, gridSize.z];
        fastNoise = new FastNoise();
    }
	
	// Update is called once per frame
	void Update () {
        calculateFlowfieldDirections();
	}

    void calculateFlowfieldDirections()
    {
        offset = new Vector3(offset.x + (offsetSpeed.x * Time.deltaTime),
                             offset.y + (offsetSpeed.y * Time.deltaTime),
                             offset.z + (offsetSpeed.z * Time.deltaTime));

        float xOff = 0f;
        for (int x = 0; x < gridSize.x; x++)
        {
            float yOff = 0f;
            for (int y = 0; y < gridSize.y; y++)
            {
                float zOff = 0f;
                for (int z = 0; z < gridSize.z; z++)
                {
                    float noise = fastNoise.GetSimplex(xOff + offset.x, yOff + offset.y, zOff + offset.z) + 1;
                    Vector3 noiseDirection = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), Mathf.Cos(noise * Mathf.PI));
                    flowFieldDirections[x,y,z] = Vector3.Normalize(noiseDirection);
                    zOff += noiseStrength;
                }
                yOff += noiseStrength;
            }
            xOff += noiseStrength;
        }
    }

    
    private void OnDrawGizmos()
    {
        if(flowFieldDirections != null && drawPreview){
            Gizmos.color = Color.white;
            Vector3 dimensions =  new Vector3(gridSize.x * cellSize,
                                              gridSize.y * cellSize,
                                              gridSize.z * cellSize);
            Gizmos.DrawWireCube(transform.position + dimensions * 0.5f , dimensions);


            // used to preview flowfield lines in scene mode
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    for (int z = 0; z < gridSize.z; z++)
                    {
                        Vector3 noiseDirection = flowFieldDirections[x,y,z];
                        Gizmos.color = new Color(noiseDirection.normalized.x, noiseDirection.normalized.y, noiseDirection.normalized.z);
                        Vector3 pos = new Vector3(x*cellSize, y*cellSize, z*cellSize) + transform.position;
                        Vector3 endpos = pos + Vector3.Normalize(noiseDirection);
                        Gizmos.DrawLine(pos, endpos);
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sketch1 : MonoBehaviour
{
    List<GameObject> points;
    List<Vector3> centers;  
    List<float> speeds;

    int rangeX = 5;
    int rangeY = 5;
    int numFrames = 100;

    // Start is called before the first frame update
    void Start()
    {
        points = new List<GameObject>();
        points.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
        points.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
        centers = new List<Vector3>();
        centers.Add(new Vector3(4f, 1f, 0f));
        centers.Add(new Vector3(1f, 2f, 0f));
        speeds = new List<float>();
        speeds.Add(2f);
        speeds.Add(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
       float t = 1.0f * ( Time.frameCount - 1) / numFrames;
       points[0].transform.position = new Vector3(nextX(t, centers[0].x, speeds[0]), nextY(t, centers[0].y,speeds[0]));
       points[1].transform.position = new Vector3(nextX(t, centers[1].x, speeds[1]), nextY(t, centers[1].y, speeds[1]));

    }
    
    float nextX(float t, float x, float speed){
    return x + Mathf.Cos(Mathf.PI * 2 *t) * speed;
    }
    float nextY(float t,float y, float speed){
    return y + Mathf.Sin(Mathf.PI * 2*t) * speed;
    }
    
}

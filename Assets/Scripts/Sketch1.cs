/* 
 * written by Jane Fan
 * algorithm adapted from necessarydisorder
 * https://necessarydisorder.wordpress.com/2018/03/31/a-trick-to-get-looping-curves-with-lerp-and-delay/
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sketch1 : MonoBehaviour
{
    List<GameObject> points;
    List<Vector3> centers;  
    List<float> radii;
    List<TubeRenderer> lines;
    int numFrames = 100;
    
    public float delayFactor = 2.0f;
    public int rangeX = 5;
    public int rangeY = 5;
    public int rangeZ = 2;
    public GameObject pointObj;
    public int numPoints = 20;
    public int numLines = 10;
    public TubeRenderer tubeRenderer;
    public int numLinePoints = 30;

    // Start is called before the first frame update
    void Start()
    {
        points = new List<GameObject>();
        centers = new List<Vector3>();
        radii = new List<float>();
        lines = new List<TubeRenderer>();

        for(int i=0; i < numLines; i++) {
            TubeRenderer tr = Instantiate(tubeRenderer,Vector3.zero,Quaternion.identity);
            lines.Add(tr);
        }

        for(int i=0; i < numPoints; i++) {
            points.Add(Instantiate(pointObj) as GameObject);
            centers.Add(new Vector3(Random.Range(-rangeX,rangeX), Random.Range(-rangeY,rangeY), Random.Range(-rangeZ,rangeZ)));
            radii.Add(Random.Range(0.1f,3f));

        }
   }

    // Update is called once per frame
    void Update()
    {
       float t = 1.0f * ( Time.frameCount - 1) / numFrames;
       for(int i=0; i < numPoints; i++){
            points[i].transform.position = new Vector3(nextX(t, centers[i].x, radii[i]), nextY(t, centers[i].y, radii[i]),centers[i].z);
       }

        for (int j = 0; j < numLines; j++){
            Vector3[] line = new Vector3[numLinePoints];
            int a = 0; //choose start point of line
            int b = j; //choose end point of line
            for (int i = 0; i < numLinePoints; i++)
            {
                float tt = 1.0f * i/numLinePoints; //interval between points
                float td = t - delayFactor * tt; //delay to make cool curls
                float te = t - delayFactor * (1- tt);
                float x = Mathf.Lerp(nextX(td, centers[a].x, radii[a]),nextX(te, centers[b].x, radii[b]),tt);
                float y = Mathf.Lerp(nextY(td, centers[a].y, radii[a]),nextY(te, centers[b].y, radii[b]),tt);
                float z = Mathf.Lerp(centers[a].z,centers[b].z,tt);
                line[i] = new Vector3(x,y,z);
            }
            lines[j].SetPoints(line,Color.white);
         }
    }

    float nextX(float t, float x, float radius){
    return x + Mathf.Cos(Mathf.PI * 2 *t) * radius;
    }
    float nextY(float t,float y, float radius){
    return y + Mathf.Sin(Mathf.PI * 2*t) * radius;
    }
    
}

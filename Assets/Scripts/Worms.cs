/* 
 * algorithm adapted from necessarydisorder
 * https://necessarydisorder.wordpress.com/2018/03/31/a-trick-to-get-looping-curves-with-lerp-and-delay/
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worms : MonoBehaviour
{
    private List<GameObject> endPoints;
    private List<Worm> worms;
    private int numFrames = 100;
    private FastNoise fastNoise;
    
    public float delayFactor = 2.0f;
    public int rangeX = 5;
    public int rangeY = 5;
    public int rangeZ = 2;
    public GameObject pointObj;
    public int numWorms = 10;
    public TubeRenderer tubeRenderer;
    public int numLinePoints = 30;

    internal class Worm {
        public Vector3 Centre { get; set; }
        public float MotionRadii { get; set; }
        public float Seed { get; set; }
        public TubeRenderer Renderer { get; set; }
    }

    // Start is called before the first frame update
    private void Start()
    {
        endPoints = new List<GameObject>();
        worms = new List<Worm>();
        fastNoise = new FastNoise();

        for(int i=0; i < numWorms; i++) {
            Worm worm = new Worm();
            worm.Renderer = Instantiate(tubeRenderer,Vector3.zero,Quaternion.identity);
            worm.Centre = new Vector3(Random.Range(-rangeX,rangeX), Random.Range(-rangeY,rangeY), Random.Range(-rangeZ,rangeZ));
            worm.MotionRadii =  Random.Range(30f,60f);
            worm.Seed = Random.Range(1f,9999f);
            worms.Add(worm);
            endPoints.Add(Instantiate(pointObj) as GameObject);
        }
   }

    // Update is called once per frame
    private void Update()
    {
       float t = 1.0f * ( Time.frameCount - 1) / numFrames;

        for (int i = 0; i < numWorms; i++){
            Worm worm = worms[i];

            endPoints[i].transform.position = new Vector3(nextXSimplex(t, worm.Centre.x, worm.MotionRadii, worm.Seed), 
            nextYSimplex(t, worm.Centre.y, worm.MotionRadii, worm.Seed),worm.Centre.z);

            Vector3[] line = new Vector3[numLinePoints];
            int a = 0; //choose start point of line
            int b = i; //choose end point of line
            for (int j = 0; j < numLinePoints; j++)
            {
                float tt = 1.0f * j/numLinePoints; //interval between points
                float td = t - delayFactor * tt; //delay to make cool curls
                float te = t - delayFactor * (1- tt);
                float z = Mathf.Lerp(worms[a].Centre.z,worms[b].Centre.z,tt);
                float x = Mathf.Lerp(nextXSimplex(td, worms[a].Centre.x, worms[a].MotionRadii, worm.Seed),
                                     nextXSimplex(te, worms[b].Centre.x, worms[b].MotionRadii, worm.Seed),tt);
                float y = Mathf.Lerp(nextYSimplex(td, worms[a].Centre.y, worms[a].MotionRadii, worm.Seed),
                                     nextYSimplex(te, worms[b].Centre.y, worms[b].MotionRadii, worm.Seed),tt);
                line[j] = new Vector3(x,y,z);
            }
            worm.Renderer.SetPoints(line,Color.white);
         }
    }

    private float nextX(float t, float x, float radius){
    return x + Mathf.Cos(Mathf.PI * 2 *t) * radius;
    }
    private float nextY(float t,float y, float radius){
    return y + Mathf.Sin(Mathf.PI * 2*t) * radius;
    }

    private float nextXSimplex(float t, float x, float motion_radius, float seed){
        return x + fastNoise.GetSimplex(seed + motion_radius*Mathf.Cos(Mathf.PI*t),
                                        motion_radius*Mathf.Sin(Mathf.PI*t));
    }
    private float nextYSimplex(float t,float y, float motion_radius, float seed){
        return y  + fastNoise.GetSimplex(seed + motion_radius*Mathf.Cos(Mathf.PI*t),
                                         motion_radius*Mathf.Sin(Mathf.PI*t));
    }
    
}

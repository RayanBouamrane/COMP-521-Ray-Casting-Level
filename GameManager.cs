using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    // public LineRenderer line;
    public Material terrainMaterial;

    private static double[] rand1;
    private static double[] rand2;
    private static double[] rand3;
    private static double[] perlinNoise;
    private static int freq = 50;

    private float scale = 0.3f;
    
    private Vector3[] terrainPoints = new Vector3[1000];
    private Vector3[] waterPoints = new Vector3[200];

    public Text leftCannonText;
    public Text rightCannonText;
    
    // Start is called before the first frame update
    void Awake()
    {
        generatePerlinNoise();
        createTerrain();
        createWater();
        createCannons();
        
    }

    void generatePerlinNoise()
    {
        //Different seed every time it's run
        Random r = new Random((int) DateTimeOffset.Now.ToUnixTimeSeconds() );

        int rand1f = freq;
        int rand2f = freq * 2;
        int rand3f = freq * 4;
        int noisef = freq * 100;
        
        //initialize 3 arrays, each with double frequency and half amplitude
        rand1 = new double[rand1f + 1];
        rand2 = new double[rand2f + 1];
        rand3 = new double[rand3f + 1];
        perlinNoise = new double[noisef];
        
        for (int i = 0; i < rand1f; i++) rand1[i] = r.NextDouble() * freq / rand1f;
            
        for (int i = 0; i < rand2f; i++) rand2[i] = r.NextDouble() * freq / rand2f;

        for (int i = 0; i < rand3f; i++) rand3[i] = r.NextDouble() * freq / rand3f;

        for (int i = 0; i < noisef; i++)
        {
            perlinNoise[i] += linInterpolate((i % 100.0) / 100.0, rand1[i / 100], rand1[i / 100 + 1]);
            perlinNoise[i] += linInterpolate((i % 50.0) / 50.0, rand2[i / 50], rand2[i / 50 + 1]);
            perlinNoise[i] += linInterpolate((i % 25.0) / 25.0, rand3[i / 25], rand3[i / 25 + 1]);
            //scale perlinNoise array to store values between -1 and 1
            perlinNoise[i] /= 1.75;
            perlinNoise[i] -= 0.5;
            perlinNoise[i] *= 2.0;
        }

    }
    static double linInterpolate(double t, double a, double b) { return a + t * (b - a); }
    void createTerrain()
    {
        //initialize basic overall terrain shape
        int pos = 0;
        for (int i = 0; i < 100; i++)
        {
            terrainPoints[pos] = new Vector3(-8.9f + i * 0.0337f, -1.65f);
            pos++;
        }
        for (int i = 0; i < 100; i++)
        {
            terrainPoints[pos] = new Vector3(-5.53f + i * 0.0191f, -1.65f + i * 0.0296f);
            pos++;
        }
        for (int i = 0; i < 200; i++)
        {
            terrainPoints[pos] = new Vector3(-3.62f + i * 0.00725f, 1.31f - i * 0.02395f);
            pos++;
        }
        for (int i = 0; i < 200; i++)
        {
            terrainPoints[pos] = new Vector3(-2.17f + i * 0.0217f, -3.48f);
            pos++;
        }
        for (int i = 0; i < 200; i++)
        {
            terrainPoints[pos] = new Vector3(2.17f + i * 0.00725f, -3.48f + i * 0.02395f);
            pos++;
        }
        for (int i = 0; i < 100; i++)
        {
            terrainPoints[pos] = new Vector3(3.62f + i * 0.0191f, 1.31f - i * 0.0296f);
            pos++;
        }
        for (int i = 0; i < 100; i++)
        {
            terrainPoints[pos] = new Vector3(5.53f + i * 0.0337f, -1.65f);
            pos++;
        }

        //add scaled Perlin Noise
        for (int i = 0; i < 1000; i++) { terrainPoints[i].y += (float) perlinNoise[i * freq / 10] * scale; }

        //create LineRenderer
        GameObject ground = new GameObject("Ground") { tag = "Terrain" };
        LineRenderer groundLine = ground.AddComponent<LineRenderer>();
        EdgeCollider2D e = ground.AddComponent<EdgeCollider2D>();
        
        //create collider with same shape as renderer
        var temp = new Vector2[1000];
        for (int i = 0; i < 1000; i++ )
            temp[i] = terrainPoints[i];
        e.points = temp;
        
        groundLine.positionCount = 1000;
        groundLine.startColor = new Color(0.2641509f, 0.1056865f, 0f, 1f);
        groundLine.endColor = new Color(0.2641509f, 0.1056865f, 0f, 1f);
        groundLine.startWidth = 0.1f;
        groundLine.endWidth = 0.1f;
        groundLine.material = terrainMaterial;
        groundLine.SetPositions(terrainPoints);
    }
    void createWater()
    {
        //use terrain to get starting point of water
        Vector3 waterStart = terrainPoints[345];
        for (int i = 0; i < 200; i++)
        {
            waterPoints[i] = new Vector3(waterStart.x + i * 0.0256f, waterStart.y);
            waterPoints[i].y += (float) perlinNoise[i * freq / 10] * scale;
        }

        waterPoints[199] = terrainPoints[655];

        GameObject water = new GameObject("Water") { tag = "Terrain" };
        LineRenderer waterLine = water.AddComponent<LineRenderer>();
        EdgeCollider2D e = water.AddComponent<EdgeCollider2D>();
        
        var temp = new Vector2[200];
        for (int i = 0; i < 200; i++ )
            temp[i] = waterPoints[i];
        e.points = temp;

        waterLine.positionCount = 200;
        waterLine.startColor = Color.blue;
        waterLine.endColor = Color.blue;
        waterLine.startWidth = 0.1f;
        waterLine.endWidth = 0.1f;
        waterLine.material = terrainMaterial;
        waterLine.SetPositions(waterPoints);
    }

    public GameObject cannon;
    private Cannon lComp;
    private Cannon rComp;
    void createCannons()
    {
        //instantiate two cannon objects
        //cannons differ in location, rotation constraints, and left is controllable by default
        var lCannon = Instantiate(cannon, new Vector3(-7f, terrainPoints[56].y+0.5f, 0.0f), Quaternion.Euler(0,0,-45));
        lCannon.name = "Left Cannon";
        lComp = lCannon.GetComponent<Cannon>();
        lComp.maxAngle = 359;
        lComp.minAngle = 270;

        var rCannon = Instantiate(cannon, new Vector3(7f, terrainPoints[944].y+0.5f, 0.0f), Quaternion.Euler(0,0,45));
        rCannon.name = "Right Cannon";
        rComp = rCannon.GetComponent<Cannon>();
        rComp.minAngle = 0;
        rComp.maxAngle = 90;
        
        lComp.isSelected = true;
    }
    
    void Update()
    {
        //switch controlled cannon
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            lComp.isSelected = !lComp.isSelected;
            rComp.isSelected = !rComp.isSelected;
        }

        //update UI with information about muzzle velocity
        leftCannonText.text = "Left Cannon Velocity: " + lComp.muzzleVelocity.ToString("F3");
        rightCannonText.text = "Right Cannon Velocity " + rComp.muzzleVelocity.ToString("F3");
    }
    
    
}
    
    
    


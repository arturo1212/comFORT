using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacameElMunieco : MonoBehaviour
{
    public GameObject ptwindow; 
    public bool jugando = false;
    public int maxEnemies = 50;
    int currentEnemies = 0;
    public GameObject[] enemigos;
    public float spawnHeight = 1f;
    public float timeInterval = 0.5f;
    public GameObject ally;
    float timer;
    public GameObject CameraOVR;

    bool setUp = false;
    MyTerrain leTerrain;
    Vector3 center;
    public float radius;

    // Start is called before the first frame update
    void Start()
    {
        leTerrain = GetComponent<MyTerrain>();
        timer = timeInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (jugando)
        {
            if (!setUp)
            {
                center = (leTerrain.corners[0] + leTerrain.corners[1] + leTerrain.corners[2] + leTerrain.corners[3]) / 4f;
                radius = (center - leTerrain.extendedCorners[0]).magnitude;
                setUp = true;
                Instantiate(ally, center, Quaternion.identity);

            }
            if (timer <= 0 && currentEnemies < maxEnemies)
            {
                int angle = Random.Range(0, 360);
                Vector2 pointInUnitCircle = Random.insideUnitCircle.normalized;
                Vector3 spawnPoint = center + Vector3.up * spawnHeight + new Vector3(pointInUnitCircle.x, 0, pointInUnitCircle.y) * radius;
                int item = Random.Range(0, enemigos.Length);
                GameObject enemigo = enemigos[item];
                Instantiate(enemigo, spawnPoint, Quaternion.LookRotation(-(center - spawnPoint), Vector3.up));
                GameObject pt = Instantiate(ptwindow, spawnPoint, Quaternion.LookRotation((center - spawnPoint), Vector3.up));
                pt.transform.SetParent(CameraOVR.transform);
                currentEnemies++;
            }
            
        }
        UpdateTimer();
    }


    void UpdateTimer()
    {
        if (timer <= 0)
        {
            timer = timeInterval;
        }
        timer -= Time.deltaTime;
    }


    private void FixedUpdate()
    {
        
    }
}

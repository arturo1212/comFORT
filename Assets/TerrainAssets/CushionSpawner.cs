using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CushionSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] cushionObjects;
    public float height = 1.2f;
    public int pillowLimit = 50;
    int currentPillows = 0;

    void Start()
    {
    }
    // Update is called once per frame
    public void DropPillow(Vector3 target)
    {
        if (currentPillows < pillowLimit)
        {
            int item = Random.Range(0, cushionObjects.Length);
            GameObject cushion = cushionObjects[item];
            Instantiate(cushion, target + transform.up * height, Quaternion.identity);
            currentPillows++;
        }

    }

}

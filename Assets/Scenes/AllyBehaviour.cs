using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public enum Cardinals
{
    north=0,
    south=2,
    east=1,
    west=3,
}

public class AllyBehaviour : MonoBehaviour
{
    public float arrivalDistance = 0.5f;
    public bool changeManually = false;
    public Cardinals currentTarget = Cardinals.north;
    MyTerrain myTerrain;
    int[] assignedCorners = { 0, 1, 2, 3 };
    int[] assignedCorners1 = { 1, 2, 3, 0 };
    NavMeshAgent navAgent;
    Vector3 pointA, pointB;
    bool goingA=false, goingB=false;
    // Start is called before the first frame update
    private void OnEnable()
    {
        myTerrain = FindObjectOfType<MyTerrain>();

    }
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (goingA)
        {
            navAgent.SetDestination(pointA);
            if ((transform.position - navAgent.destination).magnitude <= arrivalDistance)
            {
                goingB = true;
                goingA = false;
            }
        }
        else if (goingB)
        {
            navAgent.destination = pointB;
            if ((transform.position - navAgent.destination).magnitude <= arrivalDistance)
            {
                goingB = false;
                goingA = true;
            }
        }


        if (changeManually)
        {
            SetCardinal(currentTarget);
            changeManually = false;
        }
    }

    public void SetCardinal(Cardinals cardinal)
    {
        pointA = myTerrain.corners[assignedCorners[(int)cardinal]];
        pointB = myTerrain.corners[assignedCorners1[(int)cardinal]];
        goingA = true;
    }


}

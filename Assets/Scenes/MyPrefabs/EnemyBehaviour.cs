using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public float timeOfScare = 4f;
    float timer;
    NavMeshAgent navAgent;
    Vector3 target, origin;
    GameObject player;
    public bool scared = false;
    // Start is called before the first frame update
    private void OnEnable()
    {
        timer = timeOfScare;
        origin = transform.position;
    }
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("CenterEyeAnchor");
        target = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!scared)
        {
            target = player.transform.position;
        } else
        {
            if (timer <= 0)
            {
                scared = false;
            }
            target = origin;
        }

        navAgent.destination = target;
        UpdateTimer();
    }

    public void Scare()
    {
        timer = timeOfScare;
        scared = true;
    }

    void UpdateTimer()
    {
        if (timer <= 0)
        {
            timer = timeOfScare;
        }
        timer -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("DecorPillow") || collision.gameObject.name.Contains("miwo"))
        {
            Destroy(gameObject);
        }
    }
}

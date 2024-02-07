using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class TestUnit : MonoBehaviour
{

    public NavMeshAgent agent;

    public float size = 2f;

    Vector3 currentDestination;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update() 
    {

        /*
        if (Vector3.Distance(transform.position, currentDestination) < 20f) {
            agent.ResetPath();
            
        }
        */

    }
}

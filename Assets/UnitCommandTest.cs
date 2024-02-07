using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using System.Linq;

public class UnitCommandTest : MonoBehaviour
{

    public Vector3 currentDestination;

    public float formationSpacing = 10f;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) {
            Vector3 destinationPosition = Hexagons.HexToWorld(new Vector2Int(5, 5));
            currentDestination = destinationPosition;
            TestUnit[] units = FindObjectsOfType<TestUnit>();

            CommandUnitMoveGroup(units, currentDestination);
        }        
    }


    private float CalculatePathLength(NavMeshPath path)
    {
        float pathLength = 0;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return pathLength;
    }

    public void CommandUnitMoveGroup(TestUnit[] units, Vector3 destinationPosition) {

        NavMeshPath[] paths = new NavMeshPath[units.Length];
        int[] sortIndex = new int[units.Length];

        for (int i = 0; i < units.Length; i++)
        {

            TestUnit currentUnit = units[i];

            NavMeshPath path = new NavMeshPath();
            currentUnit.agent.CalculatePath(destinationPosition, path);
            paths[i] = path;

        }

        int[] sortedIndices = Enumerable.Range(0, units.Length).OrderBy(i => CalculatePathLength(paths[i])).ToArray();

        for (int i = 0; i < sortedIndices.Length; i++) {
            int index = sortedIndices[i];
            NavMeshPath path = paths[index];
            Debug.Log(path.status);
            Vector3 approachDirection = (path.corners[path.corners.Length - 1] - path.corners[path.corners.Length - 2]).normalized;

            Debug.DrawLine(destinationPosition, destinationPosition - (approachDirection * 10f), Color.red, 5f);

            if (index == 0) {
                units[index].agent.SetDestination(destinationPosition);
            } else {
                Vector3 adjustedFormationOffset = Quaternion.Euler(0, 90, 0) * approachDirection * (index * formationSpacing);
                Vector3 newDestination = destinationPosition - adjustedFormationOffset;

                units[index].agent.SetDestination(newDestination);
            }

            

        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AElevator : MonoBehaviour
{

    [Tooltip("Depth of the elevator shaft underground")]
    public int depth;

    [Tooltip("Height of the elevator shaft above ground")]
    public int height;

    [Tooltip("Current position of the elevator. Negative values are underground, positive values are above ground")]
    public float carLocation;

    [Tooltip("The current destination of the car")]
    public int carTarget;
    
    public float carTargetPosition;

    [Tooltip("The list of elevator floor pickup requests")]
    public List<int> elevatorCalls = new List<int>();

    [Tooltip("The list of elevator floor destination requests")]
    public List<int> elevatorDestinations = new List<int>();

    [Tooltip("The capacity of the car")]
    public int carCapacity;

    [Tooltip("The speed of the car")]
    public float carSpeed;

    [Tooltip("Is the car loading right now?")]
    public bool loading;

    [Tooltip("Transform of the elevator car visual")]
    public Transform carTransform;

    Coroutine movementRoutine;

    IEnumerator MoveCarToDestination() {

        // this moves the elevator between wherever it is and wherever the current carTarget is
        // it does not honor elevatorCalls or elevatorDestinations
        float slowestSpeed = 0.1f;

        while (carTransform.localPosition.y != carTargetPosition) {

            // adjust speed to slow down when approaching destination
            float moveSpeed = carSpeed;
            float distance = Mathf.Abs(carTargetPosition - carTransform.localPosition.y);
            if (distance < 0.5f)
            {
                moveSpeed *= Mathf.Max(slowestSpeed, distance / 0.5f); // make sure the speed isn't too slow
            }
            float deltaMove = Time.deltaTime * moveSpeed;

            float newLocationY;
            if (carTransform.localPosition.y < carTargetPosition) {
                newLocationY = Mathf.Clamp(carTransform.localPosition.y + deltaMove, carTransform.localPosition.y, carTargetPosition);
            } else {
                newLocationY = Mathf.Clamp(carTransform.localPosition.y - deltaMove, carTargetPosition, carTransform.localPosition.y);
            }

            carTransform.localPosition = new Vector3(carTransform.localPosition.x, newLocationY, carTransform.localPosition.z);

            yield return new WaitForEndOfFrame();
        }

        movementRoutine = null;

    }

    public void ExpandUnderground() {

        if (depth < BaseLayers.current.maxDepth)
        {
            depth++;
            BaseLayers.current.ClearPosition(depth, new Vector2Int(0, 0));
        }
    }

    public void ExpandAboveground() {
        height++;
    }

    public void RequestMove(int floor) {

        carTarget = floor;
        carTargetPosition = BaseLayers.current.layerOffsets[floor];

        if (movementRoutine != null) {
            StopCoroutine(movementRoutine);
        }
        StartCoroutine(MoveCarToDestination());
    }

    // Start is called before the first frame update
    void Start()
    {
        movementRoutine = StartCoroutine(MoveCarToDestination());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            ExpandUnderground();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RequestMove(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RequestMove(depth);
        }
    }
}

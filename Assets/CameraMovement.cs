using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float rotation = 0.5f;
    public float rotationSpeed = 5f;

    public Vector3 lowRotation = new Vector3(-10f, 0f, 0f);
    public Vector3 highRotation = new Vector3(-30f, 0f, 0f);

    public float zoomFovLow = 30f;
    public float zoomFovHigh = 50f;

    public Camera controlledCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 moveVector;
        if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f) {
            moveVector.x = Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f);
            moveVector.y = 0f;
            moveVector.z = Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 1f);
            transform.position = transform.position + (moveVector * moveSpeed * Time.deltaTime);
        }

        if (Input.mouseScrollDelta.y != 0) {
            rotation = Mathf.Clamp(rotation + Input.mouseScrollDelta.y * Time.deltaTime * rotationSpeed, 0, 1f);
        }

        transform.rotation = Quaternion.Lerp(Quaternion.Euler(lowRotation), Quaternion.Euler(highRotation), rotation);

        controlledCamera.fieldOfView = Mathf.Lerp(zoomFovHigh, zoomFovLow, rotation);

    }
}

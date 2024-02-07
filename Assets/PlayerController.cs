using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    int currentLayer = 0;

    public Transform cameraBoom;

    public float rotationSpeed;

    float cameraAngle = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0) {
            cameraAngle = Mathf.Repeat(cameraAngle + Input.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed, 360f);
        }
        Quaternion newRotation = Quaternion.Euler(0, cameraAngle, 0);
        cameraBoom.transform.rotation = newRotation;



    }
}

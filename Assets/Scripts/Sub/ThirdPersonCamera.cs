using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {
    public bool lockCursor;
    public float mouseSensitivityX = 6;
    public float mouseSensitivityY = 5;
    public Transform target;
    public Vector2 pitchMinMax = new Vector2(-60, 90);

    //For camera collision
    public float minDistance = 1.0f;
    public float maxDistance = 4.0f;
    public float distance;

    public float rotationSmoothTime = .12f;
    public float rotationSmoothCameraT = 0.01f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;
    Vector3 smoothedPos;
    Vector3 rotationSmoothCameraV;

    float yaw;
    float pitch;


    void Start(){
        //If in the user interface we ticked lock cursor, then lock curser and hide it.
        if (lockCursor){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

	// Update is called once per frame
	void Update () {
        //increase yaw (xaxis) by the mouse's X pos and multiply that by the mouse sentitivity (do same for Y axis, but inverted since that feels better)
        yaw += Input.GetAxis("Mouse X") * mouseSensitivityX;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY;
        //Clamp pitch between what we set the max and miin in the vector 2.
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        //Current rotation gets smoothdamped by making a new vector3 of the pitch and yaw which we calculated earlier smoothed by rotation smooth time
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);

        //turn the camera to currentRotation
        transform.eulerAngles = currentRotation;

        //Reposition the camera to be old position
        //So the way it works it it takes the target position, which is the child or the object we want to rotate around...
        //... and then since we have already rotated the camera, we just tell it to on its blue axis a distance of x. The minus makes it move backwards.
        Vector3 desiredPos = target.position - transform.forward * maxDistance;


        RaycastHit hit;
        if (Physics.Linecast(target.position, desiredPos, out hit))
        {
            distance = Mathf.Clamp((hit.distance * 0.9f), minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }
        Debug.DrawLine(target.position, desiredPos);


        transform.position = target.position - transform.forward * distance;
    }
}

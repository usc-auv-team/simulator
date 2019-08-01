using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public Transform objectTarget = null;
    public Transform cameraTarget = null;
    public float xSpeed = 100f;
    public float ySpeed = 100.0f;
    public float yMinLimit = -90f;
    public float yMaxLimit = 90f;
    public float smoothTime = 50f;

    public float zoomSpeed = 3.0f;
    public float zoomMin = -1.0f;
    public float zoomMax = -10.0f;

    Vector3 prevPos = Vector3.zero;

    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;
    float velocityX = 0.0f;
    float velocityY = 0.0f;

    void Start() {

        if (!objectTarget) {
            Debug.LogError("Did not find object target.");
        }

        if (!cameraTarget) {
            Debug.LogError("Did not find camera target.");
        }

        rotationYAxis = transform.eulerAngles.y;
        rotationXAxis = transform.eulerAngles.x;
    }

    void LateUpdate() {
        // Get the camera position before applying transformations
        prevPos = cameraTarget.position;

        ZoomCamera();
        MoveCamera();

        // Check if the updated position is different from the previous position
        cameraTarget.hasChanged = !(prevPos == cameraTarget.position);
    }

    // If right mouse button is held down, change rotation of this object
    // to follow path of mouse
    void MoveCamera() {

        if (Input.GetMouseButton(1)) {
            velocityX += xSpeed * Input.GetAxis("Mouse X") * 0.02f;
            velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
        }

        rotationYAxis += velocityX;
        rotationXAxis -= velocityY;
        rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
        Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
        Quaternion rotation = toRotation;

        transform.rotation = rotation;
        velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
        velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
    }

    // If right mouse button is held down, change translation of camera
    // to move it closer or farther
    void ZoomCamera() {

        if (Input.GetMouseButton(1)) {

            Vector3 position = cameraTarget.transform.localPosition;
            float scrollDelta = Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed;

            position.z += scrollDelta;
            if (position.z > zoomMin) { position.z = zoomMin; }
            if (position.z < zoomMax) { position.z = zoomMax; }

            cameraTarget.transform.localPosition = position;
        }
    }

    // Keeps given angle with range (-360, 360) and supplied min and max
    float ClampAngle(float angle, float min, float max) {

        if (angle < -360f) { angle += 360f; }
        if (angle > 360f) { angle -= 360f; }

        return Mathf.Clamp(angle, min, max);
    }
}

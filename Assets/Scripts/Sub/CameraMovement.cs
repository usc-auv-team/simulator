using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    [SerializeField] private Transform cameraTarget = null;
    [SerializeField] private float xSpeed = 100f;
    [SerializeField] private float ySpeed = 100.0f;
    [SerializeField] private float yMinLimit = -90f;
    [SerializeField] private float yMaxLimit = 90f;
    [SerializeField] private float smoothTime = 50f;

    [SerializeField] private float zoomSpeed = 3.0f;
    [SerializeField] private float zoomMin = -1.0f;
    [SerializeField] private float zoomMax = -10.0f;

    private Vector3 prevPos = Vector3.zero;

    private float rotationYAxis = 0.0f;
    private float rotationXAxis = 0.0f;
    private float velocityX = 0.0f;
    private float velocityY = 0.0f;

    private void Start() {
        rotationYAxis = transform.eulerAngles.y;
        rotationXAxis = transform.eulerAngles.x;
    }

    private void LateUpdate() {

        // Save camera position before applying transformations
        prevPos = cameraTarget.position;

        ZoomCamera();
        MoveCamera();

        // Check if the updated position is different from the previous position
        cameraTarget.hasChanged = !(prevPos == cameraTarget.position);
    }

    // If right mouse button is held down, rotate the object so the camera follows it
    private void MoveCamera() {

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

    // If right mouse button is held down, move the camera closer or farther
    private void ZoomCamera() {

        if (Input.GetMouseButton(1)) {

            Vector3 newCameraPosition = cameraTarget.transform.localPosition;

            float scrollDelta = Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed;
            newCameraPosition.z += scrollDelta;

            if (newCameraPosition.z > zoomMin) { newCameraPosition.z = zoomMin; }
            else if (newCameraPosition.z < zoomMax) { newCameraPosition.z = zoomMax; }

            cameraTarget.transform.localPosition = newCameraPosition;
        }
    }

    // Keeps given angle with range [-360, 360] then between [min, max]
    private static float ClampAngle(float angle, float min, float max) {

        if (angle < -360f) { angle += 360f; }
        else if (angle > 360f) { angle -= 360f; }

        return Mathf.Clamp(angle, min, max);
    }
}

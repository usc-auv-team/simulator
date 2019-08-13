using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private GameObject cam = null;

    [SerializeField] private float xSpeed = 100f;
    [SerializeField] private float ySpeed = 100.0f;
    [SerializeField] private float pitchMinLimit = -90f;
    [SerializeField] private float pitchMaxLimit = 90f;
    [SerializeField] private float smoothTime = 50f;

    [SerializeField] private float zoomSpeed = 3.0f;
    [SerializeField] private float zoomMin = -1.0f;
    [SerializeField] private float zoomMax = -10.0f;

    private Vector3 prevPos = Vector3.zero;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private float velocityHorizontal = 0.0f;
    private float velocityVertical = 0.0f;

    private void Start() {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    private void LateUpdate() {

        // Save camera position before applying transformations
        prevPos = cam.transform.position;

        ZoomCamera();
        MoveCamera();

        // Check if the updated position is different from the previous position
        cam.transform.hasChanged = !(prevPos == cam.transform.position);
    }

    // If right mouse button is held down, rotate the object so the camera follows it
    private void MoveCamera() {

        if (Input.GetMouseButton(1)) {
            velocityHorizontal += xSpeed * Input.GetAxis("Mouse X") * 0.02f;
            velocityVertical += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
        }

        yaw += velocityHorizontal;
        pitch -= velocityVertical;
        pitch = ClampAngle(pitch, pitchMinLimit, pitchMaxLimit);
        Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        Quaternion toRotation = Quaternion.Euler(pitch, yaw, 0);
        Quaternion rotation = toRotation;

        transform.rotation = rotation;
        velocityHorizontal = Mathf.Lerp(velocityHorizontal, 0, Time.deltaTime * smoothTime);
        velocityVertical = Mathf.Lerp(velocityVertical, 0, Time.deltaTime * smoothTime);
    }

    // If right mouse button is held down, move the camera closer or farther
    private void ZoomCamera() {

        if (Input.GetMouseButton(1)) {

            Vector3 newCameraPosition = cam.transform.localPosition;

            float scrollDelta = Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed;
            newCameraPosition.z += scrollDelta;

            if (newCameraPosition.z > zoomMin) { newCameraPosition.z = zoomMin; }
            else if (newCameraPosition.z < zoomMax) { newCameraPosition.z = zoomMax; }

            cam.transform.localPosition = newCameraPosition;
        }
    }

    // Keeps given angle with range [-360, 360] then between [min, max]
    private static float ClampAngle(float angle, float min, float max) {

        if (angle < -360f) { angle += 360f; }
        else if (angle > 360f) { angle -= 360f; }

        return Mathf.Clamp(angle, min, max);
    }
}
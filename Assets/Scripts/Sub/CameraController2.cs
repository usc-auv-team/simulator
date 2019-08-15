using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2 : MonoBehaviour {

    // ******************************************************
    // Child camera components

    private class CameraComponents {
        public Transform transform = null;
        public Camera camera = null;
    }

    private CameraComponents camReference = new CameraComponents();

    // ******************************************************
    // QoL Fields

    [SerializeField] private bool debug = false;

    // ******************************************************
    // Fields related to Orbiting

    [SerializeField] private float xSpeed = 100f;
    [SerializeField] private float ySpeed = 100.0f;
    private float pitchMinLimit = -89f;
    private float pitchMaxLimit = 89f;
    [SerializeField] private float smoothTime = 25f;
    private float yaw = 0f;
    private float pitch = 0f;
    private float velocityHorizontal = 0f;
    private float velocityVertical = 0f;

    // ******************************************************
    // Fields related to Zooming

    [SerializeField] private float zoomSpeed = 3.0f;
    private float scrollDelta = 0f;
    [SerializeField] private float minimumDistance = 1f;
    [SerializeField] private float maximumDistance = 10f;

    // ******************************************************
    // Fields related to the (ghost) camera transform

    private class MiniTransform {
        public Vector3 pos = Vector3.zero;
        public Quaternion rot = Quaternion.identity;
    }

    private MiniTransform cam;

    private float distance = 0f;

    // ******************************************************
    // Monobehavior Methods

    private void Start() {
        camReference.transform = GetComponentsInChildren<Transform>()[1];
        camReference.camera = GetComponentInChildren<Camera>();

        cam = new MiniTransform {
            pos = camReference.transform.position,
            rot = camReference.transform.rotation
        };

        yaw = cam.rot.eulerAngles.y;
        pitch = cam.rot.eulerAngles.x;
        distance = Vector3.Distance(transform.position, cam.pos);
    }

    private void LateUpdate() {
        ListenForInput();
        Zoom();
        Orbit();
        UpdateCameraReference();
    }

    // ******************************************************
    // Private Methods

    // Check for input and update any related fields
    private void ListenForInput() {

        if (Input.GetMouseButton(1)) {
            velocityHorizontal += xSpeed * Input.GetAxis("Mouse X") * 0.02f;
            velocityVertical += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
        }

        if (Input.GetKey(KeyCode.LeftArrow)) {
            velocityHorizontal = 1f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow)) {
            velocityHorizontal = 0f;
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            velocityHorizontal = -1f;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow)) {
            velocityHorizontal = 0f;
        }

        if (Input.GetKey(KeyCode.DownArrow)) {
            velocityVertical = 1f;
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow)) {
            velocityVertical = 0f;
        }

        if (Input.GetKey(KeyCode.UpArrow)) {
            velocityVertical = -1f;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow)) {
            velocityVertical = 0f;
        }

        if (Input.GetMouseButton(1)) {
            scrollDelta = Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed;
        }

    }

    // Moves the ghost camera closer/further by changing the distance
    private void Zoom() {
        distance -= scrollDelta;
        distance = Mathf.Clamp(distance, minimumDistance, maximumDistance);
    }

    // Orbit the ghost camera around the pivot by calculating
    // its position and rotation
    private void Orbit() {
        
        yaw -= velocityHorizontal;
        pitch -= velocityVertical;

        yaw = ClampAngle(yaw, -360f, 360f);
        pitch = ClampAngle(pitch, pitchMinLimit, pitchMaxLimit);

        float theta = (yaw - 90f) * Mathf.Deg2Rad;
        float phi = pitch * Mathf.Deg2Rad;
        
        cam.pos = transform.position + new Vector3(
            distance * Mathf.Cos(phi) * Mathf.Cos(theta),
            distance * Mathf.Sin(phi),
            distance * Mathf.Cos(phi) * Mathf.Sin(theta)
        );

        Vector3 forward = (transform.position - cam.pos).normalized;

        cam.rot = Quaternion.LookRotation(forward);

        velocityHorizontal = 0f;
        velocityVertical = 0f;

    }

    private void UpdateCameraReference() {
        camReference.transform.SetPositionAndRotation(cam.pos, cam.rot);
    }

    // ******************************************************
    // Private Static Methods

    // Keeps given angle with range (-360, 360) then between [min, max]
    private static float ClampAngle(float angle, float min, float max) { 
        while (angle < -360f) { angle += 360f; }
        while (angle > 360f) { angle -= 360f; }
        return Mathf.Clamp(angle, min, max);
    }

}

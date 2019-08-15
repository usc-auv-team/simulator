using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2 : MonoBehaviour {

    [SerializeField] private GameObject cam = null;

    [SerializeField] private bool debug = false;

    [SerializeField] private float xSpeed = 100f;
    [SerializeField] private float ySpeed = 100.0f;
    private float pitchMinLimit = -89f;
    private float pitchMaxLimit = 89f;
    [SerializeField] private float smoothTime = 25f;

    private float yaw = 0f;
    private float pitch = 0f;
    private float velocityHorizontal = 0f;
    private float velocityVertical = 0f;

    [SerializeField] private float zoomSpeed = 3.0f;
    [SerializeField] private float minimumDistance = -1.0f;
    [SerializeField] private float maximumDistance = -10.0f;

    private float distance = 0f;

    private void Start() {
        yaw = cam.transform.eulerAngles.y;
        pitch = cam.transform.eulerAngles.x;
        distance = Vector3.Distance(transform.position, cam.transform.position);
    }

    private void LateUpdate() {
        Orbit();
    }

    private void Orbit() {

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
        
        yaw -= velocityHorizontal;
        pitch -= velocityVertical;

        yaw = ClampAngle(yaw, -360f, 360f);
        pitch = ClampAngle(pitch, pitchMinLimit, pitchMaxLimit);

        float theta = (yaw - 90f) * Mathf.Deg2Rad;
        float phi = pitch * Mathf.Deg2Rad;
        
        cam.transform.position = transform.position + new Vector3(
            distance * Mathf.Cos(phi) * Mathf.Cos(theta),
            distance * Mathf.Sin(phi),
            distance * Mathf.Cos(phi) * Mathf.Sin(theta)
        );

        Vector3 forward = (transform.position - cam.transform.position).normalized;

        DrawDirection(forward, Vector3.up);

        cam.transform.rotation = Quaternion.LookRotation(forward);

        velocityHorizontal = 0f;
        velocityVertical = 0f;

    }

    private void DrawDirection(Vector3 forward, Vector3 up) {
        float mag = 5f;
        Debug.DrawRay(transform.position, forward * mag, Color.blue);
        Debug.DrawRay(transform.position, up * mag, Color.green);
        Debug.DrawRay(transform.position, -Vector3.Cross(forward, up) * mag, Color.red);
    }
    
    // Keeps given angle with range [-360, 360] then between [min, max]
    private static float ClampAngle(float angle, float min, float max) { 
        while (angle < -360f) { angle += 360f; }
        while (angle > 360f) { angle -= 360f; }
        return Mathf.Clamp(angle, min, max);
    }

}

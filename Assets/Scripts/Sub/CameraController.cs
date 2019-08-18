using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

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
    // [SerializeField] private float smoothTime = 25f;
    private float yaw = 0f;
    private float pitch = 0f;
    private float velocityHorizontal = 0f;
    private float velocityVertical = 0f;

    // ******************************************************
    // Fields related to Zooming

    [SerializeField] private float zoomSpeed = 3.0f;
    private float scrollDelta = 0f;
    [SerializeField] private float minimumDistance = 0.7f;
    [SerializeField] private float maximumDistance = 10f;

    // ******************************************************
    // Fields related to the (ghost) camera transform

    private class MiniTransform {
        public Vector3 pos = Vector3.zero;
        public Quaternion rot = Quaternion.identity;
    }

    private MiniTransform cam;

    private float distanceActual = 0f;
    private float distanceDesired = 0f;
    private float distancePreOccluded = 0f;

    // ******************************************************
    // Fields related to Camera Collision

    [SerializeField] private float incrementDistance = 0.05f;
    [SerializeField] private int incrementMaxSteps = 50;

    private class KeyCameraPoints {
        public Vector3 TopLeft;
        public Vector3 TopRight;
        public Vector3 BotLeft;
        public Vector3 BotRight;
        public Vector3 Back;
    }

    private class OcclusionData {
        public bool isOccluded = false;
        public float distance = float.MaxValue;
    }

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

        distanceActual = Vector3.Distance(transform.position, camReference.transform.position);
        distanceDesired = distanceActual;
    }

    private void LateUpdate() {
        distanceActual = Vector3.Distance(transform.position, camReference.transform.position);

        ListenForInput();

        Zoom();
        Orbit();

        if (debug) { DrawKeyCameraPoints(CalculateKeyCameraPoints(cam.pos)); }

        FixCameraCollision();
        ResetToPreOccluded();

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

    // Move the ghost camera closer/further by changing the distance
    private void Zoom() {
        distanceDesired -= scrollDelta;
        // if zoomed while in occluded state, then disable resetting
        if (scrollDelta != 0f) { distancePreOccluded = 0f; }
    }

    // Orbit the ghost camera around the pivot
    private void Orbit() {
        
        yaw -= velocityHorizontal;
        pitch -= velocityVertical;

        yaw = ClampAngle(yaw, -360f, 360f);
        pitch = ClampAngle(pitch, pitchMinLimit, pitchMaxLimit);

        MiniTransform newTransform = CalculateTransformation(yaw, pitch, distanceDesired);

        cam.pos = newTransform.pos;
        cam.rot = newTransform.rot;

        velocityHorizontal = 0f;
        velocityVertical = 0f;
    }

    // Takes in the given camera information and returns a position and rotation for it
    private MiniTransform CalculateTransformation(float yaw, float pitch, float dist) {

        MiniTransform output = new MiniTransform();

        float radYaw = (yaw - 90f) * Mathf.Deg2Rad;
        float radPitch = pitch * Mathf.Deg2Rad;

        output.pos = transform.position + new Vector3(
            dist * Mathf.Cos(radPitch) * Mathf.Cos(radYaw),
            dist * Mathf.Sin(radPitch),
            dist * Mathf.Cos(radPitch) * Mathf.Sin(radYaw)
        );

        output.rot = Quaternion.LookRotation((transform.position - output.pos).normalized);

        return output;
    }

    // Moves the camera closer if it is being occluded
    // Then tries to move it back once its no longer occluded
    private void FixCameraCollision() {

        OcclusionData occ = GetOcclusion(cam.pos);
        if (occ.isOccluded) {
            if (distancePreOccluded == 0f) {
                // if the camera has just been occluded, save its
                // distance for eventual resetting
                distancePreOccluded = distanceActual;
            }
            distanceDesired = CalculateBetterDistance(occ.distance);
        }
    }

    // Check if a given camera position is being occluded using linecasts
    private OcclusionData GetOcclusion(Vector3 camPos) {

        KeyCameraPoints keyPoints = CalculateKeyCameraPoints(camPos);
        OcclusionData output = new OcclusionData();

        Vector3[] arrPoints = {
            keyPoints.TopLeft,
            keyPoints.TopRight,
            keyPoints.BotLeft,
            keyPoints.BotRight,
            keyPoints.Back
        };

        // for each point, if there's a raycast hit, save the smallest distance between them all
        foreach (Vector3 point in arrPoints) {
            if (Physics.Linecast(transform.position, point, out RaycastHit hit) && hit.collider.tag != "Player") {
                output.distance = (hit.distance < output.distance) ? hit.distance : output.distance;
            }
        }

        if (output.distance < float.MaxValue) {
            output.isOccluded = true;
        }

        return output;
    }

    // Calculate the near clip plane points and back point from the given camera position
    private KeyCameraPoints CalculateKeyCameraPoints(Vector3 camPos) {

        KeyCameraPoints points = new KeyCameraPoints();

        // these are technically half the height and width of the plane, but doesn't matter
        float distance = camReference.camera.nearClipPlane;
        float height = distance * Mathf.Tan(camReference.camera.fieldOfView * Mathf.Deg2Rad * 0.5f);
        float width = height * camReference.camera.aspect;

        Vector3 forward = cam.rot * Vector3.forward;
        Vector3 right = cam.rot * Vector3.right;
        Vector3 up = cam.rot * Vector3.up;

        // update the vertices that make the camera collision pyramid

        points.BotRight = camPos + right * width;
        points.BotRight -= up * height;
        points.BotRight += forward * distance;

        points.BotLeft = camPos - right * width;
        points.BotLeft -= up * height;
        points.BotLeft += forward * distance;

        points.TopRight = camPos + right * width;
        points.TopRight += up * height;
        points.TopRight += forward * distance;

        points.TopLeft = camPos - right * width;
        points.TopLeft += up * height;
        points.TopLeft += forward * distance;

        points.Back = camPos + forward * -distance;

        return points;
    }

    // When occluded, find a closer distance to not be
    // eventually returning the parameter distance that was always safe
    private float CalculateBetterDistance(float dist) {

        // copy the starting distance
        float nudgedDistance = distanceActual;

        for (int i = 0; i < incrementMaxSteps; i++) {

            // nudge it closer to the origin
            nudgedDistance -= incrementDistance;
            Vector3 nudged = Vector3.MoveTowards(transform.position, cam.pos, nudgedDistance);

            if (!GetOcclusion(nudged).isOccluded) {
                return nudgedDistance;
            }
        }

        // if nudge was always occluded, give up
        return dist;
    }

    // Checks to make sure the pre occluded distance isn't being occluded,
    // and will change the camera distance to go back to its original once it's not
    private void ResetToPreOccluded() {

        // if not in reset state, then don't do anything
        if (distancePreOccluded == 0f) { return; }

        Vector3 prePos = CalculateTransformation(yaw, pitch, distancePreOccluded).pos;

        if (debug) { DrawKeyCameraPoints(CalculateKeyCameraPoints(prePos)); }

        OcclusionData occ = GetOcclusion(prePos);

        if (!occ.isOccluded) {
            // if point is longer occluded, then go back to it
            distanceDesired = distancePreOccluded;
        }
        else if (occ.distance <= distancePreOccluded && occ.distance > distanceDesired) {
            // if point is occluded, then get as close as possible
            distanceDesired = occ.distance;
        }

        if (distanceDesired == distancePreOccluded) {
            // if finally reached point, no longer need to try to reset
            distancePreOccluded = 0f;
        }
    }

    // Debug tool; draw the visualization of the KeyCameraPoints
    private void DrawKeyCameraPoints(KeyCameraPoints points) {
        Debug.DrawLine(transform.position, points.Back, Color.blue);

        Debug.DrawLine(transform.position, points.TopLeft, Color.red);
        Debug.DrawLine(transform.position, points.TopRight, Color.red);
        Debug.DrawLine(transform.position, points.BotLeft, Color.red);
        Debug.DrawLine(transform.position, points.BotRight, Color.red);

        Debug.DrawLine(points.TopLeft, points.TopRight, Color.red);
        Debug.DrawLine(points.TopRight, points.BotRight, Color.red);
        Debug.DrawLine(points.BotRight, points.BotLeft, Color.red);
        Debug.DrawLine(points.BotLeft, points.TopLeft, Color.red);

        Debug.DrawLine(points.TopLeft, points.Back, Color.red);
        Debug.DrawLine(points.TopRight, points.Back, Color.red);
        Debug.DrawLine(points.BotRight, points.Back, Color.red);
        Debug.DrawLine(points.BotLeft, points.Back, Color.red);
    }

    // Update the real camera with the ghost camera
    private void UpdateCameraReference() {

        distanceDesired = Mathf.Clamp(distanceDesired, minimumDistance, maximumDistance);

        float velocity = 0f;
        distanceActual = Mathf.SmoothDamp(distanceActual, distanceDesired, ref velocity, 0.01f);

        cam.pos = Vector3.MoveTowards(transform.position, cam.pos, distanceActual);
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

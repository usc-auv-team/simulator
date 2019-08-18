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

        if (debug) {
            DrawKeyCameraPoints(cam.pos);
            // OutputStatus();
        }

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

    // If camera occluded, then move it closer to not be
    private void FixCameraCollision() {

        OcclusionData occ = GetOcclusion(cam.pos);
        if (occ.isOccluded) {
            if (distancePreOccluded == 0f) {
                // if the camera has just been occluded, save its
                // distance for eventual resetting
                distancePreOccluded = distanceActual;
            }
            distanceDesired = CalculateSaferDistance(occ.distance, minimumDistance);
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

    // Given an initial distance, find a distance towards
    // a desired distance that is not occluded
    private float CalculateSaferDistance(float initial, float desired) {

        if (initial == desired) {
            return initial;
        }

        float direction = Mathf.Sign(desired - initial);

        // with direction in mind, find a point between initial and desired
        // that is not occluded using small increments of distance
        if (direction == -1f) {

            for (float i = initial; i > desired; i -= 0.01f) {
                if (!GetOcclusion(GetPosition(i)).isOccluded) {
                    return i;
                }
            }

        }
        else if (direction == 1f) {

            for (float i = initial; i < desired; i += 0.01f) {
                if (!GetOcclusion(GetPosition(i)).isOccluded) {
                    return i;
                }
            }

        }

        // if everything was occluded, then just return desired
        return desired;
    }

    // Constantly moves camera back towards pre-collision point
    // and when it succeeds, will stop further resetting
    private void ResetToPreOccluded() {

        // if not in reset state, then don't do anything
        if (distancePreOccluded == 0f) { return; }

        if (debug) { DrawKeyCameraPoints(GetPosition(distancePreOccluded)); }
        OcclusionData occ = GetOcclusion(GetPosition(distancePreOccluded));

        if (!occ.isOccluded) {
            // if no longer occluded, then stop further resetting
            distanceDesired = distancePreOccluded;
            distancePreOccluded = 0f;
            return;
        }

        // we have hit distance but could be occluded, so find a distance that isn't
        distanceDesired = CalculateSaferDistance(occ.distance, distanceDesired);
    }

    // Return a camera position given the distance
    private Vector3 GetPosition(float distance) {
        return transform.position + ((cam.pos - transform.position).normalized * distance);
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
    // Debug Methods

    // Debug tool; draw the visualization of the KeyCameraPoints
    private void DrawKeyCameraPoints(Vector3 position) {
        KeyCameraPoints points = CalculateKeyCameraPoints(position);

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

    private void OutputStatus() {
        string message = "";

        message += "Occlusion State: " + (distancePreOccluded != 0f).ToString() + "\n";

        if (distancePreOccluded != 0f) {
            OcclusionData occ = GetOcclusion(cam.pos);
            message += "Actual  : " + distanceActual.ToString() + "\n";
            message += "Desired : " + distanceDesired.ToString() + "\n";
            message += "PreOcc  : " + distancePreOccluded.ToString() + "\n";

            if (occ.distance == float.MaxValue) {
                message += "Occ     : MaxValue\n";
            }
            else {
                message += "Occ     : " + occ.distance.ToString() + "\n";
            }

            message += "Better  : " + CalculateSaferDistance(distanceActual, occ.distance).ToString() + "\n";
        }

        Debug.Log(message);
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

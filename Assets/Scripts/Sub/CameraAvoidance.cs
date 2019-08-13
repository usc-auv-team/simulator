using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAvoidance : MonoBehaviour
{
    [SerializeField] private GameObject sub = null;
    [SerializeField] private Camera cam = null;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private bool debug = false;

    [SerializeField] private float incrementDistance = 0.05f;
    [SerializeField] private int incrementMaxSteps = 100;

    private float minimumDistance = 1f;
    private float savedDistance = -1f;
    private float currDistance = -1f;

    private struct KeyCameraPoints {
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

    private Vector3 subPos = Vector3.zero;
    private Vector3 camPos = Vector3.zero;


    private void LateUpdate() {
        subPos = sub.transform.position;
        camPos = cam.transform.position;

        if (debug) { DrawKeyPoints(CalculateKeyCameraPoints(camPos)); }

        CheckToAdjustCamera();
    }

    private void CheckToAdjustCamera() {

        float currDistance = Vector3.Distance(subPos, camPos);

        OcclusionData occ = GetOcclusion(camPos);

        if (!occ.isOccluded) {
            // if not occluded, don't do anything else
            return;
        }

        float newDistance = CalculateBetterDistance(occ.distance);

        UpdateCamera(newDistance);

    }



    private KeyCameraPoints CalculateKeyCameraPoints(Vector3 cameraPoint) {

        KeyCameraPoints points = new KeyCameraPoints();

        // these are technically half the height and width of the plane, but doesn't matter
        float distance = cam.nearClipPlane;
        float height = distance * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f);
        float width = height * cam.aspect;

        // update the vertices that make the camera collision pyramid

        points.BotRight = cameraPoint + transform.right * width;
        points.BotRight -= transform.up * height;
        points.BotRight += transform.forward * distance;

        points.BotLeft = cameraPoint - transform.right * width;
        points.BotLeft -= transform.up * height;
        points.BotLeft += transform.forward * distance;

        points.TopRight = cameraPoint + transform.right * width;
        points.TopRight += transform.up * height;
        points.TopRight += transform.forward * distance;

        points.TopLeft = cameraPoint - transform.right * width;
        points.TopLeft += transform.up * height;
        points.TopLeft += transform.forward * distance;

        points.Back = cameraPoint + transform.forward * -distance;

        return points;
    }

    private OcclusionData GetOcclusion(Vector3 cameraPos) {

        KeyCameraPoints points = CalculateKeyCameraPoints(cameraPos);

        RaycastHit hit;

        OcclusionData output = new OcclusionData();

        Vector3 outerSub = Vector3.MoveTowards(subPos, camPos, minimumDistance);

        if (Physics.Linecast(outerSub, points.TopLeft, out hit, layerMask) && hit.collider.tag != "Player") {
            output.distance = (hit.distance < output.distance) ? hit.distance : output.distance;
        }

        if (Physics.Linecast(outerSub, points.TopRight, out hit) && hit.collider.tag != "Player") {
            output.distance = (hit.distance < output.distance) ? hit.distance : output.distance;
        }

        if (Physics.Linecast(outerSub, points.BotLeft, out hit) && hit.collider.tag != "Player") {
            output.distance = (hit.distance < output.distance) ? hit.distance : output.distance;
        }

        if (Physics.Linecast(outerSub, points.BotRight, out hit) && hit.collider.tag != "Player") {
            output.distance = (hit.distance < output.distance) ? hit.distance : output.distance;
        }

        if (Physics.Linecast(outerSub, points.Back, out hit) && hit.collider.tag != "Player") {
            output.distance = (hit.distance < output.distance) ? hit.distance : output.distance;
        }

        if (output.distance < float.MaxValue) {
            output.isOccluded = true;
        }

        return output;

    }

    private float CalculateBetterDistance(float desired) {

        float nudgedDistance = currDistance;

        for (int i = 0; i < incrementMaxSteps; i++) {
            nudgedDistance -= incrementDistance;
            Vector3 nudged = Vector3.MoveTowards(subPos, camPos, nudgedDistance);
            if (!GetOcclusion(nudged).isOccluded) {
                // if nudge isn't occluded, that's best case
                return nudgedDistance;
            }
        }

        // if nudge was always occluded, give up
        return desired;

    }
    
    private void UpdateCamera(float desiredDistance) {
        if (desiredDistance < minimumDistance) {
            desiredDistance = minimumDistance;
        }
        cam.transform.position = Vector3.MoveTowards(subPos, camPos, desiredDistance);
    }

    private void DrawKeyPoints(KeyCameraPoints points) {
        Debug.DrawLine(subPos, points.Back, Color.blue);

        Debug.DrawLine(subPos, points.TopLeft, Color.red);
        Debug.DrawLine(subPos, points.TopRight, Color.red);
        Debug.DrawLine(subPos, points.BotLeft, Color.red);
        Debug.DrawLine(subPos, points.BotRight, Color.red);

        Debug.DrawLine(points.TopLeft, points.TopRight, Color.red);
        Debug.DrawLine(points.TopRight, points.BotRight, Color.red);
        Debug.DrawLine(points.BotRight, points.BotLeft, Color.red);
        Debug.DrawLine(points.BotLeft, points.TopLeft, Color.red);

        Debug.DrawLine(points.TopLeft, camPos + transform.forward * -cam.nearClipPlane, Color.red);
        Debug.DrawLine(points.TopRight, camPos + transform.forward * -cam.nearClipPlane, Color.red);
        Debug.DrawLine(points.BotRight, camPos + transform.forward * -cam.nearClipPlane, Color.red);
        Debug.DrawLine(points.BotLeft, camPos + transform.forward * -cam.nearClipPlane, Color.red);

    }

    private float GetSign(float a) {
        if (a < 0f) { return -1f; }
        if (a > 0f) { return 1f; }
        else { return 0f; }
    }

}

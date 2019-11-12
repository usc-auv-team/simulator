using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterEffectManager : MonoBehaviour {

    [SerializeField] private GameObject waterObject = null;

    private enum Location : uint {
        Below,
        Between,
        Above
    };

    private Location cameraLocation = Location.Below;

    private UnderwaterFog fog;
    private UnderwaterDistortion distortion;

    void Start() {
        if (!waterObject) { Debug.LogError("No water object set."); }

        fog = GetComponent<UnderwaterFog>();
        distortion = GetComponent<UnderwaterDistortion>();
    }

    private void Update() {
        UpdateCameraLocation();

        if (cameraLocation == Location.Below) {
            if (distortion) { distortion.ShouldRender(true); }
            if (fog) { fog.ShouldRender(true); }
        }
        else {
            if (distortion) { distortion.ShouldRender(false); }
            if (fog) { fog.ShouldRender(false); }
        }
    }

    // Find where the camera is in relation to the water object
    private void UpdateCameraLocation() {
        float waterLine = waterObject.transform.position.y + waterObject.transform.localScale.y / 2f;

        if (transform.position.y < waterLine) {
            cameraLocation = Location.Below;
        }
        else {
            cameraLocation = Location.Above;
        }
    }
}

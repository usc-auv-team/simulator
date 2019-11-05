using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour {


    // ******************************************************
    // Relevant Camera Components Class

    // Relevant components specific for this script
    private class CameraComponents {

        // Camera component
        public Camera cam = null;

        // Audio Listener component
        public AudioListener audio = null;

        // Set whether components are enabled or not
        public void SetEnabled(bool enabled) {
            cam.enabled = enabled;
            audio.enabled = enabled;
        }

        // Toggle the enabled state of components
        public void Toggle() {
            cam.enabled = !cam.enabled;
            audio.enabled = !audio.enabled;
        }
    }

    // ******************************************************
    // Camera State Enum

    // Describes state of the active camera
    private enum CameraState { FIRST, THIRD }

    // ******************************************************
    // Game Object References

    // Reference to the First Person camera
    [SerializeField] private GameObject CameraObjectFirst = null;

    // Reference to the Third Person camera
    [SerializeField] private GameObject CameraObjectThird = null;

    // Reference to third person controller script
    private CameraThirdPersonController TPController = null;

    // ******************************************************
    // Fields

    // First person camera components
    private CameraComponents camFirst;

    // Third person camera components
    private CameraComponents camThird;

    // State of the active camera
    private CameraState state = CameraState.THIRD;

    // ******************************************************
    // Monobehavior Methods

    private void Start() {
        SetObjectReferences();
        camFirst.SetEnabled(false);
        camThird.SetEnabled(true);

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            ToggleCameraState();
            ToggleActiveCamera();
        }
    }

    // ******************************************************
    // Private Methods

    // Set both game object and component references at the start
    private void SetObjectReferences() {
        if (!CameraObjectFirst) {
            Debug.LogError("First Person Camera not set!");
        }
        if (!CameraObjectThird) {
            Debug.LogError("Third Person Camera not set!");
        }

        camFirst = new CameraComponents {
            cam = CameraObjectFirst.GetComponent<Camera>(),
            audio = CameraObjectFirst.GetComponent<AudioListener>()
        };

        camThird = new CameraComponents {
            cam = CameraObjectThird.GetComponent<Camera>(),
            audio = CameraObjectThird.GetComponent<AudioListener>()
        };

        TPController = GetComponent<CameraThirdPersonController>();
    }

    // Toggles state of active camera and whether TPController is enabled
    private void ToggleCameraState() {
        switch (state) {
            case CameraState.FIRST:
                state = CameraState.THIRD;
                if (TPController) { TPController.enabled = true; }
                break;
            case CameraState.THIRD:
                state = CameraState.FIRST;
                if (TPController) { TPController.enabled = false; }
                break;
        }
    }

    // Toggles the components of both FP and TP cameras
    private void ToggleActiveCamera() {
        camFirst.Toggle();
        camThird.Toggle();
    }
}

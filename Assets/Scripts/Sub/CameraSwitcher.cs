using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour {


    // ******************************************************
    // Relevant Camera Components Class

    private class CameraComponents {
        public Camera cam = null;
        public AudioListener audio = null;

        public void SetState(bool enabled) {
            cam.enabled = enabled;
            audio.enabled = enabled;
        }

        public void Toggle() {
            cam.enabled = !cam.enabled;
            audio.enabled = !audio.enabled;
        }
    }

    // ******************************************************
    // Camera State Enum

    private enum CameraState { FIRST, THIRD }

    // ******************************************************
    // Game Object References

    [SerializeField] private GameObject CameraObjectFirst = null;
    [SerializeField] private GameObject CameraObjectThird = null;
    private CameraThirdPersonController TPController = null;

    // ******************************************************
    // Fields

    private CameraComponents camFirst;
    private CameraComponents camThird;
    private CameraState state = CameraState.THIRD;

    // ******************************************************
    // Monobehavior Methods

    private void Start() {
        SetObjectReferences();
        camFirst.SetState(true);
        camThird.SetState(false);

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            ToggleCameraState();
            ToggleActiveCamera();
        }
    }

    // ******************************************************
    // Private Methods

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

    private void ToggleActiveCamera() {
        camFirst.Toggle();
        camThird.Toggle();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCameraInteractivity : MonoBehaviour {

    enum Location : uint {
        Below,
        Between,
        Above
    };

    [SerializeField] GameObject topWater = null;
    [SerializeField] GameObject botWater = null;

    // Camera currently being used to render
    Camera cam = null;

    void Start() {
        cam = Camera.main;
        if (!cam) { Debug.LogError("Failed to find current camera in scene."); }

        if (!topWater) { Debug.LogError("Failed to find reference to top water game object."); }
        if (!botWater) { Debug.LogError("Failed to find reference to bottom water game object."); }

        // Configure which water object to render from the start
        UpdateWaterLevel();
        
    }

    void Update() {
        if (cam.transform.hasChanged) { UpdateWaterLevel(); }
    }

    // Since only one water object is visible to the camera
    // disable the other depending on the position of the camera.
    void UpdateWaterLevel() {
        Location loc = GetCameraLocation();

        switch (loc) {
            case Location.Below:
                topWater.SetActive(false);
                botWater.SetActive(true);
                break;
            case Location.Between:
                topWater.SetActive(true);
                botWater.SetActive(true);
                break;
            case Location.Above:
                topWater.SetActive(true);
                botWater.SetActive(false);
                break;
        }
    }

    // Find where the camera is in relation to the two water objects
    private Location GetCameraLocation() {
        float camY = cam.transform.position.y;
        float botY = botWater.transform.position.y;
        float topY = topWater.transform.position.y;

        // Error check to make sure they're top and bot
        if (botY > topY) { Debug.LogError("Bottom water is positioned above top water."); }

        if (camY < botY) {
            return Location.Below;
        }
        else if (camY < topY) {
            return Location.Between;
        }
        else {
            return Location.Above;
        }
    }
}

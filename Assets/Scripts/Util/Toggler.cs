using UnityEngine;

public class Toggler : MonoBehaviour {

    [SerializeField] bool defaultActiveState;
    private bool isActive;

    public void Start() {
        // By default, this game object should be disabled
        gameObject.SetActive(defaultActiveState);
        isActive = defaultActiveState;
    }

    public void ToggleActive() {
        isActive = !isActive;
        gameObject.SetActive(isActive);
    }
}

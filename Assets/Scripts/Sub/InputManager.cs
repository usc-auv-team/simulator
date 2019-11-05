using UnityEngine;
using System.Collections;

/// <summary>
/// The main <c>InputManager</c> class
/// An abstract class that serves as a base for all <c>InputManager</c> classes
/// </summary>
public abstract class InputManager : MonoBehaviour {
    private bool up = false;
    private bool down = false;
    private bool forward = false;
    private bool backward = false;
    private bool left = false;
    private bool right = false;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        CheckInputs();
    }

    /// <summary>
    /// This method checks for input and updates the movement booleans
    /// </summary>
    protected abstract void CheckInputs();

    /// <summary>
    /// This method retrieves the movement booleans
    /// </summary>
    protected abstract void GetInputs();
}

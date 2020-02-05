using UnityEngine;
using System.Collections;

/// <summary>
/// The main <c>InputManager</c> class
/// An abstract class that serves as a base for all <c>InputManager</c> classes
/// </summary>
public abstract class InputManager : MonoBehaviour {
    public Vector3 Direction { get; protected set; }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        CheckInputs();
    }

    /// <summary>
    /// This method checks for input and updates the movement vector
    /// </summary>
    protected abstract void CheckInputs();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputManager : InputManager {
    const KeyCode Up = KeyCode.Space;
    const KeyCode Down = KeyCode.LeftShift;
    const KeyCode Forward = KeyCode.W;
    const KeyCode Backward = KeyCode.S;
    const KeyCode Left = KeyCode.A;
    const KeyCode Right = KeyCode.D;

    // Start is called before the first frame update
    void Start() {
        
    }

    protected override void CheckInputs() {
        Vector3 oldDirection = Direction;

        Direction = Vector3.zero;
        if (Input.GetKey(Up)) {
            Direction += Vector3.up;
        }
        if (Input.GetKey(Down)) {
            Direction += Vector3.down;
        }
        if (Input.GetKey(Forward)) {
            Direction += Vector3.forward;
        }
        if (Input.GetKey(Backward)) {
            Direction += Vector3.back;
        }
        if (Input.GetKey(Left)) {
            Direction += Vector3.left;
        }
        if (Input.GetKey(Right)) {
            Direction += Vector3.right;
        }

        if (oldDirection != Direction) {
            Debug.Log(Direction);
        }
    }
}
 
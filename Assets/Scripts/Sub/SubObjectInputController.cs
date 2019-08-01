using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubObjectInputController : MonoBehaviour {

    private RigidbodyManager rbm;

    private void Start() {
        rbm = GetComponent<RigidbodyManager>();
    }

    private bool up = false;
    [SerializeField] private float upForce = 1f;

    private bool down = false;
    [SerializeField] private float downForce = 1f;

    private bool forward = false;
    [SerializeField] private float forwardForce = 1f;

    private bool backward = false;
    [SerializeField] private float backwardForce = 1f;

    private bool left = false;
    [SerializeField] private float leftForce = 1f;

    private bool right = false;
    [SerializeField] private float rightForce = 1f;

    private void Update() {
        CheckInput(KeyCode.Space, ref up);
        CheckInput(KeyCode.LeftShift, ref down);
        CheckInput(KeyCode.W, ref forward);
        CheckInput(KeyCode.S, ref backward);
        CheckInput(KeyCode.A, ref left);
        CheckInput(KeyCode.D, ref right);
    }

    private void CheckInput(KeyCode key, ref bool movement) {
        if (Input.GetKeyDown(key)) {
            movement = true;
        }
        else if (Input.GetKeyUp(key)) {
            movement = false;
        }
    }

    private void FixedUpdate() {
        ApplyForce(up, Vector3.up, upForce);
        ApplyForce(down, Vector3.down, downForce);
        ApplyForce(forward, Vector3.forward, forwardForce);
        ApplyForce(backward, Vector3.back, backwardForce);
        ApplyTorque(left, Vector3.down, leftForce);
        ApplyTorque(right, Vector3.up, rightForce);
    }

    private void ApplyForce(bool movement, Vector3 direction, float force) {
        if (movement) {
            rbm.AddRelativeForce(direction * force);
        }
    }
    private void ApplyTorque(bool movement, Vector3 direction, float force) {
        if (movement) {
            rbm.AddRelativeTorque(direction * force);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMovement : MonoBehaviour {

    // ============================================================
    // Serialized fields

    [SerializeField] private float upForce = 1f;
    [SerializeField] private float downForce = 1f;
    [SerializeField] private float forwardForce = 1f;
    [SerializeField] private float backwardForce = 1f;
    [SerializeField] private float leftForce = 1f;
    [SerializeField] private float rightForce = 1f;

    // ============================================================
    // Private fields

    private RigidbodyManager rbm;

    private bool up = false;
    private bool down = false;
    private bool forward = false;
    private bool backward = false;
    private bool left = false;
    private bool right = false;

    // ============================================================
    // Private MonoBehavior methods

    private void Start() {
        rbm = GetComponent<RigidbodyManager>();
    }

    private void Update() {
        CheckInput(KeyCode.Space, ref up);
        CheckInput(KeyCode.LeftShift, ref down);
        CheckInput(KeyCode.W, ref forward);
        CheckInput(KeyCode.S, ref backward);
        CheckInput(KeyCode.A, ref left);
        CheckInput(KeyCode.D, ref right);
    }

    private void FixedUpdate() {
        ApplyForce(up, Vector3.up, upForce);
        ApplyForce(down, Vector3.down, downForce);
        ApplyForce(forward, Vector3.forward, forwardForce);
        ApplyForce(backward, Vector3.back, backwardForce);
        ApplyTorque(left, Vector3.down, leftForce);
        ApplyTorque(right, Vector3.up, rightForce);
    }

    // ============================================================
    // Private class methods

    private void CheckInput(KeyCode key, ref bool movement) {
        movement = Input.GetKey(key);
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
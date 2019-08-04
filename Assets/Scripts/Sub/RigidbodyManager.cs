using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyManager : MonoBehaviour {

    // ============================================================
    // Game object references

    [SerializeField] private GameObject waterVolume = null;

    // ============================================================
    // Rigidbody properties

    private Rigidbody rb = null;

    public float Mass = 1f;
    public float Drag = 1f;
    public float AngularDrag = 1f;
    public bool UseGravity = false;

    // ============================================================
    // Measurements

    public Vector3 Acceleration { get; private set; } = Vector3.zero;
    public Vector3 Velocity { get; private set; } = Vector3.zero;
    public Vector3 Position { get; private set; } = Vector3.zero;

    public Vector3 AngularAcceleration { get; private set; } = Vector3.zero;
    public Vector3 AngularVelocity { get; private set; } = Vector3.zero;
    public Vector3 Rotation { get; private set; } = Vector3.zero;

    public Vector3 Displacement { get; private set; } = Vector3.zero;
    public float Distance { get; private set; } = 0f;
    public float Depth { get; private set; } = 0f;

    public float TimeElapsed { get; private set; } = 0f;

    private Vector3 initialPosition = Vector3.zero;

    // ============================================================
    // Public methods

    public void AddRelativeForce(Vector3 force) {
        rb.AddRelativeForce(force, ForceMode.Force);
    }

    public void AddRelativeTorque(Vector3 torque) {
        rb.AddRelativeTorque(torque, ForceMode.Force);
    }

    // ============================================================
    // Private methods

    private void Start() {
        if (GetComponent<Rigidbody>()) {
            Debug.LogError("Remove Rigidbody component from " + gameObject.ToString());
        }

        rb = gameObject.AddComponent<Rigidbody>();
        SetRigidbody();

        Position = rb.position;
        initialPosition = rb.position;
    }

    private void Update() {
        SetRigidbody();
    }

    private void FixedUpdate() {
        UpdateFields();
    }

    private void SetRigidbody() {
        rb.mass = Mass;
        rb.drag = Drag;
        rb.angularDrag = AngularDrag;
        rb.useGravity = UseGravity;
    }

    private void UpdateFields() {
        Distance += (rb.position - Position).magnitude;

        Acceleration = (rb.velocity - Velocity) / Time.fixedDeltaTime;
        Velocity = rb.velocity;
        Position = rb.position;

        AngularAcceleration = (rb.angularVelocity - AngularVelocity) / Time.fixedDeltaTime;
        AngularVelocity = rb.angularVelocity;
        Rotation = rb.rotation.eulerAngles;

        Displacement = rb.position - initialPosition;
        Depth = rb.position.y - (waterVolume.transform.position.y + waterVolume.transform.localScale.y / 2f);

        TimeElapsed = Time.time;
    }
}

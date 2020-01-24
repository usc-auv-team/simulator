using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyManager : MonoBehaviour {

// ============================================================
// Rigidbody properties

    private Rigidbody rb = null;

    public float Mass = 1f;
    public float AngularDrag = 1f;

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

    // ============================================================
    // Private fields

    [SerializeField] private float waterDrag = 1f;
    [SerializeField] private float airDrag = 0.05f;
    [SerializeField] [Range(0.8f, 1.2f)] private float buoyancy = 1f;

    private Vector3 initialPosition = Vector3.zero;

    private float subTopPoint = 0f;
    private float subMidPoint = 0f;
    private float subBotPoint = 0f;
    private float waterTopPoint = 0f;

    // ============================================================
    // Public methods

    public void AddRelativeForce(Vector3 force) {
        if (subTopPoint < waterTopPoint) {
            rb.AddRelativeForce(force, ForceMode.Force);
        }
    }

    public void AddRelativeTorque(Vector3 torque) {
        if (subTopPoint < waterTopPoint) {
            rb.AddRelativeTorque(torque, ForceMode.Force);
        }
    }

    // ============================================================
    // MonoBehavior methods

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() { 
        UpdateProperties();
    }

    // ============================================================
    // Methods

    private void UpdateProperties() {
        Distance += (rb.position - Position).magnitude;

        Acceleration = (rb.velocity - Velocity) / Time.fixedDeltaTime;
        Velocity = rb.velocity;
        Position = rb.position;

        AngularAcceleration = (rb.angularVelocity - AngularVelocity) / Time.fixedDeltaTime;
        AngularVelocity = rb.angularVelocity;
        Rotation = rb.rotation.eulerAngles;

        Displacement = rb.position - initialPosition;
        
        TimeElapsed = Time.time;
    }
}

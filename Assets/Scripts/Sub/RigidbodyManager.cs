using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyManager : MonoBehaviour {

    /*
     * This class is the authority on all changes to the objects Rigidbody component
     * This is because in order to have accurate readings (especially force related ones),
     * this class needs to receive all inputs in order to calculate the various fields and
     * Unity does not have a sufficient method for viewing the Rigidbody component in complete depth.
     * 
     * Therefore, any script that calls a rigidbody function should call it from this class.
     * 
     * Acceleration
     *   Vector3
     *      X: m/s^2 
     *      Y: m/s^2 
     *      Z: m/s^2
     * Velocity
     *   Vector3
     *      X: m/s
     *      Y: m/s 
     *      Z: m/s
     * Position
     *   Vector3
     *      X: m
     *      Y: m 
     *      Z: m
     * 
     * Angular acceleration
     *   Vector3
     *      X: rad/s^2 
     *      Y: rad/s^2 
     *      Z: rad/s^2
     * Angular velocity
     *   Vector3
     *      X: rad/s
     *      Y: rad/s 
     *      Z: rad/s
     * Rotation
     *   Vector3
     *      X: rad
     *      Y: rad 
     *      Z: rad
     * 
     * Displacement
     *   Vector3
     *   Difference of the object's starting position from its current position
     *      X: m
     *      Y: m 
     *      Z: m
     * Distance
     *   Vector3
     *   Total meters travelled by the object since t=0
     *      m
     * Depth
     *   float
     *   Distance the object is below the water line
     *      m
     *      
     *      
     * Time Elapsed
     *   float
     *   Total time that has passed since t=0
     * 
     */
    

    // ============================================================
    // Rigidbody related fields and properties
    private Rigidbody rb = null;

    [SerializeField] private float mass = 1f;
    [SerializeField] private float drag = 1f;
    [SerializeField] private float angularDrag = 1f;
    [SerializeField] private bool useGravity = false;

    public float Mass { get => mass; set => mass = value; }
    public float Drag { get => drag; set => drag = value; }
    public float AngularDrag { get => angularDrag; set => angularDrag = value; }
    public bool UseGravity { get => useGravity; set => useGravity = value; }

    // ============================================================
    // Information related
    private Vector3 acceleration { get; }
    private Vector3 velocity { get; }
    private Vector3 position { get; }

    private Vector3 angularAcceleration { get; }
    private Vector3 angularVelocity { get; }
    private Vector3 rotation { get; }

    private Vector3 displacement { get; }
    private float distance { get; }
    private float depth { get; }

    private float timeElapsed { get; }

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

        AddAndSetRigidbody();

    }

    private void AddAndSetRigidbody() {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.useGravity = useGravity;
    }
}

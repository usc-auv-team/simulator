using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Information : MonoBehaviour {

    private RigidbodyManager rbm;

    public Vector3 acceleration = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public Vector3 position = Vector3.zero;

    public Vector3 angularAcceleration = Vector3.zero;
    public Vector3 angularVelocity = Vector3.zero;
    public Vector3 rotation = Vector3.zero;

    public Vector3 displacement = Vector3.zero;
    public float distance = 0f;
    public float depth = 0f;

    public float timeElapsed = 0f;

    private void Start() {
        rbm = GetComponent<RigidbodyManager>();
    }

    private void Update() {
        acceleration = rbm.Acceleration;
        velocity = rbm.Velocity;
        position = rbm.Position;

        angularAcceleration = rbm.AngularAcceleration;
        angularVelocity = rbm.AngularVelocity;
        rotation = rbm.Rotation;

        displacement = rbm.Displacement;
        distance = rbm.Distance;
        depth = rbm.Depth;

        timeElapsed = rbm.TimeElapsed;
    }
}

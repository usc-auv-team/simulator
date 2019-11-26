using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forcethang : MonoBehaviour
{
    public float f = 9.81f;
    Vector3 lastPos;
    Vector3 velocity1 = Vector3.zero;
    public float velocity2;
    public float acceleration;
    private Rigidbody rb;

    void Start(){
        rb = GetComponent<Rigidbody>();
        lastPos = transform.position;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        velocity2 = rb.velocity.magnitude;
        if (Input.GetKey(KeyCode.A)) { rb.AddForce(f * Vector3.up); }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AUVController : MonoBehaviour {

	public Text position;

	Rigidbody rb;
	float speed;

	// Use this for initialization
	void Start () {
		speed = 5.0f;
		rb = GetComponent<Rigidbody> ();
		Debug.Log (GetComponent<MeshFilter>().mesh.bounds);
	}
	
	// Update is called once per frame
	void Update () {
		updatePositionText ();
	}

	void FixedUpdate () {
		// dummy AUV control script, only contains 2-D control
		// by default arrow keys and "WASD" should both work
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);

		// I choosed force control instead of direct transform control for the AUV because
		// it seems more realistic to apply force to the sub.
		rb.AddForce (movement * speed);

		// TODO: set differnt mode of controlling the sub including the at least one for 
		// convenience and one for more realistic test 
	}

	// Updates the position and velocity information of our AUV
	void updatePositionText() {
		position.text = "AUV position:" +
		"\nx: " + gameObject.transform.position.x.ToString () +
		"\ny: " + gameObject.transform.position.y.ToString () +
		"\nz: " + gameObject.transform.position.z.ToString () +
		"\nAUV velocity:" +
		"\nx: " + rb.velocity.x.ToString () +
		"\ny: " + rb.velocity.y.ToString () +
		"\nz: " + rb.velocity.z.ToString ();
	}
}

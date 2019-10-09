using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicCamera : MonoBehaviour
{
    public GameObject bigBoyObject;
    public float CameraDist = 5f;
    private Vector3 distance = new Vector3(0, 5f, 0);

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            distance = new Vector3(0, CameraDist, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            distance = new Vector3(0, CameraDist, CameraDist);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            distance = new Vector3(0, 0, -CameraDist);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            distance = new Vector3(0, 0, CameraDist);
        }

        transform.position = bigBoyObject.transform.position + distance;
        transform.LookAt(bigBoyObject.transform);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCamera : MonoBehaviour
{
    private float pitch = 0;
    private float yaw = 0;
    public float sensitivity = 1;

    void Update()
    {
        yaw += sensitivity * Input.GetAxis("Mouse X");
        pitch += sensitivity * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(pitch, -yaw, 0);
    }
}

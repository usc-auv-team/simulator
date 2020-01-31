using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    public Camera cam;

    private void Start() {
        cam = Camera.main;
    }

    // Update is called once per frame
    private void OnDrawGizmos() {
        Vector2 bl = new Vector2(10, 10), tr = new Vector2(60, 60);
        DrawSquare(bl, tr);
    }

    private void OnGUI() {
        GUI.BeginGroup(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 100));
       
        GUI.EndGroup();
    }

}

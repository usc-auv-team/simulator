using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    private Camera cam;
    private float xMax, xMin, yMax, yMin;
    private Mesh mesh;
    private static Texture2D whiteTexture;


    private void Start() {
        cam = Camera.main;
        MeshFilter mf = GetComponent<MeshFilter>();
        if(mf == null) {
            mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }
        else {
            mesh = mf.mesh;
        }

        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
    }

    private void OnGUI() {
        CalculateAndDrawBorders();
    }

    private void CalculateAndDrawBorders() {
        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        xMax = 0;
        xMin = int.MaxValue;
        yMax = 0;
        yMin = int.MaxValue;

        for (int i = 0; i < mesh.vertexCount; i++) {
            Vector2 v = cam.WorldToScreenPoint(localToWorld.MultiplyPoint3x4(mesh.vertices[i]));
            if (v.x < xMin) {
                xMin = v.x;
            }
            else if (v.x > xMax) {
                xMax = v.x;
            }
            if (v.y < yMin) {
                yMin = v.y;
            }
            else if (v.y > yMax) {
                yMax = v.y;
            }
        }
        xMax = Mathf.Clamp(xMax, 0, Screen.width);
        xMin = Mathf.Clamp(xMin, 0, Screen.width);
        yMax = Mathf.Clamp(yMax, 0, Screen.height);
        yMin = Mathf.Clamp(yMin, 0, Screen.height);

        DrawScreenRect(new Rect((int)xMin, (int)(Screen.height - yMin), 
        (int)(xMax - xMin), (int)(-yMax + yMin)), new Color(0, 0, 1, 0.2f));
    }

    public static void DrawScreenRect(Rect rect, Color color) {
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTexture);
        GUI.color = Color.white;
    }
}
